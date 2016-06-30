using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coroutine
{
    public delegate void CompletedHandler(object result);
    public delegate void ActionHandler(CoroutineTask previousTask, CompletedHandler onComplete, ErrorHandler onError);
    public delegate void ErrorHandler(Exception e);
    public delegate void FinallyHandler();

    public class CoroutineTask
    {

        private ActionHandler onAction;
        private CompletedHandler onCompleted;
  
        private ErrorHandler onError;
        private FinallyHandler onFinally;

        private CoroutineTask parentTask;
        private CoroutineTask childTask;
        public object Result { get;private set; }
        public CoroutineTask(ActionHandler actionHandler)
        {
            onAction = actionHandler;
        }

        public T ResultAs<T>()
        {
            if (Result is T)
            {
                return (T) Result;
            }
            else
            {
                throw new Exception("");
            }
        }
        public CoroutineTask Then(ActionHandler actionHandler)
        {
            CoroutineTask next=new CoroutineTask(actionHandler);
            next.parentTask = this;
            this.childTask = next;
            return next;
        }

        public CoroutineTask Catch(ErrorHandler errorHandler)
        {
            if (errorHandler != null)
            {
                onError += errorHandler;
            }

            return this;
        }

        public CoroutineTask Finally(FinallyHandler action)
        {
            if (action != null)
            {
                onFinally += action;
            }

            return this;
        }

        public void Start(CompletedHandler completedHandler = null, ErrorHandler errorHandler = null)
        {
            CoroutineTask rootTask = this;
            while (rootTask.parentTask!=null)
            {
                rootTask = rootTask.parentTask;
            }
            if (rootTask==this)
            {
                rootTask.ExecuteTask(null,completedHandler,errorHandler);
            }
            else
            {
                if (completedHandler!=null)
                {
                    this.onCompleted += completedHandler;
                }
                if (errorHandler!=null)
                {
                    this.onError += errorHandler;
                }

                 rootTask.ExecuteTask(null,null,null);
                
            }
           
        }

        private void ExecuteTask(CoroutineTask prevResult = null, CompletedHandler completedHandler = null, ErrorHandler errorHandler = null)
        {
            if (completedHandler != null)
            {
                onCompleted += completedHandler;
            }
            if (errorHandler!=null)
            {
                onError += errorHandler;
            }

            try
            {
                onAction(prevResult, Completed,Error);
            }
            catch (Exception e)
            {
                
                Error(e);
            }
        }

        private void Completed(object result)
        {
            if (onCompleted != null)
            {
                onCompleted(result);
            }
            Result = result;
            if (childTask!=null)
            {
                 childTask.ExecuteTask(this,null);
            }
            if (onFinally!=null)
            {
                onFinally();
            }
        }

        private void Error(Exception e)
        {
            if (onError!=null)
            {
                onError(e);
            }
            if (childTask != null)
            {
                childTask.Error(e);
            }
            if (onFinally != null)
            {
                onFinally();
            }
        }
    }
}

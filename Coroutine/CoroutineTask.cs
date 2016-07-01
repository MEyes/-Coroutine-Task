using System;

namespace Coroutine
{
    //申明委托，将具体的操作交给开发人员
    //CompletedHandler：完成时，接下来做什么
    public delegate void CompletedHandler(object result);
    //ActionHandler:怎么做
    public delegate void ActionHandler(CoroutineTask previousTask, CompletedHandler onComplete, ErrorHandler onError);
    //ErrorHandler：异常了，怎么做
    public delegate void ErrorHandler(Exception e);
    //FinallyHandler：最后的扫尾工作
    public delegate void FinallyHandler();
    /// <summary>
    /// 代表协程任务
    /// </summary>
    public class CoroutineTask
    {
        private ActionHandler _onOnAction;
        private CompletedHandler _onCompleted;

        private ErrorHandler _onError;
        private FinallyHandler _onFinally;

        private CoroutineTask _parentTask;
        private CoroutineTask _childTask;
        public object Result { get; private set; }

        public CoroutineTask(ActionHandler onAction)
        {
            _onOnAction = onAction;
        }

        public T ResultAs<T>()
        {
            if (Result is T)
            {
                return (T) Result;
            }
            throw new Exception("cast failed");
        }

        public CoroutineTask Then(ActionHandler onAction)
        {
            var next = new CoroutineTask(onAction);
            //设置父 Task
            next._parentTask = this;
            //设置子 Task
            this._childTask = next;
            return next;
        }
        /// <summary>
        /// 捕获的异常，放在最外层
        /// </summary>
        /// <param name="onError"></param>
        /// <returns></returns>
        public CoroutineTask Catch(ErrorHandler onError)
        {
            if (onError != null)
            {
                _onError += onError;
            }

            return this;
        }
        /// <summary>
        /// 扫尾工作，放在最外层
        /// </summary>
        /// <param name="onFinally"></param>
        /// <returns></returns>
        public CoroutineTask Finally(FinallyHandler onFinally)
        {
            if (onFinally != null)
            {
                _onFinally += onFinally;
            }

            return this;
        }
        /// <summary>
        /// 启动 Coroutine Task
        /// </summary>
        /// <param name="onCompleted"></param>
        /// <param name="onError"></param>
        public void Start(CompletedHandler onCompleted = null, ErrorHandler onError = null)
        {
            var rootTask = this;
            while (rootTask._parentTask != null)
            {
                //循环，找到第一个 Task
                rootTask = rootTask._parentTask;
            }
            if (rootTask == this)
            {
                rootTask.ExecuteTask(null, onCompleted, onError);
            }
            else
            {
                //注意 this，代表离 Start 方法最近的 Task 实例
                if (onCompleted != null)
                {
                    this._onCompleted += onCompleted;
                }
                if (onError != null)
                {
                    this._onError += onError;
                }

                rootTask.ExecuteTask(null, null, null);
            }
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="previousTask"></param>
        /// <param name="onCompleted"></param>
        /// <param name="onError"></param>
        private void ExecuteTask(CoroutineTask previousTask = null, CompletedHandler onCompleted = null, ErrorHandler onError = null)
        {
            if (onCompleted != null)
            {
                _onCompleted += onCompleted;
            }
            if (onError != null)
            {
                _onError += onError;
            }

            try
            {
                //调用委托指向的方法
                _onOnAction(previousTask, Completed, Error);
            }
            catch (Exception e)
            {
                Error(e);
            }
        }
        /// <summary>
        /// Completed，回掉此方法将执行下一个任务
        /// </summary>
        /// <param name="result"></param>
        private void Completed(object result)
        {
            if (_onCompleted != null)
            {
                _onCompleted(result);
            }
            Result = result;
            if (_childTask != null)
            {
                _childTask.ExecuteTask(this, null, null);
            }
            if (_onFinally != null)
            {
                _onFinally();
            }
        }
        /// <summary>
        /// Error，回掉此方法将 Catch 异常
        /// </summary>
        /// <param name="e"></param>
        private void Error(Exception e)
        {
            if (_onError != null)
            {
                _onError(e);
            }
            if (_childTask != null)
            {
                _childTask.Error(e);
            }
            if (_onFinally != null)
            {
                _onFinally();
            }
        }
    }
}

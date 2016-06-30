using System;

namespace Coroutine
{
    class Program
    {
        static void Main(string[] args)
        {
            CoroutineTask task=null;
            new CoroutineTask((prevResult, onComplete,onError) =>
            {
                Console.WriteLine("1");
                onComplete(1);
            })
            .Then((prevResult, onComplete, onError) =>
            {
              
                var x = prevResult.Result.ToString();
                Console.WriteLine("2-"+x);

                var xx=task.Result.ToString();

                new CoroutineTask((a, b, c) =>
                {
                    Console.WriteLine("a");
                    b("a");
                })
               .Then((a, b,c) =>
                {
                    Console.WriteLine("b");
                    b("b");
                })
                .Start(onComplete);

            })
            .Then((prevResult, onComplete, onError) =>
            {
                var y = prevResult.Result.ToString();
                Console.WriteLine("3-"+y);
                new CoroutineTask((a, b,c) =>
                {
                    Console.WriteLine("aa");
                    b("aa");
                })
               .Start(onComplete);
            })
            .Then((prevResult, onComplete, onError) =>
            {
                var z= prevResult.Result.ToString();
                Console.WriteLine("4-"+z);
                onComplete(4);
            })
            .Catch((e) =>
            {
                Console.WriteLine("异常:"+e.Message);
            })
            .Finally(() =>
            {
                Console.WriteLine("Finally");
            })
            .Start();

            Console.ReadKey();
        }

    }
}

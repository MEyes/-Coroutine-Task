using System;

namespace Coroutine
{
    class Program
    {
        static void Main(string[] args)
        {
            new CoroutineTask((prevResult, onComplete,onError) =>
            {
                Console.WriteLine("1");
              
                onComplete(1);
            })
            .Then((prevResult, onComplete, onError) =>
            {
              
                var x = prevResult.Result.ToString();
                Console.WriteLine("2-"+x);

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
                .Start(onComplete,onError);

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
               .Start(onComplete,onError);
            })
            .Then((prevResult, onComplete, onError) =>
            {
                var z= prevResult.Result.ToString();
               
                Console.WriteLine("4-"+z);
                onComplete(4);
            })
            .Catch((e) =>
            {
                Console.WriteLine("throw a exception:"+e.Message);
            })
            .Finally(() =>
            {
                Console.WriteLine("end");
            })
            .Start();

            Console.ReadKey();
        }

    }
}

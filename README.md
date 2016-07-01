

A implementation of promise for C#. Inspired by JQuery. 

The library make coroutine task easy. 

Below is an example of start coroutine task. 

```
new CoroutineTask((previousTask, onComplete, onError) =>
{
    Console.WriteLine("1");
    onComplete(1);
})
.Then((previousTask, onComplete, onError) =>
{
    var result = previousTask.Result.ToString();
    Console.WriteLine("2-" + result);
    new CoroutineTask((a, b, c) =>
    {
        Console.WriteLine("a");
        b("a");
    })
    .Then((a, b, c) =>
    {
        Console.WriteLine("b");
        b("b");
    })
    .Start(onComplete, onError);
})
.Then((previousTask, onComplete, onError) =>
{
    var result = previousTask.Result.ToString();
    Console.WriteLine("3-" + result);
    new CoroutineTask((a, b, c) =>
    {
        Console.WriteLine("aa");
        b("aa");
    })
    .Start(onComplete, onError);
})
.Then((previousTask, onComplete, onError) =>
{
    var z = previousTask.Result.ToString();
    Console.WriteLine("4-" + z);
    onComplete(4);
})
.Catch(e => { Console.WriteLine("throw a exception:" + e.Message); })
.Finally(() => { Console.WriteLine("end"); })
.Start();
Console.ReadKey();

Output:
1 
2-1 
a 
b 
3-b 
aa 
4-aa 
end 
```




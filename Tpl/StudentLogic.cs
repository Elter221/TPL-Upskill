namespace Tpl;

public static class StudentLogic
{
    public static Task TaskCreated()
    {
        Task task = new Task(() => { });

        return task;
    }

    public static Task WaitingForActivation()
    {
        Task task = Foo(5);

        return task;
    }

    public static Task WaitingToRun()
    {
        Task task = new Task(() =>
        {
            _ = Foo(5);
        });

        task.Start(TaskScheduler.Default);
        return task;
    }

    public static Task Running()
    {
        var task = Task.Run(() => { Task.Delay(TimeSpan.FromSeconds(5)).Wait(); });
        Task.Delay(TimeSpan.FromSeconds(2)).Wait();
        return task;
    }

    public static Task RanToCompletion()
    {
        Task task = Task.Run(() => Foo(5));
        task.Wait();
        return task;
    }

    public static Task WaitingForChildrenToComplete()
    {
        var task = Task.Factory.StartNew(static () =>
        {
            Task.Factory.StartNew(
                static () =>
                {
                    Task.Delay(TimeSpan.FromSeconds(7)).Wait();
                }, TaskCreationOptions.AttachedToParent);
        });

        Task.Delay(TimeSpan.FromSeconds(1)).Wait();
        return task;
    }

    public static Task IsCompleted()
    {
        Task task = Task.Run(() => Foo(5).Wait());
        task.Wait();
        return task;
    }

    public static Task IsCancelled()
    {
        var tokenSource = new CancellationTokenSource();
        var ct = tokenSource.Token;
        var task = Task.Run(
            () =>
            {
                Task.Delay(TimeSpan.FromSeconds(8), ct).Wait(ct);
            }, ct);

        tokenSource.CancelAsync();
        try
        {
            task.Wait(ct);
            return task;
        }
        catch (OperationCanceledException)
        {
            return task;
        }
        finally
        {
            tokenSource.Dispose();
        }
    }

    public static Task IsFaulted()
    {
        Task task = Task.Run(() =>
        {
            throw new NotImplementedException();
        });

        try
        {
            task.Wait();
            return task;
        }
        catch (AggregateException)
        {
            return task;
        }
    }

    public static List<int> ForceParallelismPlinq()
    {
        var testList = Enumerable.Range(1, 300).ToList();
        var result = testList.AsParallel().Select(x => x * x).ToList();
        return result;
    }

    private static async Task<string> Foo(int seconds)
    {
        return await Task.Run(() =>
        {
            for (int i = 0; i < seconds; i++)
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }

            return "Foo Completed";
        });
    }

}

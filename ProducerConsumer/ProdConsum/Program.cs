using System.Collections.Concurrent;
using System.Threading;

namespace ProdConsum
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string stopSignal = null;
            SafeStack<string> safeStack = new SafeStack<string>();
            Random rand = new Random();
            var mutex = new Mutex();
            var path = @"logs.txt";
            StreamWriter file = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), path));
            object _lockObject = new object();
           
            var producer = new Thread(() =>
            {
                int randomNumber;
                for(; stopSignal == null;)
                {

                    mutex.WaitOne();
                    try
                    {
                        randomNumber = rand.Next(0, 100);
                        safeStack.TryAdd(randomNumber.ToString());
                        Console.WriteLine("ADDED");
                        lock (_lockObject)
                        {
                            file.WriteLine($"{randomNumber} added");
                        }
                    }
                    finally { mutex.ReleaseMutex(); }
                    Thread.Sleep(2000);
                }
            });
            producer.Name = "producer";
            var consumer = new Thread(() =>
            {
                for (;stopSignal == null;)
                {
                    mutex.WaitOne();
                    try
                    {
                        safeStack.TryPop(out var item);
                        if (item != null)
                            Console.WriteLine($"{item} was taken");
                        lock (_lockObject)
                        {
                            file.WriteLine($"{item} was taken");
                        }
                    }
                    finally { mutex.ReleaseMutex(); }

                    Thread.Sleep(2000);
                }
            });
            consumer.Name = "consumer";
            producer.Start();
            consumer.Start();
            stopSignal = Console.ReadLine();
            producer.Join();
            consumer.Join();
            file.Close();
            Console.ReadLine();
        }
    }
}

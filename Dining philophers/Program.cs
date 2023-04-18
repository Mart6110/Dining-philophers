using System;
using System.Threading;

namespace Dining_philophers
{
    class Fork
    {
        static object _lock = new object();

        // Creating a array of bools with a length of 5.
        bool[] fork = new bool[5];
        public void Get(int left, int right)
        {
            Monitor.Enter(_lock);
            try
            {
                // Checking if both left and rigth fork is available and if not we wait for them to be available.
                while (fork[left] || fork[right]) Monitor.Wait(_lock);
                fork[left] = true; 
                fork[right] = true;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        public void Put(int left, int right)
        {
            Monitor.Enter(_lock);
            try
            {
                fork[left] = false;
                fork[right] = false;
                // Telling the other threads that the forks are available.
                Monitor.PulseAll(_lock);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

    }

    class Philo
    {
        int philoNumber;
        int thinkDelay;
        int eatDelay;
        int left;
        int right;
        Fork fork;

        public Philo(int philoNumber, int thinkDelay, int eatDelay, Fork fork)
        {
            this.philoNumber = philoNumber;
            this.thinkDelay = thinkDelay;
            this.eatDelay = eatDelay;
            this.fork = fork;

            // Calculating the fork number
            left = philoNumber == 0 ? 4 : philoNumber - 1;
            right = (philoNumber + 1) % 5;

            // Creating a new thread
            new Thread(new ThreadStart(DinnerReady)).Start();
        }

        public void DinnerReady()
        {
            for(; ; )
            {
                try
                {
                    Console.WriteLine("Philosopher " + philoNumber + " is thinking...");
                    Thread.Sleep(TimeSpan.FromSeconds(thinkDelay));
                    fork.Get(left, right); // Caliing the Get method from the Fork class
                    Console.WriteLine("Philosopher " + philoNumber + " is eating...");
                    Thread.Sleep(TimeSpan.FromSeconds(eatDelay));
                    fork.Put(left, right); // Caliing the Put method from the Fork class
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error caught.", e);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int philoNumber = 0;
            Random rnd = new Random();

            Fork fork = new Fork();
            
            for(int i = 0; i < 5; i++)
            {
                int thinkDelay = rnd.Next(1, 5);
                int eatDelay = rnd.Next(1, 5);
                new Philo(philoNumber, thinkDelay, eatDelay, fork);
                philoNumber++;
            }

        }
    }
}

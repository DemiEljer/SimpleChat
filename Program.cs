namespace SimpleChat
{
    internal class Program
    {
        static ApplicationHandler app { get; } = new();

        static void Main(string[] args)
        {
            Console.WriteLine("Simple chat application has started");

            app.Start();

            SpinWait waiter = new();
            while (app.ApplicationState)
            {
                waiter.SpinOnce();
            }

            Console.WriteLine("\r\nSimple chat application has closed");
        }
    }
}

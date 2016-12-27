using System;

namespace Camera_Server
{
    public static class SynchronousSocketListener
    {
        public static int Main(string[] args)
        {
            Logger.Logger log = new Logger.Logger();
            log.RemoveOldLogs(DateTime.Today.AddMonths(-1));

            Console.WriteLine("Starting");
            try
            {
                Listener listen = new Listener();
                listen.StartListening();
            }
            catch (Exception e)
            {
                Console.WriteLine("CRITICAL EXCEPTION SHUTING DOWN!!!");
                Console.WriteLine(e);
            }
            return 0;
        }
    }
}
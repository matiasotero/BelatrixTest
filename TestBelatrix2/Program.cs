using System;
using TestBelatrix;

namespace TestBelatrix2
{
    class Program
    {
        static void Main(string[] args)
        {
            //Run Example
            var jobLogger = new JobLogger(logToFile: false, logToConsole: true, logToDatabase: false);

            jobLogger.LogMessage("Test", JobLogger.MessageType.WARNING);

            Console.ReadKey();
        }
    }
}

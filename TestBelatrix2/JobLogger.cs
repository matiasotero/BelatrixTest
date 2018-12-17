namespace TestBelatrix2
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class JobLogger
    {

        private static bool _logToFile;
        private static bool _logToConsole;
        private static bool _logMessage;
        private static bool _logWarning;
        private static bool _logError;
        private static bool LogToDatabase;
        private static string _connectionString;
        private static string _logFileDirectory;
        private static string _currentDate;

        public JobLogger(bool logToFile, bool logToConsole, bool logToDatabase, bool logMessage, bool logWarning, bool logError)
        {
            _logError = logError;
            _logMessage = logMessage;
            _logWarning = logWarning; LogToDatabase = logToDatabase;
            _logToFile = logToFile;
            _logToConsole = logToConsole;
            _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            _logFileDirectory = ConfigurationManager.AppSettings["Log FileDirectory"];
            _currentDate = DateTime.Now.ToShortDateString();
        }

        public static void LogMessage(string message, bool warning, bool error)
        {
            int t = 0;
            string l = "";

            message.Trim();

            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            if (!_logToConsole && !_logToFile && !LogToDatabase)
            {
                throw new Exception("Invalid configuration");
            }
            if ((!_logError && !_logMessage && !_logWarning) || (string.IsNullOrEmpty(message) && !warning && !error))
            {
                throw new Exception("Error or Warning or Message must be specified");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    if (string.IsNullOrEmpty(message) && _logMessage)
                    {
                        t = 1;
                    }
                    if (error && _logError)
                    {
                        t = 2;
                    }
                    if (warning && _logWarning)
                    {
                        t = 3;
                    }
                    SqlCommand command = new SqlCommand("Insert into Log Values('" + message + "', " + t.ToString() + ")");
                    command.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw new Exception("Something went wrong in database");
                }
                finally
                {
                    connection.Close();
                }
            }
            
            if (!File.Exists(_logFileDirectory + "LogFile" + _currentDate + ".txt"))
            {
                l = File.ReadAllText(_currentDate + "LogFile" + _currentDate + ".txt");
            }

            switch (t)
            {
                case 1:
                    l = l + _currentDate + message;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 2:
                    l = l + _currentDate + message;
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case 3:
                    l = l + _currentDate + message;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    break;
            }

            File.WriteAllText(_logFileDirectory + "LogFile" + _currentDate + ".txt", l);

            Console.WriteLine(_currentDate + message);
        }

        public enum MessageType
        {
            MESSAGE,
            WARNING,
            ERROR
        }
    }

}

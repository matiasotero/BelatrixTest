namespace TestBelatrix
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text;

    public class JobLogger
    {
        private static bool _logToFile;
        private static bool _logToConsole;
        private static bool LogToDatabase;

        public JobLogger(bool logToFile, bool logToConsole, bool logToDatabase)
        {
            LogToDatabase = logToDatabase;
            _logToFile = logToFile;
            _logToConsole = logToConsole;
        }

        public void LogMessage(string message, MessageType messageType)
        {
            string l = "";
            string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            string _logFileDirectory = "";
#if DEBUG
            _logFileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
#else
            _logFileDirectory = ConfigurationManager.AppSettings["Log FileDirectory"];
#endif
            string _currentDate = DateTime.Now.ToString("dd-MM-yyyy");

            message.Trim();

            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            if (!_logToConsole && !_logToFile && !LogToDatabase)
            {
                throw new Exception("Invalid configuration");
            }
          
            if (LogToDatabase)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    try
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand("Insert into Log Values('" + message + "', " + (int)messageType + ")");
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        throw new Exception("Something went wrong in the database.");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            if (_logToFile && File.Exists(Path.Combine(_logFileDirectory, "LogFile" + _currentDate + ".txt")))
            {
                l = File.ReadAllText(Path.Combine(_logFileDirectory, "LogFile" + _currentDate + ".txt"));
            }

            switch (messageType)
            {
                case MessageType.MESSAGE:
                    l = l + _currentDate + message;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case MessageType.ERROR:
                    l = l + _currentDate + message;
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case MessageType.WARNING:
                    l = l + _currentDate + message;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    break;
            }

            if (_logToFile)
            {
                File.WriteAllText(Path.Combine(_logFileDirectory, "LogFile" + _currentDate + ".txt"), l + " || ", Encoding.UTF8);
            }

            Console.WriteLine(_currentDate + message);
        }

        public enum MessageType
        {
            MESSAGE = 1,
            ERROR = 2,
            WARNING = 3
        }
    }

}

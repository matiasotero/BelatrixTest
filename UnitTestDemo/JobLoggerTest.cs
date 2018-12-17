using NUnit.Framework;
using TestBelatrix;

namespace UnitTestDemo
{
    [TestFixture]
    public class JobLoggerTest
    {
        private JobLogger _jobLogger = null;

        [TestCase]
        public void LogMessage_SetDatabaseConfigurationConnectionStringWrong_ResultThrowAnException()
        {
            try
            {
                _jobLogger = new JobLogger(logToFile: false, logToConsole: false, logToDatabase: true);
                _jobLogger.LogMessage("test", JobLogger.MessageType.ERROR);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestCase]
        public void LogMessage_SetFileConfiguration_ResultOk()
        {
            try
            {
                _jobLogger = new JobLogger(logToFile: true, logToConsole: false, logToDatabase: false);
                _jobLogger.LogMessage("test", JobLogger.MessageType.ERROR);
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [TestCase]
        public void LogMessage_SetOnlyConsole_ResultOk()
        {
            try
            {
                _jobLogger = new JobLogger(logToFile: false, logToConsole: true, logToDatabase: false);
                _jobLogger.LogMessage("test", JobLogger.MessageType.ERROR);
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [TestCase]
        public void LogMessage_SetInvalidConfiguration_ResultException()
        {
            try
            {
                _jobLogger = new JobLogger(logToFile: false, logToConsole: false, logToDatabase: false);
                _jobLogger.LogMessage("test", JobLogger.MessageType.ERROR);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }
    }
}

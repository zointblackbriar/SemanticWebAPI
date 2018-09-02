//CodeProject
//The Microsoft Public License(Ms-PL)
//Toby Patke

using System;
using System.IO;
using System.Text;
using System.Threading;

//Asynchronous logging library

namespace SemanticAPI
{
    public sealed class LogBase
    {
        //Log File Writing 
        public static bool Listening { get; private set; }

        public static FileInfo TargetLogFile { get; private set; }

        public static DirectoryInfo TargetDirectory { get { return TargetLogFile?.Directory; } }

        public static bool LogToConsole = false;

        public static int BatchInterval = 1000;

        public static bool IgnoreDebug = false;

        private static readonly Timer Timer = new Timer(Tick); //Setup a timer

        private static readonly StringBuilder LogQueue = new StringBuilder();

        public static void Start(FileInfo targetLogFile)
        {
            if (Listening)
                return;
            Listening = true;
            TargetLogFile = targetLogFile;
            VerifyTargetDirectory();

            Timer.Change(BatchInterval, Timeout.Infinite); //The tick event that is reset every time
        }
        //Directory found function
        private static void VerifyTargetDirectory()
        {
            if (TargetDirectory == null)
            {
                throw new DirectoryNotFoundException("Directory not found");
            }

            TargetDirectory.Refresh();
            if (!TargetDirectory.Exists)
                TargetDirectory.Create();
        }
        #region Log Functions
        public readonly string Name;
        public EventHandler<LogMessageInfo> LogMessageAdd;
        private bool _startedErrorShown = false;

        public const string DEBUG = "DEBUG";
        public const string INFO = "INFO";
        public const string WARN = "WARN";
        public const string ERROR = "ERROR";

        public LogBase(string Name)
        {
            this.Name = Name;
        }

        public LogBase(Type t) : this(t.Name)
        {

        }

        public void Debug(string message)
        {
            if (IgnoreDebug)
                return;

            Log(DEBUG, message);
        }

        public void Info(string message)
        {
            Log(INFO, message);
        }

        public void Log(string level, string message, Exception exception = null)
        {
            if (!CheckListening())
                return;

            if (exception != null)
                message += string.Format("\r\n{0}\r\n{1}", exception.Message, exception.StackTrace);

            var info = new LogMessageInfo(level, Name, message);
            var messageInfo = info.ToString();

            lock (LogQueue)
            {
                LogQueue.AppendLine(messageInfo);
            }

            var eventAdded = LogMessageAdd;
            if (eventAdded != null)
                eventAdded.Invoke(this, info); //Block Caller
        }

        private bool CheckListening()
        {
            if (Listening)
                return true;

            if (!_startedErrorShown)
            {
                Console.WriteLine("Logging is not started");
                _startedErrorShown = true;
            }

            return false;
        }

        #endregion

        #region Event Args with LogMessageInfo
        //Event Handler
        public sealed class LogMessageInfo : EventArgs
        {
            public readonly DateTime Timestamp;
            public readonly string ThreadId;
            public readonly string Logger;
            public readonly string Message;
            public readonly string Level;

            public bool IsError { get { return SemanticAPI.LogBase.ERROR.Equals(this.Level, StringComparison.Ordinal); } }

            public bool IsWarning { get { return SemanticAPI.LogBase.WARN.Equals(Level, StringComparison.Ordinal); } }

            public bool IsInformation { get { return SemanticAPI.LogBase.INFO.Equals(Level, StringComparison.Ordinal); } }

            public bool IsDebug { get { return SemanticAPI.LogBase.DEBUG.Equals(Level, StringComparison.Ordinal); } }

            //Constructor
            public LogMessageInfo(string Level, string logger, string message)
            {
                this.Timestamp = DateTime.UtcNow; //Current Time
                var thread = Thread.CurrentThread;
                this.ThreadId = string.IsNullOrEmpty(thread.Name) ? thread.ManagedThreadId.ToString() : thread.Name;
                this.Level = Level;
                this.Logger = logger;
                this.Message = message;
            }

            public override string ToString()
            {
                return string.Format("{0:yyyy/MM/dd HH:mm:ss.fff} {1} {2} {3} {4}",
                    Timestamp, ThreadId, Logger, Level, Message);
            }

        }
        #endregion

        #region Ticker Function
        //Event Handler for Time Ticking
        private static void Tick(object state)
        {
            try
            {
                var logMessage = "";
                //synchronize
                lock (LogQueue)
                {
                    logMessage = LogQueue.ToString();
                    LogQueue.Length = 0;
                }

                if (string.IsNullOrEmpty(logMessage))
                    return;

                if (LogToConsole)
                    Console.Write(logMessage);

                VerifyTargetDirectory();   //File may be deleted after initialization
                File.AppendAllText(TargetLogFile.FullName, logMessage);

            }
            finally
            {
                if (Listening)
                {
                    //Reset timer
                    Timer.Change(BatchInterval, Timeout.Infinite);
                }
            }
        }


        public static void StartTickin(FileInfo targetLogFile)
        {
            //If Listening is false
            if (Listening != false)
                return;

            Listening = true;
            TargetLogFile = targetLogFile;
            VerifyTargetDirectory();

            Timer.Change(BatchInterval, Timeout.Infinite);
        }

        public static void ShutDown()
        {
            Console.WriteLine("Listening in Shutdown: " + Listening);
            if (!Listening)
                return;

            Listening = false;
            Timer.Dispose();
            Tick(null);
        }
        #endregion

        #region Test Code
        //private static void RunLog()
        //{
        //    var log = new LogBase(typeof(Program)); // Create logger with class name.
        //    var tasks = new List<Task>();

        //    for (int i = 1; i <= 10; i++)
        //    {
        //        var threadId = i;
        //        log.Info("Starting thread: " + threadId);
        //        tasks.Add(Task.Factory.StartNew(() => LogMessages(threadId)));
        //    }

        //    Task.WaitAll(tasks.ToArray());
        //}

        //private static void LogMessages(int threadId)
        //{
        //    var random = new Random();
        //    var log = new LogBase("Thread_" + threadId); // Create logger from string.

        //    for (int i = 1; i < 100; i++)
        //    {
        //        log.Info("This is log message " + i);
        //        Thread.Sleep(random.Next(10, 100)); // sleep to simulate a more realistic execution.
        //    }
        //}
        #endregion
    }
}

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace SemanticAPI.Helpers
{
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly string _log4NetConfigFile;
        private readonly ConcurrentDictionary<string, Log4NetLogger> _loggersConcurrent =
            new ConcurrentDictionary<string, Log4NetLogger>();

        public Log4NetProvider(string log4NetConfigFile)
        {
            _log4NetConfigFile = log4NetConfigFile;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggersConcurrent.GetOrAdd(categoryName, CreateLoggerImplementation);
        }
        private Log4NetLogger CreateLoggerImplementation(string name)
        {
            return new Log4NetLogger(name, ParseLog4NetConfigFile(_log4NetConfigFile));
        }

        private XmlElement ParseLog4NetConfigFile(string fileName)
        {
            XmlDocument log4NetConfig = new XmlDocument();
            log4NetConfig.Load(File.OpenRead(fileName));
            return log4NetConfig["log4net"];
        }

        public void Dispose()
        {
            _loggersConcurrent.Clear();
            if(_loggersConcurrent != null)
                throw new Exception("Something is wrong after clearing ");
        }
    }
}

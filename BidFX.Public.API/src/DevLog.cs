using System;
using System.IO;
using log4net;
using log4net.Core;
using log4net.Repository;

namespace BidFX.Public.API
{
    public class DevLog : ILog
    {
        private static readonly StreamWriter Writer = new StreamWriter("D:/BIDFX_API_LOG.log");
        private readonly string _type;

        public static DevLog CreateLogger(Type type)
        {
            return new DevLog(type.ToString());
        }

        public DevLog(string type)
        {
            _type = type;
        }
        
        private static void Write(string message)
        {
            Writer.WriteLine(message);
            Writer.Flush();
        }

        public ILogger Logger
        {
            get { return null; }
        }

        public void Debug(object message)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[DEBUG][" + _type + "]: " + message);
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void Debug(object message, Exception exception)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[DEBUG][" + _type + "]: " + message + "\n" + exception.Message);
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[DEBUG][" + _type + "]: " + string.Format(format, args));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void DebugFormat(string format, object arg0)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[DEBUG][" + _type + "]: " + string.Format(format, arg0));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[DEBUG][" + _type + "]: " + string.Format(format, arg0, arg1));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[DEBUG][" + _type + "]: " + string.Format(format, arg0, arg1, arg2));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[DEBUG][" + _type + "]: " + string.Format(provider, format, args));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void Info(object message)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[INFO][" + _type + "]: " + message);
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void Info(object message, Exception exception)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[INFO][" + _type + "]: " + exception.Message);
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[INFO][" + _type + "]: " + string.Format(format, args));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void InfoFormat(string format, object arg0)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[INFO][" + _type + "]: " + string.Format(format, arg0));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[INFO][" + _type + "]: " + string.Format(format, arg0, arg1));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[INFO][" + _type + "]: " + string.Format(format, arg0, arg1, arg2));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[INFO][" + _type + "]: " + string.Format(provider, format, args));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void Warn(object message)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[WARN][" + _type + "]: " + message);
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void Warn(object message, Exception exception)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[WARN][" + _type + "]: " + exception.Message);
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[WARN][" + _type + "]: " + string.Format(format, args));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void WarnFormat(string format, object arg0)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[WARN][" + _type + "]: " + string.Format(format, arg0));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[WARN][" + _type + "]: " + string.Format(format, arg0, arg1));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[WARN][" + _type + "]: " + string.Format(format, arg0, arg1, arg2));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[WARN][" + _type + "]: " + string.Format(provider, format, args));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void Error(object message)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[ERROR][" + _type + "]: " + message);
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void Error(object message, Exception exception)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[ERROR][" + _type + "]: " + exception.Message);
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[ERROR][" + _type + "]: " + string.Format(format, args));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void ErrorFormat(string format, object arg0)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[ERROR][" + _type + "]: " + string.Format(format, arg0));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[ERROR][" + _type + "]: " + string.Format(format, arg0, arg1));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[ERROR][" + _type + "]: " + string.Format(format, arg0, arg1, arg2));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[ERROR][" + _type + "]: " + string.Format(provider, format, args));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void Fatal(object message)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[FATAL][" + _type + "]: " + message);
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void Fatal(object message, Exception exception)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[FATAL][" + _type + "]: " + exception.Message);
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[FATAL][" + _type + "]: " + string.Format(format, args));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void FatalFormat(string format, object arg0)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[FATAL][" + _type + "]: " + string.Format(format, arg0));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[FATAL][" + _type + "]: " + string.Format(format, arg0, arg1));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[FATAL][" + _type + "]: " + string.Format(format, arg0, arg1, arg2));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                Write(GetTimestamp(DateTime.Now) + "[FATAL][" + _type + "]: " + string.Format(provider, format, args));
            }
            catch (Exception e)
            {
                Write("Error logging message: " + e.Message);
            }
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled {
            get { return true; }
        }
        public bool IsWarnEnabled
        {
            get { return true; }
        }
        public bool IsErrorEnabled
        {
            get { return true; }
        }
        public bool IsFatalEnabled
        {
            get { return true; }
        }

        public string Name
        {
            get { return ""; }
        }
        public ILoggerRepository Repository
        {
            get { return null; }
        }
        
        private static string GetTimestamp(DateTime value) {
            return value.ToString("[yyyy-MM-dd HH:mm:ss.ffffZ]");
        }
    }
}
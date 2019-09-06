using System;
using System.IO;
using System.Web;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    /// <summary>
    /// 日志操作管理类
    /// </summary>
    public class Logger
    {
        #region 字段

        private ILog _log;

        #endregion

        #region 构造函数

        static Logger()
        {
            const string filename = "log4net.config";
            string basePath = HttpContext.Current != null
                ? AppDomain.CurrentDomain.SetupInformation.PrivateBinPath
                : AppDomain.CurrentDomain.BaseDirectory;
            string configFile = Path.Combine(basePath, filename);
            if (File.Exists(configFile))
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo(configFile));
                return;
            }

            //默认设置
            RollingFileAppender appender = new RollingFileAppender
            {
                Name = "root",
                File = "logs\\log",
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Composite,
                DatePattern = "yyyyMMdd\".log\"",
                MaxSizeRollBackups = 10
            };

            PatternLayout layout = new PatternLayout("[%d{yyyy-MM-dd HH:mm:ss.fff}] %c.%M %t %n%m%n");
            appender.Layout = layout;
            BasicConfigurator.Configure(appender);
            appender.ActivateOptions();
        }

        #endregion

        #region 公共方法

        /// <summary>
        ///     获取指定名称的日志实例
        /// </summary>
        public static Logger GetLogger(string name)
        {
            return new Logger { _log = LogManager.GetLogger(name) };
        }

        /// <summary>
        ///     获取指定类型的日志实例
        /// </summary>
        public static Logger GetLogger(Type type)
        {
            return new Logger { _log = LogManager.GetLogger(type) };
        }

        public static Logger GetCurMethodLog(int up = 0)
        {
            if(up <= 0)
            {
                return new Logger { _log = LogManager.GetLogger(MethodInfo.GetCurrentMethod().DeclaringType) };
            }
            else
            {
                return new Logger { _log = LogManager.GetLogger(new StackTrace().GetFrame(up).GetMethod().DeclaringType) };
            }            
        }

        public static string GetPosition()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);            
            return "Position：" + sf.GetMethod().DeclaringType.Name + "[" + sf.GetMethod().Name + "]\r\n";
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Debug" />级别的日志
        /// </summary>
        public void Debug(object message, Exception exception = null)
        {
            if (exception == null)
            {
                _log.Debug(message);
            }
            else
            {
                _log.Debug(message, exception);
            }
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Debug" />级别的日志
        /// </summary>
        public void DebugAsync(object message, Exception exception = null)
        {
            if (exception == null)
            {
                _log.Debug(message);
            }
            else
            {
                _log.Debug(message, exception);
            }
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Debug" />级别的日志
        /// </summary>
        public void DebugFormat(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Debug" />级别的日志
        /// </summary>
        public void DebugFormatAsync(string format, params object[] args)
        {
            Task.Factory.StartNew(() => { _log.DebugFormat(format, args); });
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Info" />级别的日志
        /// </summary>
        public void Info(object message, Exception exception = null)
        {
            if (exception == null)
            {
                _log.Info(message);
            }
            else
            {
                _log.Info(message, exception);
            }
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Info" />级别的日志
        /// </summary>
        public void InfoAsync(object message, Exception exception = null)
        {
            Task.Factory.StartNew(() => { _log.Info(message, exception); });
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Info" />级别的日志
        /// </summary>
        public void InfoFormat(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Info" />级别的日志
        /// </summary>
        public void InfoFormatAsync(string format, params object[] args)
        {
            Task.Factory.StartNew(() => { _log.InfoFormat(format, args); });            
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Warn" />级别的日志
        /// </summary>
        public void Warn(object message, Exception exception = null)
        {
            if (exception == null)
            {
                _log.Warn(message);
            }
            else
            {
                _log.Warn(message, exception);
            }
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Warn" />级别的日志
        /// </summary>
        public void WarnAsync(object message, Exception exception = null)
        {
            Task.Factory.StartNew(() => { _log.Warn(message, exception); });  
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Warn" />级别的日志
        /// </summary>
        public void WarnFormat(string format, params object[] args)
        {
            _log.WarnFormat(format, args);
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Warn" />级别的日志
        /// </summary>
        public void WarnFormatAsync(string format, params object[] args)
        {
            Task.Factory.StartNew(() => { _log.WarnFormat(format, args); });            
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Error" />级别的日志
        /// </summary>
        public void Error(object message, Exception exception = null)
        {
            if (exception == null)
            {
                _log.Error(message);
            }
            else
            {
                _log.Error(message, exception);
            }
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Error" />级别的日志
        /// </summary>
        public void ErrorAsync(object message, Exception exception = null)
        {
            Task.Factory.StartNew(() => { _log.Error(message, exception); });    
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Error" />级别的日志
        /// </summary>
        public void ErrorFormat(string format, params object[] args)
        {
            _log.ErrorFormat(format, args);
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Error" />级别的日志
        /// </summary>
        public void ErrorFormatAsync(string format, params object[] args)
        {
            Task.Factory.StartNew(() => { _log.ErrorFormat(format, args); });
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Fatal" />级别的日志
        /// </summary>
        public void Fatal(object message, Exception exception = null)
        {
            if (exception == null)
            {
                _log.Fatal(message);
            }
            else
            {
                _log.Fatal(message, exception);
            }
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Fatal" />级别的日志
        /// </summary>
        public void FatalAsync(object message, Exception exception = null)
        {
            Task.Factory.StartNew(() => { _log.Fatal(message, exception); });
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Fatal" />级别的日志
        /// </summary>
        public void FatalFormat(string format, params object[] args)
        {
            _log.FatalFormat(format, args);
        }

        /// <summary>
        ///     输出<see cref="LogLevel.Fatal" />级别的日志
        /// </summary>
        public void FatalFormatAsync(string format, params object[] args)
        {
            Task.Factory.StartNew(() => { _log.FatalFormat(format, args); });            
        }

        /// <summary>
        ///     输出指定<see cref="LogLevel" />级别的日志
        /// </summary>
        public void Write(EnumLogLevel logLevel, object message, Exception exception = null)
        {
            switch (logLevel)
            {
                case EnumLogLevel.Debug:
                    Debug(GetPosition() + message, exception);
                    break;
                case EnumLogLevel.Info:
                    Info(GetPosition() + message, exception);
                    break;
                case EnumLogLevel.Warn:
                    Warn(GetPosition() + message, exception);
                    break;
                case EnumLogLevel.Error:
                    Error(GetPosition() + message, exception);
                    break;
                case EnumLogLevel.Fatal:
                    Fatal(GetPosition() + message, exception);
                    break;
            }
        }

        /// <summary>
        ///     输出指定<see cref="LogLevel" />级别的日志
        /// </summary>
        public void WriteAsync(EnumLogLevel logLevel, object message, Exception exception = null)
        {
            switch (logLevel)
            {
                case EnumLogLevel.Debug:
                    DebugAsync(GetPosition() + message, exception);
                    break;
                case EnumLogLevel.Info:
                    InfoAsync(GetPosition() + message, exception);
                    break;
                case EnumLogLevel.Warn:
                    WarnAsync(GetPosition() + message, exception);
                    break;
                case EnumLogLevel.Error:
                    ErrorAsync(GetPosition() + message, exception);
                    break;
                case EnumLogLevel.Fatal:
                    FatalAsync(GetPosition() + message, exception);
                    break;
            }
        }

        /// <summary>
        ///     输出指定<see cref="LogLevel" />级别的日志
        /// </summary>
        public void WriteFormat(EnumLogLevel logLevel, string format, params object[] args)
        {
            switch (logLevel)
            {
                case EnumLogLevel.Debug:
                    DebugFormat(format, args);
                    break;
                case EnumLogLevel.Info:
                    InfoFormat(format, args);
                    break;
                case EnumLogLevel.Warn:
                    WarnFormat(format, args);
                    break;
                case EnumLogLevel.Error:
                    ErrorFormat(format, args);
                    break;
                case EnumLogLevel.Fatal:
                    FatalFormat(format, args);
                    break;
            }
        }

        /// <summary>
        ///     输出指定<see cref="LogLevel" />级别的日志
        /// </summary>
        public void WriteFormatAsync(EnumLogLevel logLevel, string format, params object[] args)
        {
            switch (logLevel)
            {
                case EnumLogLevel.Debug:
                    DebugFormatAsync(format, args);
                    break;
                case EnumLogLevel.Info:
                    InfoFormatAsync(format, args);
                    break;
                case EnumLogLevel.Warn:
                    WarnFormatAsync(format, args);
                    break;
                case EnumLogLevel.Error:
                    ErrorFormatAsync(format, args);
                    break;
                case EnumLogLevel.Fatal:
                    FatalFormatAsync(format, args);
                    break;
            }
        }

        #endregion
    }
}
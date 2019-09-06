using log4net.Core;
using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    internal sealed class ContentPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, LoggingEvent loggingEvent)
        {
            var messageLog = loggingEvent.MessageObject as MessageLog;
            if (messageLog != null)
            {
                writer.Write(messageLog.Content);
            }
        }
    }
    internal sealed class NamePatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, LoggingEvent loggingEvent)
        {
            var messageLog = loggingEvent.MessageObject as MessageLog;
            if (messageLog != null)
            {
                writer.Write(messageLog.Name);
            }
        }
    }

    internal sealed class ParamPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, LoggingEvent loggingEvent)
        {
            var messageLog = loggingEvent.MessageObject as MessageLog;
            if (messageLog != null)
            {
                writer.Write(messageLog.Param);
            }
        }
    }

    internal sealed class ResultPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, LoggingEvent loggingEvent)
        {
            var messageLog = loggingEvent.MessageObject as MessageLog;
            if (messageLog != null)
            {
                writer.Write(messageLog.Result);
            }
        }
    }
    
}

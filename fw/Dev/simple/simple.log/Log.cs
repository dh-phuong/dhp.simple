using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace simple.log
{
    /// <summary>
    /// Log Type
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// The bug
        /// </summary>
        BUG = 0,

        /// <summary>
        /// The debug
        /// </summary>
        DEBUG = 1,

        /// <summary>
        /// The warning
        /// </summary>
        WARNING = 2,

        /// <summary>
        /// The information
        /// </summary>
        INFO = 3,
    }

    /// <summary>
    /// Log interface
    /// </summary>
    public interface ILog
    {
        void Write(Exception ex);

        void Write(LogType type, string message);

        void Write(string tag, string message);
    }

    /// <summary>
    /// Log Util
    /// </summary>
    public sealed class Log : ILog, IDisposable
    {
        private static ILog _owner;

        /// <summary>
        /// Gets instance of LogUtil
        /// </summary>
        /// <value>
        /// LogUtil
        /// </value>
        public static ILog Me
        {
            get
            {
                if (_owner == null)
                {
                    _owner = new Log();
                }
                return _owner;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Log"/> class from being created.
        /// </summary>
        private Log()
        {
        }

        #region Private variable

        private const string DT_FORMAT_L = "yyyy/MM/dd HH:mm:ss";
        private const string DT_FORMAT_F = "yyyyMMdd HHmmss";
        private const int BITE_SIZE = 1024;
        private const int MB = 2;
        private const int GB = 3;
        private const int TB = 4;

        #endregion Private variable

        #region Log
        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="ex">The ex.</param>
        void ILog.Write(Exception ex)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder("Message:");
            str.Append(ex.Message);
            str.AppendLine("Source:");
            str.Append(ex.Source);
            str.AppendLine("StackTrace:");
            str.Append(ex.StackTrace);

            if (ex.InnerException != null)
            {
                str.AppendLine("InnerException:");
                str.Append(ex.InnerException.Message);
            }
            Me.Write(LogType.BUG, str.ToString());
        }

        /// <summary>
        /// Writes the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        void ILog.Write(LogType type, string message)
        {
            string tag = string.Format("[{0}]", type.ToString());
            Me.Write(tag, message);
        }

        /// <summary>
        /// Writes the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        void ILog.Write(string tag, string message)
        {
            FileInfo _fileInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Log\log.txt"));
            if (!_fileInfo.Directory.Exists)
            {
                _fileInfo.Directory.Create();
            }
            System.IO.StreamWriter sw = new System.IO.StreamWriter(_fileInfo.FullName, true);
            string timeL = System.DateTime.Now.ToString(DT_FORMAT_L);
            string timeF = System.DateTime.Now.ToString(DT_FORMAT_F);

            if (_fileInfo.Exists)
            {
                if (_fileInfo.Length > (Math.Pow(BITE_SIZE, MB)))
                {
                    //Close StreamWriter
                    sw.Close();
                    //Backup file
                    _fileInfo.CopyTo(string.Format(@"Log\{0}.log", timeF));
                    //Delete
                    _fileInfo.Delete();
                    sw = new System.IO.StreamWriter(Path.Combine(_fileInfo.DirectoryName, @"log.txt"), false);
                }
            }

            #region write

            StringBuilder log = new StringBuilder();
            log.AppendLine(string.Format("{0} - {1}", timeL, tag));
            log.AppendLine("--------------------------------");
            log.AppendLine(message);
            log.AppendLine("--------------------------------");
            sw.WriteLine(log);
#if DEBUG
            Debug.WriteLine(log);
#endif
            sw.Close();

            #endregion write
        }
        #endregion

        #region IDisposable メンバー

        public void Dispose()
        {
        }

        #endregion IDisposable メンバー
    }
}

namespace LogTest
{
    using System;
    using System.Text;

    /// <summary>
    /// This is the object that the diff. loggers (filelogger, consolelogger etc.) will operate on. 
    /// The BuildFormattedLine() method will be called to get the formatted line text to be logged - both timestamp and text
    /// </summary>
    public class LogLine
    {
        public LogLine()
        {
            this.Text = "";
        }

        /// <summary>
        /// Return the formatted text of the log line - both timestamp and text
        /// </summary>
        public virtual string BuildFormattedLine()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (this.Text.Length > 0)
            {
                stringBuilder.Append(this.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                stringBuilder.Append("\t");
                stringBuilder.Append(this.Text);
                stringBuilder.Append(". \t");
                stringBuilder.Append(Environment.NewLine);

                return stringBuilder.ToString();
            }
            else
                return this.Text;
        }

        /// <summary>
        /// The text to be display in logline
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The Timestamp is initialized when the log is added
        /// </summary>
        public virtual DateTime Timestamp { get; set; }
    }
}
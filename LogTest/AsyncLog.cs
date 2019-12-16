namespace LogTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    public class AsyncLog : ILog
    {
        private Thread _runThread;
        private List<LogLine> _lines = new List<LogLine>();

        private StreamWriter _writer;

        private bool _exit = false;
        private bool _stopWithFlush = false;

        private string _directoryPath;

        private DateTime _curDate = DateTime.Now;

        public AsyncLog(string directoryPath)
        {
            this._directoryPath = directoryPath;

            this._runThread = new Thread(this.MainLoop);
            this._runThread.Start();
        }

        /// <summary>
        /// This method loops through the list of log lines and writes each formatted log line to the file; acts as a consumer in a producer-consumer scenario
        /// If _exit is set to true, the loop stops without writing any pending log lines
        /// If _stopWithFlush is set to true, the loop continues to write the remaining lines; when there are no lines left, _exit is set to true and the loop stops
        /// </summary>
        private void MainLoop()
        {
            InitializeDirectory();

            InitializeNewFile();

            while (!this._exit)
            {
                if (this._lines.Count > 0)
                {
                    List<LogLine> _handled = new List<LogLine>();
                    try
                    {
                        foreach (LogLine logLine in this._lines)
                        {
                            if (!this._exit || this._stopWithFlush)
                            {
                                if ((DateTime.Now - _curDate).Days != 0)
                                {
                                    _curDate = DateTime.Now;
                                    InitializeNewFile();
                                }

                                this._writer.Write(logLine.BuildFormattedLine());

                                _handled.Add(logLine);
                            }
                        }

                        for (int y = 0; y < _handled.Count; y++)
                        {
                            this._lines.Remove(_handled[y]);
                        }

                        if (this._stopWithFlush == true && this._lines.Count == 0)
                            this._exit = true;

                        Thread.Sleep(50);
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine("Concurrency issue with the list of lines to be logged: " + e.Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("There was an error during execution: " + e.Message);
                    }
                }
            }
        }

        public void StopWithoutFlush()
        {
            this._exit = true;
        }

        public void StopWithFlush()
        {
            this._stopWithFlush = true;
        }

        /// <summary>
        /// Adds the log lines to a list so that the calling application can continue its work while another thread will write the logs to a file
        /// Acts as a producer in a producer-consumer scenario
        /// </summary>
        public void Write(string text)
        {
            this._lines.Add(new LogLine() { Text = text, Timestamp = DateTime.Now });
        }

        /// <summary>
        /// Creates the directory to store the log files if it does not already exist
        /// </summary>
        private void InitializeDirectory()
        {
            if (!Directory.Exists(this._directoryPath))
                Directory.CreateDirectory(this._directoryPath);
        }

        /// <summary>
        /// Creates a new file and initializes it with the column headers text on the first row
        /// </summary>
        private void InitializeNewFile()
        {
            this._writer = File.AppendText(this._directoryPath + @"\Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log");
            this._writer.Write("Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine);
            this._writer.AutoFlush = true;
        }
    }
}
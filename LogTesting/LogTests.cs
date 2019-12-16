using LogTest;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace LogTesting
{
    public class LogTests
    {
        /// <summary>
        /// Test that a call to Ilog will end up in writing something
        /// </summary>
        [Fact]
        public void LogsGetWritten()
        {
            // Arrange
            ILog logger = new AsyncLog(@"C:\LogTesting");
            DirectoryInfo directory = new DirectoryInfo(@"C:\LogTesting");
            string line;

            // Act
            logger.Write("A call to Ilog does indeed end up writing something");
            logger.StopWithFlush();

            using (StreamReader reader = new StreamReader(@"C:\LogTesting\" + directory.GetFiles().OrderByDescending(file => file.LastWriteTime).First().Name))
            {
                line = reader.ReadToEnd();
            }

            // Assert
            Assert.Contains("A call to Ilog does indeed end up writing something", line);
        }

        /// <summary>
        /// Test that new files are created if midnight is crossed
        /// </summary>
        [Fact]
        public void NewFileAtMidnight()
        {
            // Arrange
            ILog logger = new AsyncLog(@"C:\LogTesting");
            DirectoryInfo directory = new DirectoryInfo(@"C:\LogTesting");
            string line;

            // Access and overwrite the logger current date to same day at 23:59:50

            // Act
            for (int i = 50; i > 0; i--)
            {
                logger.Write("Number with flush: " + i.ToString());
                Thread.Sleep(20);
            }

            logger.StopWithFlush();

            using (StreamReader reader = new StreamReader(@"C:\LogTesting\" + directory.GetFiles().OrderByDescending(file => file.LastWriteTime).First().Name))
            {
                line = reader.ReadToEnd();
            }

            // Assert
            Assert.DoesNotContain("Number with No flush: 50.", line);
        }

        /// <summary>
        /// Test that the stop behavior with flush is working as described
        /// </summary>
        [Fact]
        public void StopWithFlushWorks()
        {
            // Arrange
            ILog logger = new AsyncLog(@"C:\LogTesting");
            DirectoryInfo directory = new DirectoryInfo(@"C:\LogTesting");
            string line;

            // Act
            for (int i = 0; i < 15; i++)
            {
                logger.Write("Number with Flush: " + i.ToString());
                Thread.Sleep(50);
            }

            logger.StopWithFlush();

            using (StreamReader reader = new StreamReader(@"C:\LogTesting\" + directory.GetFiles().OrderByDescending(file => file.LastWriteTime).First().Name))
            {
                line = reader.ReadToEnd();
            }

            // Assert
            Assert.Contains("Number with Flush: 14.", line);
        }


        /// <summary>
        /// Test that the stop behavior without flush is working as described
        /// </summary>
        [Fact]
        public void StopWithoutFlushWorks()
        {
            // Arrange
            ILog logger = new AsyncLog(@"C:\LogTesting");
            DirectoryInfo directory = new DirectoryInfo(@"C:\LogTesting");
            string line;

            // Act
            for (int i = 50; i > 0; i--)
            {
                logger.Write("Number with No flush: " + i.ToString());
                Thread.Sleep(20);
            }

            logger.StopWithoutFlush();

            using (StreamReader reader = new StreamReader(@"C:\LogTesting\" + directory.GetFiles().OrderByDescending(file => file.LastWriteTime).First().Name))
            {
                line = reader.ReadToEnd();
            }

            // Assert
            Assert.DoesNotContain("Number with No flush: 1.", line);
        }
    }
}

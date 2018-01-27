using System;
using System.IO;

namespace HedgeModManager
{
    public static class LogFile
    {
        //Variables/Constants
        public static readonly string LogPath = Path.Combine(Program.StartDirectory, "HedgeModManager.log");

        private static TextWriter logWriter;
        private static bool useTimeStamp = true;

        //Methods
        public static void Initialize(bool createNewFile = true, bool useTimeStampBool = true)
        {
            try
            {
                useTimeStamp = useTimeStampBool;
                if (createNewFile)
                    logWriter = File.CreateText(LogPath);
                else
                {
                    logWriter = File.AppendText(LogPath);
                    logWriter.WriteLine($"\r\n======== New Session ========\r\n");
                }
            }
            catch { }

        }

        public static void AddMessage(string message)
        {
            if (logWriter == null) return;
            string logMessage = string.Empty;

            if (useTimeStamp) logMessage += DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss]: ");
            logMessage += message;

            logWriter.WriteLine(logMessage);
            logWriter.Flush();
        }

        public static void WriteMessage(string message, bool? useTimeStamp)
        {
            if (logWriter == null) return;
            string logMessage = string.Empty;

            if (useTimeStamp != null ? (bool)useTimeStamp : LogFile.useTimeStamp) logMessage += DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss]: ");
            logMessage += message;

            logWriter.Write(logMessage);
            logWriter.Flush();
        }

        public static void AddEmptyLine()
        {
            if (logWriter == null) return;
            logWriter.WriteLine("");
        }

        public static void Close()
        {
            if (logWriter == null) return;
            logWriter.Close();
        }
    }
}

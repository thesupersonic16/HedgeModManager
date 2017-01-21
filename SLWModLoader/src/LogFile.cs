using System;
using System.IO;

namespace SLWModLoader
{
    public static class LogFile
    {
        //Variables/Constants
        public static readonly string LogPath = Path.Combine(Program.StartDirectory, "SLWModLoader.log");

        private static TextWriter logWriter;
        private static bool useTimeStamp = true;

        //Methods
        public static void Initialize(bool useTimeStampBool = true)
        {
            useTimeStamp = useTimeStampBool;
            logWriter = File.CreateText(LogPath);
        }

        public static void AddMessage(string message)
        {
            if (logWriter == null) return;
            string logMessage = string.Empty;

            if (useTimeStamp) logMessage += DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss: ");
            logMessage += message;

            logWriter.WriteLine(logMessage);
        }

        public static void AddEmptyLine()
        {
            logWriter.WriteLine("");
        }

        public static void Close()
        {
            logWriter.Close();
        }
    }
}

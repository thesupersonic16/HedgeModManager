using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SLWModLoader
{
    public static class LogFile
    {
        public static string LogPath = Path.Combine(Program.StartDirectory, "SLWModLoader.log");

        private static bool useTimeStamp = true;
        private static TextWriter logWriter;

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

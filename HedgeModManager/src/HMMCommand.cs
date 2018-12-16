using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public static class HMMCommand
    {

        public static NamedPipeServerStream Pipe;
        public static bool Running;

        public static void Start()
        {
            Running = true;
            Pipe = new NamedPipeServerStream(Program.ProgramNameShort, PipeDirection.In, 10, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            Pipe.BeginWaitForConnection(OnConnect, Pipe);
        }

        public static void Close()
        {
            Running = false;
            if (Pipe.IsConnected)
                Pipe.Disconnect();
            Pipe.Close();
        }

        public static void SendMessage(string message)
        {
            LogFile.AddMessage(string.Format("[HMMCOMMAND] MESSAGE SEND \"{0}\"", message));
            using (var pipe = new NamedPipeClientStream(".", Program.ProgramNameShort, PipeDirection.Out))
            {
                pipe.Connect(2000);
                var writer = new StreamWriter(pipe);
                writer.WriteLine(message);
                writer.Flush();
            }
        }

        private static void OnConnect(IAsyncResult result)
        {

            using (var pipe = result.AsyncState as NamedPipeServerStream)
            {
                try
                {
                    pipe.EndWaitForConnection(result);

                    var reader = new StreamReader(pipe);

                    string line;
                    while (pipe.IsConnected)
                    {
                        if ((line = reader.ReadLine()) != null)
                            OnMessage(line);
                    }
                }
                catch (Exception)
                {
                }
            }
            Start();
        }

        private static void OnMessage(string message)
        {
            try
            {
                string c = message.Substring(0, message.IndexOf(' '));
                string r = message.Substring(message.IndexOf(' ') + 1);

                if (message.StartsWith("GB "))
                {
                    LogFile.AddMessage("Running GB Installer");
                    var thread = new Thread(new ParameterizedThreadStart(GBCommand));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start(r);
                }
            }
            catch(Exception)
            {

            }
        }

        private static void GBCommand(object link)
        {
            Program.DownloadGameBananaItem(link as string);
            Program.MainWindow?.Invoke(new Action(() =>
            {
                Program.MainWindow?.RefreshModsList();
            }));
        }

    }
}

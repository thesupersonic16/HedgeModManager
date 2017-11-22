using HedgeModManager.Properties;
using SonicAudioLib.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HedgeModManager
{
    public class CPK
    {
        protected uint m_CurrentFileID = 0;
        public int FileCount { get; protected set; }

        public virtual void AddFilesFromDirectory(string directoryPath) { }

        public virtual void AddFile(string rootDirectoryPath, string filePath) { }

        public virtual void Pack(string CPKPath) { }
    }

    public class CPKSAL : CPK
    {

        private CriCpkArchive m_CpkArchive = new CriCpkArchive();
        
        public override void AddFilesFromDirectory(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            uint i = m_CurrentFileID;
            for (; i < m_CurrentFileID + files.Length; ++i)
            {
                string fullFilePath = files[i - m_CurrentFileID].Replace('/', '\\');
                string filePath = fullFilePath.Replace(directoryPath.Replace('/', '\\') + '\\', "");
                var entry = new CriCpkEntry();
                entry.Id = i;
                entry.Name = Path.GetFileName(filePath);
                entry.DirectoryName = Path.GetDirectoryName(filePath);
                entry.FilePath = new FileInfo(fullFilePath);
                m_CpkArchive.Add(entry);
                FileCount++;
            }
            m_CurrentFileID = i;
        }

        public override void AddFile(string rootDirectoryPath, string filePath)
        {
            string CPKFilePath = filePath.Replace(rootDirectoryPath.Replace('\\', '/') + Path.DirectorySeparatorChar.ToString(), "");
            var entry = new CriCpkEntry();
            entry.Id = m_CurrentFileID++;
            entry.Name = Path.GetFileName(CPKFilePath);
            entry.DirectoryName = Path.GetDirectoryName(CPKFilePath);
            entry.FilePath = new FileInfo(filePath);
            m_CpkArchive.Add(entry);
            FileCount++;
        }

        public override void Pack(string CPKPath)
        {
            m_CpkArchive.Mode = CriCpkMode.FileName;
            m_CpkArchive.Align = 16;
            m_CpkArchive.Save(CPKPath);
        }

    }

    public class CPKMakerCRI : CPK
    {

        private CPKMaker m_CpkMaker;
        
        public CPKMakerCRI()
        {
            try
            {
                m_CpkMaker = new CPKMaker(Path.Combine(Program.StartDirectory, "CPKMaker.dll"));
            }
            catch (Exception ex)
            {
                MainForm.AddMessage("Exception thrown while Loading CPKMaker.", ex,
                    Path.Combine(Program.StartDirectory, "CPKMaker.dll"));
            }
        }

        public CPKMakerCRI(string DLLPath)
        {
            try
            {
                m_CpkMaker = new CPKMaker(DLLPath);
            }
            catch (Exception ex)
            {
                MainForm.AddMessage("Exception thrown while Loading CPKMaker.", ex,
                    Path.Combine(Program.StartDirectory, "CPKMaker.dll"));
            }
        }

        public override void AddFilesFromDirectory(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            uint i = m_CurrentFileID;
            for (; i < m_CurrentFileID + files.Length; ++i)
            {
                string fullFilePath = files[i - m_CurrentFileID];
                string filePath = fullFilePath.Replace(directoryPath + Path.DirectorySeparatorChar.ToString(), "");
                m_CpkMaker.AddFile(fullFilePath, filePath, i, false);
                FileCount++;
            }
            m_CurrentFileID = i;
        }

        public override void AddFile(string rootDirectoryPath, string filePath)
        {
            string CPKFilePath = filePath.Replace(rootDirectoryPath + Path.DirectorySeparatorChar.ToString(), "");
            m_CpkMaker.AddFile(CPKFilePath, filePath, m_CurrentFileID++, false);
            FileCount++;
        }

        public override void Pack(string CPKPath)
        {
            m_CpkMaker.SetCPKMode("ModeFilename");
            m_CpkMaker.DataAlign = 16u;
            m_CpkMaker.StartToBuild(CPKPath);

            var status = m_CpkMaker.Execute();
            while ((status = m_CpkMaker.Execute()) != CPKMaker.Status.Complete)
            {
                Thread.Sleep(0);
                if (status == CPKMaker.Status.Error)
                {
                    MainForm.AddMessageToUser("Failed to Build CPK!");
                    throw new Exception("Failed to Build CPK!");
                }
            }
        }

        public class CPKMaker
        {
            public static Type CPKMakerType = null;
            public static Type EnumCpkFileModeType = null;
            public static object CPKMakerObject = null;

            public uint DataAlign
            {
                get
                {
                    return (uint)CPKMakerType.GetProperty("DataAlign").GetValue(CPKMakerObject);
                }
                set
                {
                    CPKMakerType.GetProperty("DataAlign").SetValue(CPKMakerObject, value);
                }
            }

            public CPKMaker(string DLLPath)
            {
                // Load DLL
                if (CPKMakerType == null)
                {
                    var cpkMaker = Assembly.LoadFile(DLLPath);
                    CPKMakerType = cpkMaker.GetType("CriCpkMaker.CpkMaker");
                    EnumCpkFileModeType = cpkMaker.GetType("CriCpkMaker.CpkMaker+EnumCpkFileMode");
                }
                CPKMakerObject = Activator.CreateInstance(CPKMakerType);
            }

            public void BuildCPK(string inDirectory, string filePath)
            {
                object cpkMaker = CPKMakerObject;

                // cpkMaker.CpkFileMode = CriCpkMaker.CpkMaker.EnumCpkFileMode.ModeFilename;
                CPKMakerType.GetProperty("CpkFileMode").SetValue(cpkMaker,
                    EnumCpkFileModeType.GetField("ModeFilename").GetValue(null));

                DataAlign = 16;

                // Add Files
                uint id = 0;
                foreach (string path in Directory.GetFiles(inDirectory, "*", SearchOption.AllDirectories))
                {
                    string localPath = path;
                    string cpkPath = localPath.Replace(inDirectory + @"\", "");

                    AddFile(localPath, cpkPath, id++);
                }

                StartToBuild(filePath);
            }

            public void BuildCPK(string inDirectory)
            {
                string cpkFileName = new DirectoryInfo(inDirectory).Name + ".cpk";
                string filePath = Path.Combine(Directory.GetParent(inDirectory).FullName, cpkFileName);

                BuildCPK(inDirectory, filePath);
            }

            public void ExtractCPK(string outDirectoryPath, string inFilePath)
            {
                AnalyzeCpkFile(inFilePath);
                if (!Directory.Exists(outDirectoryPath))
                    Directory.CreateDirectory(outDirectoryPath);
                StartToExtract(outDirectoryPath);
                WaitForComplete();
            }

            public string ExtractCPK(string inFilePath)
            {
                string directory = Directory.GetParent(inFilePath).FullName +
                    Path.DirectorySeparatorChar +
                    Path.GetFileNameWithoutExtension(inFilePath);
                ExtractCPK(directory, inFilePath);
                return directory;
            }

            public void SetCPKMode(string modeName)
            {
                CPKMakerType.GetProperty("CpkFileMode").SetValue(CPKMakerObject,
                    EnumCpkFileModeType.GetField(modeName).GetValue(null));
            }

            public bool IsCompleted()
            {
                return Execute() == Status.Complete;
            }

            public bool AnalyzeCpkFile(string filePath)
            {
                // cpkMaker.AnalyzeCpkFile(filePath);
                var method = CPKMakerType.GetMethod("AnalyzeCpkFile", new[] { typeof(string) });
                return (bool)method.Invoke(CPKMakerObject, new object[] { filePath });
            }

            public void StartToExtract(string filePath)
            {
                // cpkMaker.StartToExtract(filePath);
                var method = CPKMakerType.GetMethod("StartToExtract", new[] { typeof(string) });
                method.Invoke(CPKMakerObject, new object[] { filePath });
            }

            public void StartToBuild(string filePath)
            {
                // cpkMaker.StartToBuild(filePath);
                var method = CPKMakerType.GetMethod("StartToBuild", new[] { typeof(string) });
                method.Invoke(CPKMakerObject, new object[] { filePath });
            }

            /// <summary>
            /// NOTE: This returns a void not a CFileInfo
            /// </summary>
            public void AddFile(string localFilePath, string cpkFilePath, uint ID)
            {
                // cpkMaker.AddFile(localFilePath, cpkFilePath, ID);
                var method = CPKMakerType.GetMethod("AddFile", new[] { typeof(string), typeof(string), typeof(uint) });
                method.Invoke(CPKMakerObject, new object[] { localFilePath, cpkFilePath, ID });
            }

            /// <summary>
            /// NOTE: This returns a void not a CFileInfo
            /// </summary>
            public void AddFile(string localFilePath, string cpkFilePath, uint ID, bool flag)
            {
                // cpkMaker.AddFile(localFilePath, cpkFilePath, ID, flag);
                var method = CPKMakerType.GetMethod("AddFile", new[] { typeof(string), typeof(string), typeof(uint), typeof(bool) });
                method.Invoke(CPKMakerObject, new object[] { localFilePath, cpkFilePath, ID, flag });
            }

            public bool WaitForComplete()
            {
                // cpkMaker.WaitForComplete();
                var method = CPKMakerType.GetMethod("WaitForComplete");
                return (bool)method.Invoke(CPKMakerObject, new object[0]);
            }

            public Status Execute()
            {
                // cpkMaker.Execute();
                var method = CPKMakerType.GetMethod("Execute");
                return (Status)method.Invoke(CPKMakerObject, new object[0]);
            }

            public double GetProgress()
            {
                // cpkMaker.GetProgress();
                var method = CPKMakerType.GetMethod("GetProgress");
                return (double)method.Invoke(CPKMakerObject, new object[0]);
            }

            public enum Status
            {
                Stop,
                Verified = 202,
                Verifying = 201,
                VerifyPreparing = 200,
                Extracted = 102,
                Extracting = 101,
                ExtractPreparing = 100,
                Complete = 50,
                CpkBuildFinalize = 17,
                MselfRewriting = 16,
                HeaderRewriting = 15,
                EtocBuilding = 14,
                GtocBuilding = 13,
                ItocBuilding = 12,
                TocBuilding = 11,
                PreTocBuilding = 10,
                GtocInfoMaking = 9,
                Error = -1,
                ItocInfoMaking = 8,
                TocInfoMaking = 7,
                FileDataBuildingCopying = 6,
                FileDataBuildingStart = 5,
                FileDataBuildingPrep = 4,
                PreFileDataBuilding = 3,
                HeaderSkipping = 2,
                HeaderMselfSkipping = 1
            }
        }
    }
}
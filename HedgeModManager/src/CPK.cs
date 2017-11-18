using SonicAudioLib.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public class CPK
    {

        private CriCpkArchive m_CpkArchive = new CriCpkArchive();
        private uint m_CurrentFileID = 0;

        public void AddFilesFromDirectory(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            uint i = m_CurrentFileID;
            for (; i < m_CurrentFileID + files.Length; ++i)
            {
                string fullFilePath = files[i - m_CurrentFileID];
                string filePath = fullFilePath.Replace(directoryPath + Path.DirectorySeparatorChar.ToString(), "");
                var entry = new CriCpkEntry();
                entry.Id = i;
                entry.Name = Path.GetFileName(filePath);
                entry.DirectoryName = Path.GetDirectoryName(filePath);
                entry.FilePath = new FileInfo(fullFilePath);
                m_CpkArchive.Add(entry);
            }
            m_CurrentFileID = i;
        }

        public void AddFile(string rootDirectoryPath, string filePath)
        {
            string CPKFilePath = filePath.Replace(rootDirectoryPath + Path.DirectorySeparatorChar.ToString(), "");
            var entry = new CriCpkEntry();
            entry.Id = m_CurrentFileID++;
            entry.Name = Path.GetFileName(CPKFilePath);
            entry.DirectoryName = Path.GetDirectoryName(CPKFilePath);
            entry.FilePath = new FileInfo(filePath);
            m_CpkArchive.Add(entry);
        }

        public void Pack(string CPKPath)
        {
            m_CpkArchive.Mode = CriCpkMode.FileName;
            m_CpkArchive.Save(CPKPath);
        }
    }
}

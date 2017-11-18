using CriCpkMaker;
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

        private CpkMaker m_CpkMaker = new CpkMaker();
        private uint m_CurrentFileID = 0;

        public void AddFilesFromDirectory(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            uint i = m_CurrentFileID;
            for (; i < m_CurrentFileID + files.Length; ++i)
            {
                string fullFilePath = files[i - m_CurrentFileID];
                string filePath = fullFilePath.Replace(directoryPath + Path.DirectorySeparatorChar.ToString(), "");
                m_CpkMaker.AddFile(fullFilePath, filePath, i, false);
            }
            m_CurrentFileID = i;
        }

        public void AddFile(string rootDirectoryPath, string filePath)
        {
            string CPKFilePath = filePath.Replace(rootDirectoryPath + Path.DirectorySeparatorChar.ToString(), "");
            m_CpkMaker.AddFile(CPKFilePath, filePath, m_CurrentFileID++, false);
        }

        public void Pack(string CPKPath)
        {
            m_CpkMaker.CpkFileMode = CpkMaker.EnumCpkFileMode.ModeFilename;
            m_CpkMaker.DataAlign = 16u;
            m_CpkMaker.StartToBuild(CPKPath);
            while (m_CpkMaker.Execute() != Status.Complete)
            {
                Thread.Sleep(100);
            }
        }
    }
}

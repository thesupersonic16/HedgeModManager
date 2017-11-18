using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SonicAudioLib.Archives;
using SonicAudioLib.IO;
using SonicAudioLib.CriMw;
using SonicAudioLib.FileBases;

namespace SonicAudioLib.Archives
{
    public enum CriAaxEntryFlag
    {
        Intro = 0,
        Loop = 1,
    }

    public enum CriAaxArchiveMode
    {
        Adx = 0,
        Dsp = 4,
        Wav = 5,
    }

    public class CriAaxEntry : EntryBase
    {
        public CriAaxEntryFlag Flag { get; set; }

        // DSP
        public byte[] DspHeader { get; set; }
        public uint DspSampleRate { get; set; }
        public uint DspSampleCount { get; set; }
        public byte DspChannelCount { get; set; }
    }

    public class CriAaxArchive : ArchiveBase<CriAaxEntry>
    {
        public CriAaxArchiveMode Mode { get; set; }

        public override void Read(Stream source)
        {
            using (CriTableReader reader = CriTableReader.Create(source))
            {
                switch (reader.TableName)
                {
                    // ADX wrapper
                    case "AAX":
                        Mode = CriAaxArchiveMode.Adx;
                        break;

                    // DSP wrapper
                    case "ADPCM_WII":
                        Mode = CriAaxArchiveMode.Dsp;
                        break;

                    // WAV wrapper
                    case "SWLPCM":
                        Mode = CriAaxArchiveMode.Wav;
                        break;

                    // Unknown
                    default:
                        throw new InvalidDataException($"Unknown AAX type '{reader.TableName}'. Please report the error with the file(s).");
                }

                if (Mode == CriAaxArchiveMode.Dsp)
                {
                    // TODO
                    throw new NotImplementedException("DSP files aren't supported yet.");
                }

                else
                {
                    while (reader.Read())
                    {
                        CriAaxEntry entry = new CriAaxEntry();
                        entry.Flag = (CriAaxEntryFlag)reader.GetByte("lpflg");
                        entry.Position = reader.GetPosition("data");
                        entry.Length = reader.GetLength("data");
                        entries.Add(entry);
                    }
                }
            }
        }

        public override void Write(Stream destination)
        {
            using (CriTableWriter writer = CriTableWriter.Create(destination, CriTableWriterSettings.AdxSettings))
            {
                string tableName = "AAX";

                switch (Mode)
                {
                    case CriAaxArchiveMode.Adx:
                        tableName = "AAX";
                        break;

                    case CriAaxArchiveMode.Dsp:
                        tableName = "ADPCM_WII";
                        break;

                    case CriAaxArchiveMode.Wav:
                        tableName = "SWLPCM";
                        break;
                }

                writer.WriteStartTable(tableName);
                writer.WriteField("data", typeof(byte[]));
                writer.WriteField("lpflg", typeof(byte));

                foreach (CriAaxEntry entry in entries.OrderBy(entry => entry.Flag))
                {
                    writer.WriteRow(true, entry.FilePath, (byte)entry.Flag);
                }

                writer.WriteEndTable();
            }
        }

        public override void Add(CriAaxEntry item)
        {
            if (entries.Count == 2 || entries.Exists(entry => (entry.Flag == item.Flag)))
            {
                return;
            }

            base.Add(item);
        }

        public string GetModeExtension()
        {
            switch (Mode)
            {
                case CriAaxArchiveMode.Adx:
                    return ".adx";

                case CriAaxArchiveMode.Dsp:
                    return ".dsp";

                case CriAaxArchiveMode.Wav:
                    return ".wav";
            }

            return string.Empty;
        }

        public void SetModeFromExtension(string extension)
        {
            switch (Path.GetExtension(extension).ToLower())
            {
                case ".adx":
                    Mode = CriAaxArchiveMode.Adx;
                    break;

                case ".dsp":
                    Mode = CriAaxArchiveMode.Dsp;
                    break;

                case ".wav":
                    Mode = CriAaxArchiveMode.Wav;
                    break;
            }
        }
    }
}

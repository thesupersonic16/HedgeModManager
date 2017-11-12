using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace HedgeModManager
{
    // Most of the code is from mod-loader-common, Sonic Retro
    public class CodeLoader
    {
        public static string CodesXMLPath = Path.Combine(Program.StartDirectory, "mods\\Codes.xml");
        public static string CodesPath = Path.Combine(Program.StartDirectory, "mods\\Codes.dat");
        public static string PatchesPath = Path.Combine(Program.StartDirectory, "mods\\Patches.dat");

        public static List<Code> LoadAllCodes(string codesPath)
        {
            CodeList codeList = null;
            if (!File.Exists(codesPath))
                return new List<Code>();
            using (FileStream filestream = File.OpenRead(codesPath))
                codeList = (CodeList)(new XmlSerializer(typeof(CodeList)).Deserialize(filestream));
            return codeList.Codes;
        }

        public static void SaveCodesAndPatches(ModsDatabase modDB, List<Code> codes)
        {
            var selectedCodes = new List<Code>();
            var selectedPatches = new List<Code>();

            foreach (string item in modDB.GetCodeList())
            {
                var code = codes.FirstOrDefault(t => t.Name == item);
                if (code == null)
                    continue;
                if (code.Patch)
                    selectedPatches.Add(code);
                else
                    selectedCodes.Add(code);
            }

            using (var fileStream = File.Create(PatchesPath))
            using (var writer = new BinaryWriter(fileStream, Encoding.ASCII))
            {
                writer.Write(new[] { 'c', 'o', 'd', 'e', 'v', '5' });
                writer.Write(selectedPatches.Count);
                foreach (var item in selectedPatches)
                {
                    if (item.IsReg)
                        writer.Write((byte)CodeType.newregs);
                    WriteCodes(item.Lines, writer);
                }
                writer.Write(byte.MaxValue);
            }
            using (var fileStream = File.Create(CodesPath))
            using (var writer = new BinaryWriter(fileStream, Encoding.ASCII))
            {
                writer.Write(new[] { 'c', 'o', 'd', 'e', 'v', '5' });
                writer.Write(selectedCodes.Count);
                foreach (var item in selectedCodes)
                {
                    if (item.IsReg)
                        writer.Write((byte)CodeType.newregs);
                    WriteCodes(item.Lines, writer);
                }
                writer.Write(byte.MaxValue);
            }
        }

        public static void SaveCodesAndPatches64(ModsDatabase modDB, List<Code> codes)
        {
            var selectedCodes = new List<Code>();
            var selectedPatches = new List<Code>();

            foreach (string item in modDB.GetCodeList())
            {
                var code = codes.FirstOrDefault(t => t.Name == item);
                if (code == null)
                    continue;
                if (code.Patch)
                    selectedPatches.Add(code);
                else
                    selectedCodes.Add(code);
            }

            using (var fileStream = File.Create(PatchesPath))
            using (var writer = new BinaryWriter(fileStream, Encoding.ASCII))
            {
                writer.Write(new[] { 'c', 'o', 'd', 'e', 'v', '5' });
                writer.Write(selectedPatches.Count);
                foreach (var item in selectedPatches)
                {
                    if (item.IsReg)
                        writer.Write((byte)CodeType.newregs);
                    WriteCodes64(item.Lines, writer);
                }
                writer.Write(byte.MaxValue);
            }
            using (var fileStream = File.Create(CodesPath))
            using (var writer = new BinaryWriter(fileStream, Encoding.ASCII))
            {
                writer.Write(new[] { 'c', 'o', 'd', 'e', 'v', '5' });
                writer.Write(selectedCodes.Count);
                foreach (var item in selectedCodes)
                {
                    if (item.IsReg)
                        writer.Write((byte)CodeType.newregs);
                    WriteCodes64(item.Lines, writer);
                }
                writer.Write(byte.MaxValue);
            }
        }

        public static void WriteCodes(List<CodeLine> codeList, BinaryWriter writer)
        {
            foreach (CodeLine line in codeList)
            {
                writer.Write((byte)line.Type);
                uint address;
                if (line.Address.StartsWith("r"))
                    address = uint.Parse(line.Address.Substring(1), NumberStyles.None, NumberFormatInfo.InvariantInfo);
                else
                    address = uint.Parse(line.Address, NumberStyles.HexNumber);
                if (line.Pointer)
                    address |= 0x80000000u;
                writer.Write(address);
                if (line.Pointer)
                    if (line.Offsets != null)
                    {
                        writer.Write((byte)line.Offsets.Count);
                        foreach (int off in line.Offsets)
                            writer.Write(off);
                    }
                    else
                        writer.Write((byte)0);
                if (line.Type == CodeType.ifkbkey)
                    writer.Write((int)(Keys)Enum.Parse(typeof(Keys), line.Value));
                else
                    switch (line.ValueType)
                    {
                        case ValueType.@decimal:
                            switch (line.Type)
                            {
                                case CodeType.writefloat:
                                case CodeType.addfloat:
                                case CodeType.subfloat:
                                case CodeType.mulfloat:
                                case CodeType.divfloat:
                                case CodeType.ifeqfloat:
                                case CodeType.ifnefloat:
                                case CodeType.ifltfloat:
                                case CodeType.iflteqfloat:
                                case CodeType.ifgtfloat:
                                case CodeType.ifgteqfloat:
                                    writer.Write(float.Parse(line.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo));
                                    break;
                                default:
                                    writer.Write(unchecked((int)long.Parse(line.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo)));
                                    break;
                            }
                            break;
                        case ValueType.hex:
                            writer.Write(uint.Parse(line.Value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
                            break;
                    }
                writer.Write(line.RepeatCount ?? 1);
                if (line.IsIf)
                {
                    WriteCodes(line.TrueLines, writer);
                    if (line.FalseLines.Count > 0)
                    {
                        writer.Write((byte)CodeType.@else);
                        WriteCodes(line.FalseLines, writer);
                    }
                    writer.Write((byte)CodeType.endif);
                }
            }
        }

        public static void WriteCodes64(List<CodeLine> codeList, BinaryWriter writer)
        {
            foreach (CodeLine line in codeList)
            {
                writer.Write((byte)line.Type);
                ulong address;
                if (line.Address.StartsWith("r"))
                    address = ulong.Parse(line.Address.Substring(1), NumberStyles.None, NumberFormatInfo.InvariantInfo);
                else
                    address = ulong.Parse(line.Address, NumberStyles.HexNumber);
                if (line.Pointer)
                    address |= 0x8000000000000000ul;
                writer.Write(address);
                if (line.Pointer)
                    if (line.Offsets != null)
                    {
                        writer.Write((byte)line.Offsets.Count);
                        foreach (int off in line.Offsets)
                            writer.Write(off);
                    }
                    else
                        writer.Write((byte)0);
                if (line.Type == CodeType.ifkbkey)
                    writer.Write((int)(Keys)Enum.Parse(typeof(Keys), line.Value));
                else
                    switch (line.ValueType)
                    {
                        case ValueType.@decimal:
                            switch (line.Type)
                            {
                                case CodeType.writefloat:
                                case CodeType.addfloat:
                                case CodeType.subfloat:
                                case CodeType.mulfloat:
                                case CodeType.divfloat:
                                case CodeType.ifeqfloat:
                                case CodeType.ifnefloat:
                                case CodeType.ifltfloat:
                                case CodeType.iflteqfloat:
                                case CodeType.ifgtfloat:
                                case CodeType.ifgteqfloat:
                                    writer.Write(float.Parse(line.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo));
                                    break;
                                default:
                                    writer.Write(unchecked((int)long.Parse(line.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo)));
                                    break;
                            }
                            break;
                        case ValueType.hex:
                            writer.Write(uint.Parse(line.Value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
                            break;
                    }
                writer.Write(line.RepeatCount ?? 1);
                if (line.IsIf)
                {
                    WriteCodes64(line.TrueLines, writer);
                    if (line.FalseLines.Count > 0)
                    {
                        writer.Write((byte)CodeType.@else);
                        WriteCodes64(line.FalseLines, writer);
                    }
                    writer.Write((byte)CodeType.endif);
                }
            }
        }

        [XmlRoot]
        public class CodeList
        {
            [XmlElement("Code")]
            public List<Code> Codes { get; set; }
        }

        public class Code
        {
            [XmlAttribute("name")]
            public string Name { get; set; }
            [XmlAttribute("required")]
            public bool Required { get; set; }
            [XmlAttribute("patch")]
            public bool Patch { get; set; }
            [XmlElement("CodeLine")]
            public List<CodeLine> Lines { get; set; }

            [XmlIgnore]
            public bool IsReg { get { return Lines.Any((line) => line.IsReg); } }

            public override string ToString()
            {
                return Name;
            }
        }

        public class CodeLine
        {
            public CodeType Type { get; set; }
            [XmlElement(IsNullable = false)]
            public string Address { get; set; }
            public bool Pointer { get; set; }
            [XmlIgnore]
            public bool PointerSpecified { get { return Pointer; } set { } }
            [XmlIgnore]
            public List<long> Offsets { get; set; }
            [XmlArray("Offsets")]
            [XmlArrayItem("Offset")]
            public string[] OffsetStrings
            {
                get { return Offsets?.Select((a) => a.ToString("X")).ToArray(); }
                set { Offsets = value.Select((a) => long.Parse(a, NumberStyles.HexNumber)).ToList(); }
            }
            [XmlIgnore]
            public bool OffsetStringsSpecified { get { return Offsets != null && Offsets.Count > 0; } set { } }
            [XmlElement(IsNullable = false)]
            public string Value { get; set; }
            public ValueType ValueType { get; set; }
            public uint? RepeatCount { get; set; }
            [XmlIgnore]
            public bool RepeatCountSpecified { get { return RepeatCount.HasValue; } set { } }
            [XmlArray]
            public List<CodeLine> TrueLines { get; set; }
            [XmlIgnore]
            public bool TrueLinesSpecified { get { return TrueLines.Count > 0 && IsIf; } set { } }
            [XmlArray]
            public List<CodeLine> FalseLines { get; set; }
            [XmlIgnore]
            public bool FalseLinesSpecified { get { return FalseLines.Count > 0 && IsIf; } set { } }

            [XmlIgnore]
            public bool IsIf
            {
                get
                {
                    return (Type >= CodeType.ifeq8 && Type <= CodeType.ifkbkey)
                        || (Type >= CodeType.ifeqreg8 && Type <= CodeType.ifmaskreg32);
                }
            }

            [XmlIgnore]
            public bool IsReg
            {
                get
                {
                    if (IsIf)
                    {
                        if (TrueLines.Any((line) => line.IsReg))
                            return true;
                        if (FalseLines.Any((line) => line.IsReg))
                            return true;
                    }
                    if (Address.StartsWith("r"))
                        return true;
                    if (Type >= CodeType.readreg8 && Type <= CodeType.ifmaskreg32)
                        return true;
                    return false;
                }
            }
        }

        public enum CodeType
        {
            write8, write16, write32, writefloat,
            add8, add16, add32, addfloat,
            sub8, sub16, sub32, subfloat,
            mulu8, mulu16, mulu32, mulfloat,
            muls8, muls16, muls32,
            divu8, divu16, divu32, divfloat,
            divs8, divs16, divs32,
            modu8, modu16, modu32,
            mods8, mods16, mods32,
            shl8, shl16, shl32,
            shru8, shru16, shru32,
            shrs8, shrs16, shrs32,
            rol8, rol16, rol32,
            ror8, ror16, ror32,
            and8, and16, and32,
            or8, or16, or32,
            xor8, xor16, xor32,
            writenop,
            ifeq8, ifeq16, ifeq32, ifeqfloat,
            ifne8, ifne16, ifne32, ifnefloat,
            ifltu8, ifltu16, ifltu32, ifltfloat,
            iflts8, iflts16, iflts32,
            ifltequ8, ifltequ16, ifltequ32, iflteqfloat,
            iflteqs8, iflteqs16, iflteqs32,
            ifgtu8, ifgtu16, ifgtu32, ifgtfloat,
            ifgts8, ifgts16, ifgts32,
            ifgtequ8, ifgtequ16, ifgtequ32, ifgteqfloat,
            ifgteqs8, ifgteqs16, ifgteqs32,
            ifmask8, ifmask16, ifmask32,
            ifkbkey,
            readreg8, readreg16, readreg32,
            writereg8, writereg16, writereg32,
            addreg8, addreg16, addreg32, addregfloat,
            subreg8, subreg16, subreg32, subregfloat,
            mulregu8, mulregu16, mulregu32, mulregfloat,
            mulregs8, mulregs16, mulregs32,
            divregu8, divregu16, divregu32, divregfloat,
            divregs8, divregs16, divregs32,
            modregu8, modregu16, modregu32,
            modregs8, modregs16, modregs32,
            shlreg8, shlreg16, shlreg32,
            shrregu8, shrregu16, shrregu32,
            shrregs8, shrregs16, shrregs32,
            rolreg8, rolreg16, rolreg32,
            rorreg8, rorreg16, rorreg32,
            andreg8, andreg16, andreg32,
            orreg8, orreg16, orreg32,
            xorreg8, xorreg16, xorreg32,
            writenopreg,
            ifeqreg8, ifeqreg16, ifeqreg32, ifeqregfloat,
            ifnereg8, ifnereg16, ifnereg32, ifneregfloat,
            ifltregu8, ifltregu16, ifltregu32, ifltregfloat,
            ifltregs8, ifltregs16, ifltregs32,
            iflteqregu8, iflteqregu16, iflteqregu32, iflteqregfloat,
            iflteqregs8, iflteqregs16, iflteqregs32,
            ifgtregu8, ifgtregu16, ifgtregu32, ifgtregfloat,
            ifgtregs8, ifgtregs16, ifgtregs32,
            ifgteqregu8, ifgteqregu16, ifgteqregu32, ifgteqregfloat,
            ifgteqregs8, ifgteqregs16, ifgteqregs32,
            ifmaskreg8, ifmaskreg16, ifmaskreg32,
            s8tos32, s16tos32, s32tofloat, u32tofloat, floattos32, floattou32,
            @else,
            endif,
            newregs
        }

        public enum ValueType
        {
            @decimal,
            hex
        }

    }
}

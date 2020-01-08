using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Input;

namespace HedgeModManager
{
    public class CodeLoader
    {
        public static string CodesXMLPath => Path.Combine(App.ModsDbPath, "Codes.xml");
        public static string CodesHMMPath => Path.Combine(App.ModsDbPath, "Codes.hmm");
        public static string CodesPath => Path.Combine(App.ModsDbPath, "Codes.dat");

        public static CodeList LoadAllCodes()
        {
            if (File.Exists(CodesHMMPath))
                return CodeList.Load(CodesHMMPath);
            else if (File.Exists(CodesXMLPath))
            {
                var list = CodeList.Load(CodesXMLPath);
                list.Save(CodesHMMPath);
                return list;
            }
            else
                return new CodeList();
        }
    }

    [XmlRoot]
    public class CodeList
    {
        static readonly XmlSerializer serializer = new XmlSerializer(typeof(CodeList));

        public static CodeList Load(string filename)
        {
            if (Path.GetExtension(filename).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                using (FileStream fs = File.OpenRead(filename))
                    return (CodeList)serializer.Deserialize(fs);
            else
                using (StreamReader sr = File.OpenText(filename))
                {
                    CodeList result = new CodeList();
                    Stack<Tuple<List<CodeLine>, List<CodeLine>>> stack = new Stack<Tuple<List<CodeLine>, List<CodeLine>>>();
                    int linenum = 0;
                    while (!sr.EndOfStream)
                    {
                        ++linenum;
                        string line = sr.ReadLine().Trim(' ', '\t');
                        if (line.Length == 0) continue;
                        if (line.StartsWith(";")) continue;
                        string[] split = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        Code code = null;
                        switch (split[0])
                        {
                            case "Code":
                                if (stack.Count > 1)
                                    throw new FormatException($"Invalid code line \"{line}\" in {filename}:line {linenum}");
                                if (stack.Count == 1)
                                    stack.Pop();

                                code = new Code();
                                ProcessCodeLine(filename, linenum, split, code);
                                result.Codes.Add(code);
                                stack.Push(new Tuple<List<CodeLine>, List<CodeLine>>(code.Lines, null));
                                break;
                            case "Patch":
                                if (stack.Count > 1)
                                    throw new FormatException($"Invalid code line \"{line}\" in {filename}:line {linenum}");
                                if (stack.Count == 1)
                                    stack.Pop();
                                code = new Code() { Patch = true };
                                ProcessCodeLine(filename, linenum, split, code);
                                result.Codes.Add(code);
                                stack.Push(new Tuple<List<CodeLine>, List<CodeLine>>(code.Lines, null));
                                break;
                            default:
                                if (Enum.TryParse(split[0], out CodeType type))
                                {
                                    switch (type)
                                    {
                                        case CodeType.@else:
                                            if (stack.Peek().Item2 == null)
                                                throw new FormatException($"Invalid code line \"{line}\" in {filename}:line {linenum}");
                                            stack.Push(new Tuple<List<CodeLine>, List<CodeLine>>(stack.Pop().Item2, null));
                                            continue;
                                        case CodeType.endif:
                                            if (stack.Count < 2)
                                                throw new FormatException($"Invalid code line \"{line}\" in {filename}:line {linenum}");
                                            stack.Pop();
                                            continue;
                                        case CodeType.newregs:
                                            throw new FormatException($"Invalid code line \"{line}\" in {filename}:line {linenum}");
                                        default:
                                            break;
                                    }

                                    CodeLine cl = new CodeLine() { Type = type };
                                    string address = split[1];
                                    if (address.StartsWith("p"))
                                    {
                                        cl.Pointer = true;
                                        string[] offs = address.Split('|');
                                        cl.Address = offs[0].Substring(1);
                                        if (offs.Length > 1)
                                        {
                                            cl.Offsets = new List<int>();
                                            for (int i = 1; i < offs.Length; i++)
                                                cl.Offsets.Add(int.Parse(offs[i], System.Globalization.NumberStyles.HexNumber));
                                        }
                                    }
                                    else
                                        cl.Address = address;
                                    int it = 2;
                                    switch (type)
                                    {
                                        case CodeType.s8tos32:
                                        case CodeType.s16tos32:
                                        case CodeType.s32tofloat:
                                        case CodeType.u32tofloat:
                                        case CodeType.floattos32:
                                        case CodeType.floattou32:
                                            cl.Value = "0";
                                            break;
                                        default:
                                            cl.Value = split[it++];
                                            break;
                                    }
                                    if (it < split.Length && !split[it].StartsWith(";"))
                                        cl.RepeatCount = uint.Parse(split[it++].Substring(1));
                                    stack.Peek().Item1.Add(cl);
                                    if (cl.IsIf)
                                        stack.Push(new Tuple<List<CodeLine>, List<CodeLine>>(cl.TrueLines, cl.FalseLines));
                                }
                                else
                                    throw new FormatException($"Invalid code line \"{line}\" in {filename}:line {linenum}");
                                break;
                        }
                    }
                    return result;
                }
        }

        private static void ProcessCodeLine(string filename, int linenum, string[] split, Code code)
        {
            var sb = new System.Text.StringBuilder(split[1].TrimStart('"'));
            int i = 2;
            if (!split[1].EndsWith("\""))
                for (; i < split.Length; i++)
                {
                    sb.AppendFormat(" {0}", split[i]);
                    if (split[i].EndsWith("\"")) { ++i; break; }
                }
            code.Name = sb.ToString().TrimEnd('"');
            if (i < split.Length)
                for (; i < split.Length; i++)
                {
                    if (split[i].StartsWith(";")) break;
                    switch (split[i])
                    {
                        case "Required":
                            code.Required = true;
                            break;
                        default:
                            throw new Exception($"Unknown attribute {split[i]} in code \"{code.Name}\" in {filename}:line {linenum}");
                    }
                }
        }

        public void Save(string filename)
        {
            if (Path.GetExtension(filename).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                using (FileStream fs = File.Create(filename))
                    serializer.Serialize(fs, this);
            else
                using (StreamWriter sw = File.CreateText(filename))
                {
                    foreach (Code code in Codes)
                    {
                        sw.Write("{0} \"{1}\"", code.Patch ? "Patch" : "Code", code.Name);
                        if (code.Required)
                            sw.Write(" Required");
                        sw.WriteLine();
                        List<CodeLine> lines = code.Lines;
                        SaveCodeLines(sw, lines, 0);
                        sw.WriteLine();
                    }
                }
        }

        private static void SaveCodeLines(StreamWriter sw, List<CodeLine> lines, int indent)
        {
            foreach (CodeLine line in lines)
            {
                sw.Write("{0}{1} ", new string('\t', indent), line.Type);
                if (line.Pointer)
                    sw.Write("p");
                sw.Write(line.Address);
                if (line.Offsets != null)
                    foreach (int off in line.Offsets)
                        sw.Write("|{0:X}", off);
                switch (line.ValueType)
                {
                    case ValueType.hex:
                        sw.Write(" 0x{0}", line.Value);
                        break;
                    default:
                        sw.Write(" {0}", line.Value);
                        break;
                }
                if (line.RepeatCount.HasValue)
                    sw.Write(" x{0}", line.RepeatCount.Value);
                sw.WriteLine();
                if (line.IsIf)
                {
                    if (line.TrueLines != null && line.TrueLines.Count > 0)
                        SaveCodeLines(sw, line.TrueLines, indent + 1);
                    if (line.FalseLines != null && line.FalseLines.Count > 0)
                    {
                        sw.WriteLine("{0}else", new string('\t', indent));
                        SaveCodeLines(sw, line.FalseLines, indent + 1);
                    }
                    sw.WriteLine("{0}endif", new string('\t', indent));
                }
            }
        }

        [XmlElement("Code")]
        public List<Code> Codes { get; set; } = new List<Code>();

        public static void WriteDatFile(string path, IList<Code> codes, bool is64Bit)
        {
            foreach(var code in codes)
            {
                if (!code.Patch)
                    continue;

                foreach(var line in code.Lines)
                {
                    line.Patch = code.Patch;
                }
            }

            using (FileStream fs = File.Create(path))
            using (BinaryWriter bw = new BinaryWriter(fs, Encoding.ASCII))
            {
                bw.Write(new[] { 'c', 'o', 'd', 'e', 'v', '5', '1' });
                bw.Write(codes.Count);
                foreach (Code item in codes)
                {
                    if (item.IsReg)
                        bw.Write((byte)CodeType.newregs);
                    if (is64Bit)
                        WriteCodes64(item.Lines, bw);
                    else
                        WriteCodes(item.Lines, bw);
                }
                bw.Write(byte.MaxValue);
            }
        }

        private static void WriteCodes(List<CodeLine> lines, BinaryWriter bw)
        {
            foreach (CodeLine line in lines)
            {
                bw.Write((byte)line.Type);
                bw.Write((byte)(line.Patch ? 1 : 0));
                uint address;
                if (line.Address.StartsWith("r"))
                    address = uint.Parse(line.Address.Substring(1), System.Globalization.NumberStyles.None, System.Globalization.NumberFormatInfo.InvariantInfo);
                else
                    address = uint.Parse(line.Address, System.Globalization.NumberStyles.HexNumber);
                if (line.Pointer)
                    address |= 0x80000000u;
                bw.Write(address);
                if (line.Pointer)
                    if (line.Offsets != null)
                    {
                        bw.Write((byte)line.Offsets.Count);
                        foreach (int off in line.Offsets)
                            bw.Write(off);
                    }
                    else
                        bw.Write((byte)0);
                if (line.Type == CodeType.ifkbkey)
                    bw.Write((int)(Keys)Enum.Parse(typeof(Keys), line.Value));
                else
                    switch (line.ValueType)
                    {
                        case null:
                            if (line.Value.StartsWith("0x"))
                                bw.Write(uint.Parse(line.Value.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo));
                            else if (line.IsFloat)
                                bw.Write(float.Parse(line.Value, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo));
                            else
                                bw.Write(unchecked((int)long.Parse(line.Value, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo)));
                            break;
                        case ValueType.@decimal:
                            if (line.IsFloat)
                                bw.Write(float.Parse(line.Value, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo));
                            else
                                bw.Write(unchecked((int)long.Parse(line.Value, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo)));
                            break;
                        case ValueType.hex:
                            bw.Write(uint.Parse(line.Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo));
                            break;
                    }
                bw.Write(line.RepeatCount ?? 1);
                if (line.IsIf)
                {
                    WriteCodes(line.TrueLines, bw);
                    if (line.FalseLines.Count > 0)
                    {
                        bw.Write((byte)CodeType.@else);
                        WriteCodes(line.FalseLines, bw);
                    }
                    bw.Write((byte)CodeType.endif);
                }
            }
        }
        private static void WriteCodes64(List<CodeLine> lines, BinaryWriter bw)
        {
            foreach (CodeLine line in lines)
            {
                bw.Write((byte)line.Type);
                bw.Write((byte)(line.Patch ? 1 : 0));
                ulong address;
                if (line.Address.StartsWith("r"))
                    address = ulong.Parse(line.Address.Substring(1), System.Globalization.NumberStyles.None, System.Globalization.NumberFormatInfo.InvariantInfo);
                else
                    address = ulong.Parse(line.Address, System.Globalization.NumberStyles.HexNumber);
                if (line.Pointer)
                    address |= 0x8000000000000000ul;
                bw.Write(address);
                if (line.Pointer)
                    if (line.Offsets != null)
                    {
                        bw.Write((byte)line.Offsets.Count);
                        foreach (int off in line.Offsets)
                            bw.Write(off);
                    }
                    else
                        bw.Write((byte)0);
                if (line.Type == CodeType.ifkbkey)
                    bw.Write((int)(Keys)Enum.Parse(typeof(Keys), line.Value));
                else
                    switch (line.ValueType)
                    {
                        case null:
                            if (line.Value.StartsWith("0x"))
                                bw.Write(uint.Parse(line.Value.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo));
                            else if (line.IsFloat)
                                bw.Write(float.Parse(line.Value, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo));
                            else
                                bw.Write(unchecked((int)long.Parse(line.Value, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo)));
                            break;
                        case ValueType.@decimal:
                            if (line.IsFloat)
                                bw.Write(float.Parse(line.Value, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo));
                            else
                                bw.Write(unchecked((int)long.Parse(line.Value, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo)));
                            break;
                        case ValueType.hex:
                            bw.Write(uint.Parse(line.Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo));
                            break;
                    }
                bw.Write(line.RepeatCount ?? 1);
                if (line.IsIf)
                {
                    WriteCodes64(line.TrueLines, bw);
                    if (line.FalseLines.Count > 0)
                    {
                        bw.Write((byte)CodeType.@else);
                        WriteCodes64(line.FalseLines, bw);
                    }
                    bw.Write((byte)CodeType.endif);
                }
            }
        }
    }

    public class Code
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("credit")]
        public string Credit { get; set; }
        [XmlAttribute("required")]
        public bool Required { get; set; }
        [XmlAttribute("patch")]
        public bool Patch { get; set; }
        [XmlElement("CodeLine")]
        public List<CodeLine> Lines { get; set; } = new List<CodeLine>();

        [XmlIgnore]
        public bool IsReg { get { return Lines.Any((line) => line.IsReg); } }

        [XmlIgnore]
        public bool Enabled { get; set; }

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
        public bool Patch { get; set; }
        [XmlIgnore]
        public bool PointerSpecified { get { return Pointer; } set { } }
        [XmlIgnore]
        public List<int> Offsets { get; set; }
        [XmlArray("Offsets")]
        [XmlArrayItem("Offset")]
        public string[] OffsetStrings
        {
            get { return Offsets?.Select((a) => a.ToString("X")).ToArray(); }
            set { Offsets = value.Select((a) => int.Parse(a, System.Globalization.NumberStyles.HexNumber)).ToList(); }
        }
        [XmlIgnore]
        public bool OffsetStringsSpecified { get { return Offsets != null && Offsets.Count > 0; } set { } }
        [XmlElement(IsNullable = false)]
        public string Value { get; set; }
        public ValueType? ValueType { get; set; }
        public uint? RepeatCount { get; set; }
        [XmlIgnore]
        public bool RepeatCountSpecified { get { return RepeatCount.HasValue; } set { } }
        [XmlArray]
        public List<CodeLine> TrueLines { get; set; } = new List<CodeLine>();
        [XmlIgnore]
        public bool TrueLinesSpecified { get { return TrueLines.Count > 0 && IsIf; } set { } }
        [XmlArray]
        public List<CodeLine> FalseLines { get; set; } = new List<CodeLine>();
        [XmlIgnore]
        public bool FalseLinesSpecified { get { return FalseLines.Count > 0 && IsIf; } set { } }

        [XmlIgnore]
        public bool IsFloat
        {
            get
            {
                switch (Type)
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
                    case CodeType.addregfloat:
                    case CodeType.subregfloat:
                    case CodeType.mulregfloat:
                    case CodeType.divregfloat:
                    case CodeType.ifeqregfloat:
                    case CodeType.ifneregfloat:
                    case CodeType.ifltregfloat:
                    case CodeType.iflteqregfloat:
                    case CodeType.ifgtregfloat:
                    case CodeType.ifgteqregfloat:
                        return true;
                    default:
                        return false;
                }
            }
        }

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
        writeoff,
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

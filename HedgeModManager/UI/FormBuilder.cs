using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using HedgeModManager.Controls;
using Newtonsoft.Json;
using static HedgeModManager.Lang;

namespace HedgeModManager.UI
{
    public class FormBuilder
    {
        public static Dictionary<string, Type> TypeDatabase = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "string", typeof(string) },
            { "int", typeof(long) },
            { "double", typeof(double) },
            { "float", typeof(float) },
            { "bool", typeof(bool) }
        };

        public static Thickness GroupMargin = new Thickness(0, 0, 0, 15);

        public static Panel Build(FormSchema schema, Action<string> onDescriptionHover = null)
        {
            var stack = new StackPanel();
            foreach (var group in schema.Groups)
            {
                var box = new GroupBox(){ Name = group.Name, Header = group.DisplayName, Margin = GroupMargin};
                var panel = new StackPanel();
                box.Content = panel;
                foreach (var element in group.Elements)
                {
                    var item = new FormItem(element, schema);
                    item.OnDescriptionHover += onDescriptionHover;
                    panel.Children.Add(item);
                }

                stack.Children.Add(box);
            }

            return stack;
        }
    }

    public class FormElement
    {
        public string Name;
        public List<string> Description = new List<string>();
        public string DisplayName;
        public string Type;
        public double? MinValue;
        public double? MaxValue;
        public dynamic DefaultValue;

        [JsonIgnore]
        public dynamic Value { get; set; }

        [JsonIgnore]
        public long ValueLong
        {
            get => Value;
            set => Value = value;
        }

        [JsonIgnore]
        public double ValueDouble
        {
            get => Value;
            set => Value = value;
        }
    }

    public class FormGroup
    {
        public string Name;
        public string DisplayName;
        public List<FormElement> Elements = new List<FormElement>();
    }

    public class FormEnum
    {
        public string DisplayName;
        public string Value;
        public List<string> Description = new List<string>();

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class FormSchema
    {
        public string IniFile { get; set; } = "Config.ini";
        public List<FormGroup> Groups = new List<FormGroup>();
        public Dictionary<string, List<FormEnum>> Enums = new Dictionary<string, List<FormEnum>>();

        public bool TryLoad(ModInfo mod)
        {
            return TryLoad(mod, Path.Combine(mod.RootDirectory, mod.ConfigSchema.IniFile));
        }

        public bool TryLoad(ModInfo mod, string iniPath)
        {
            try
            {
                LoadValuesFromIni(iniPath);
                return true;
            }
            catch 
            {
                var messageBox = new HedgeMessageBox(Localise("DialogUIConfigLoadErrorHeader"), LocaliseFormat("DialogUIConfigLoadErrorBody", mod.Title, mod.Author));
                messageBox.AddButton(Localise("CommonUIOK"), () => messageBox.Close());
                messageBox.ShowDialog();
                return false;
            }
        }

        public void LoadValuesFromIni(string path)
        {
            if (!File.Exists(path))
                return;

            var file = new IniFile(path);
            foreach (var group in Groups)
            {
                foreach (var element in group.Elements)
                {
                    if (FormBuilder.TypeDatabase.TryGetValue(element.Type, out var t))
                    {
                        if (file.Groups.ContainsKey(group.Name) && file[group.Name].Params.ContainsKey(element.Name))
                        {
                            element.Value = Convert.ChangeType(file[group.Name][element.Name], t);
                        }
                    }
                    else if (file.Groups.ContainsKey(group.Name) && file[group.Name].Params.ContainsKey(element.Name))
                    {
                        element.Value = file[group.Name][element.Name];
                    }
                }
            }
        }

        public void SaveIni(string path)
        {
            var file = new IniFile();
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    file.Read(stream);
                }
            }
            foreach (var group in Groups)
            {
                foreach (var element in group.Elements)
                {
                    if (element.Value is FrameworkElement && ((FrameworkElement)element.Value).DataContext is FormEnum)
                        file[group.Name][element.Name] = ((FormEnum)((FrameworkElement)element.Value).DataContext)?.Value;
                    else
                        file[group.Name][element.Name] = element.Value?.ToString() ?? element.DefaultValue.ToString();
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var stream = File.Create(path))
            {
                file.Write(stream);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for PropertyEditor.xaml
    /// </summary>
    public partial class PropertyEditor : UserControl
    {
        protected object mInstance;
        public object Instance
        {
            get
            {
                return mInstance;
            }
            set
            {
                mInstance = value;
                RefreshProperties();
            }
        }
        public PropertyEditor()
        {
            InitializeComponent();
        }

        protected void RefreshProperties()
        {
            PropList.Items.Clear();
            foreach(var prop in Instance.GetType().GetProperties())
            {
                var info = new PropInfo(Instance, prop);
                if (!info.IsIndexed && prop.GetCustomAttribute(typeof(PropertyIgnore)) == null)
                {
                    PropList.Items.Add(info);
                }
            }
        }

        private void PropList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            new EditPropertyWindow((PropInfo)PropList.SelectedItem).ShowDialog();
        }
    }

    public class PropInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get { return PropertyInfo.Name; } }
        public object Value { get { return PropertyInfo.GetValue(Object); } }
        public object Object { get; }
        public PropertyInfo PropertyInfo { get; }
        public bool IsIndexed { get; }

        public string AsString { get { return ToString(); } }

        public PropInfo(object obj, string propName)
        {
            Object = obj;
            PropertyInfo = obj.GetType().GetProperty(propName);
            IsIndexed = PropertyInfo.GetIndexParameters().Length > 0;
        }

        public PropInfo(object obj, PropertyInfo prop)
        {
            Object = obj;
            PropertyInfo = prop;
            IsIndexed = PropertyInfo.GetIndexParameters().Length > 0;
        }

        public void Notify()
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public override string ToString()
        {
            if (PropertyInfo.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyInfo.PropertyType))
            {
                var col = Value as IEnumerable;
                var res = string.Empty;
                foreach (var item in col)
                {
                    res += $"{item.ToString()};";
                }
                return res;
            }
            return Value == null ? string.Empty : Value.ToString();
        }
    }

    public class PropertyIgnore : Attribute
    {

    }
}

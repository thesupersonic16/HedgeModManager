using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HedgeModManager.UI;

namespace HedgeModManager.Controls
{
    /// <summary>
    /// Interaction logic for FormItem.xaml
    /// </summary>
    public partial class FormItem : UserControl
    {
        public event Action<string> OnDescriptionHover;
        
        public FormSchema Schema { get; }
        public FormItem()
        {
            InitializeComponent();
        }

        public FormItem(FormElement element, FormSchema schema)
        {
            InitializeComponent();
            Schema = schema;
            if (element.Description != null && element.Description.Count > 0)
            {
                DisplayName.Tag = string.Join("\r\n", element.Description);

                DisplayName.MouseEnter += ItemOnMouseEnter;
            }

            DisplayName.Content = element.DisplayName;
            DisplayValue.Child = CreateUiElement(element);

            DisplayValue.Tag = DisplayName.Tag;
            DisplayValue.MouseEnter += ItemOnMouseEnter;
        }

        public UIElement CreateUiElement(FormElement element)
        {
            if (element.DefaultValue == null && element.Value == null)
            {
                if(FormBuilder.TypeDatabase.TryGetValue(element.Type, out Type t))
                    element.DefaultValue = Activator.CreateInstance(t);
            }

            if (element.Value is FrameworkElement fElement && fElement.DataContext is FormEnum fEnum)
            {
                element.Value = fEnum.Value;
            }

            if (element.Value == null)
                element.Value = element.DefaultValue;
            
            switch (element.Type)
            {
                case "double":
                case "float":
                    return CreateTextBox(element, "ValueDouble");
                case "uint":
                case "int":
                    return CreateTextBox(element, "ValueLong");
                case "string":
                    return CreateTextBox(element, "Value");

                case "bool":
                {
                    var box = new CheckBox()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    box.SetBinding(ToggleButton.IsCheckedProperty, "Value");
                    box.DataContext = element;
                    return box;
                }
                default:
                {
                    if (Schema.Enums.TryGetValue(element.Type, out var enums))
                    {
                        var box = new ComboBox
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            Height = 20,
                            DataContext = element,
                        };

                        foreach (var val in enums)
                        {
                            var item = new ComboBoxItem()
                            {
                                Content = val.DisplayName, 
                                DataContext = val,
                            };

                            if (val.Description != null && val.Description.Count > 0)
                            {
                                item.Tag = string.Join("\r\n", val.Description);
                            }

                            item.MouseMove += ItemOnMouseEnter;
                            box.Items.Add(item);
                        }

                        box.SetBinding(Selector.SelectedValueProperty, "Value");
                        
                        if (element.Value != null)
                        {
                            var value = enums.FirstOrDefault(x => x.Value == element.Value.ToString());
                            var i = enums.IndexOf(value);
                            box.SelectedIndex=  i < 0 ? 0 : i;
                        }
                        else
                        {
                            box.SelectedIndex = 0;
                        }

                        return box;
                    }
                    else
                    {
                        throw new NotImplementedException($"Element of type {element.Type} is not supported.");
                    }
                }
            }
        }

        private void ItemOnMouseEnter(object sender, MouseEventArgs e)
        {
            OnDescriptionHover?.Invoke((sender as FrameworkElement)?.Tag as string);
        }

        TextBox CreateTextBox(FormElement context, string binding)
        {
            var box = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            box.SetBinding(TextBox.TextProperty, new Binding(binding) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

            box.DataContext = context;

            if (context.MinValue.HasValue || context.MaxValue.HasValue)
            {
                var rule = new RangeValidator(context.MinValue ?? double.NegativeInfinity, context.MaxValue ?? double.PositiveInfinity);
                BindingOperations.GetBinding(box, TextBox.TextProperty)?.ValidationRules.Add(rule);
            }

            return box;
        }
    }
}

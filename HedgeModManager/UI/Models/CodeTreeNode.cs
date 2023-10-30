using HedgeModManager.CodeCompiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace HedgeModManager.UI.Models
{
    public class CodeTreeNode : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsExpanded { get; set; }

        public bool IsCategory { get; set; }

        public bool IsUncategorised { get; set; }

        public ObservableCollection<object> Children { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;

        public static ObservableCollection<object> BuildCategoryTree(IEnumerable<CSharpCode> codes)
        {
            var root = new CodeTreeNode();

            foreach (var code in codes)
            {
                var categories = code.Category.Split('/');
                var currentNode = root;
                var path = string.Empty;

                for (int i = 0; i < categories.Length; i++)
                {
                    var category = categories[i];
                    bool isUncategorised = false;

                    if (string.IsNullOrEmpty(category))
                    {
                        category = Lang.Localise("CodesUINullCategory");
                        isUncategorised = true;
                    }

                    var subcategory = currentNode.Children.FirstOrDefault(c => (c as CodeTreeNode)?.Name == category);

                    // Set path for traversing cached expanded nodes.
                    path += i == 0 ? category : $"/{category}";

                    if (subcategory == null)
                    {
                        subcategory = new CodeTreeNode
                        {
                            Name = category,
                            Path = path,
                            IsCategory = true,
                            IsUncategorised = isUncategorised
                        };

                        currentNode.Children.Add(subcategory);
                    }

                    currentNode = (CodeTreeNode)subcategory;
                }

                currentNode.Children.Add(code);
            }

            void Sort(CodeTreeNode node)
            {
                var categories = new List<CodeTreeNode>();
                var codes = new List<CSharpCode>();

                foreach (var child in node.Children)
                {
                    if (child is CodeTreeNode c)
                    {
                        if (c.IsCategory)
                        {
                            categories.Add(c);
                            Sort(c);
                        }
                    }
                    else if (child is CSharpCode cc)
                    {
                        codes.Add(cc);
                    }
                }

                node.Children.Clear();

                categories.Sort((a, b) => a.Name.CompareTo(b.Name));
                codes.Sort((a, b) => a.Name.CompareTo(b.Name));

                // Add uncategorised nodes.
                foreach (var cat in categories)
                {
                    if (!cat.IsUncategorised)
                        continue;

                    node.Children.Add(cat);
                }

                // Add category nodes.
                foreach (var cat in categories)
                {
                    if (cat.IsUncategorised)
                        continue;

                    node.Children.Add(cat);
                }

                // Add code nodes.
                foreach (var code in codes)
                    node.Children.Add(code);
            }

            Sort(root);

            return root.Children;
        }
    }
}

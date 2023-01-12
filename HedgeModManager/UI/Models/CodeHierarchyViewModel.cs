using System.Collections.Generic;

namespace HedgeModManager.UI.Models
{
    public class CodeHierarchyViewModel
    {
        public string Name { get; set; }

        public Code Code { get; set; }

        public bool IsExpanded { get; set; }

        public bool IsRoot { get; set; } = false;

        public IEnumerable<CodeHierarchyViewModel> Children { get; set; }
    }
}

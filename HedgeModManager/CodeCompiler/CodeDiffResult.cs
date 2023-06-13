using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CodeCompiler
{
    public class CodeDiffResult
    {
        /// <summary>
        /// The details about this change.
        /// </summary>
        public string Changelog { get; set; }

        /// <summary>
        /// The change made to this code.
        /// </summary>
        public CodeDiffType Type { get; set; }

        /// <summary>
        /// The original name of a code before renaming.
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// The new name of a code after renaming.
        /// </summary>
        public string NewName { get; set; }

        public CodeDiffResult(string changelog, CodeDiffType type)
        {
            Changelog = changelog;
            Type = type;
        }

        public CodeDiffResult(string changelog, CodeDiffType type, string originalName, string newName)
        {
            Changelog = changelog;
            Type = type;
            OriginalName = originalName;
            NewName = newName;
        }

        public override string ToString()
        {
            string key = Type switch
            {
                CodeDiffType.Added => "DiffUIAdded",
                CodeDiffType.Removed => "DiffUIRemoved",
                CodeDiffType.Renamed => "DiffUIRenamed",
                _ => "DiffUIModified",
            };

            return Lang.LocaliseFormat(key, Changelog);
        }

        public enum CodeDiffType
        {
            Added,
            Modified,
            Removed,
            Renamed
        }
    }
}

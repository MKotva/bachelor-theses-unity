using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class CodeEditorAttribute : Attribute
    {
        public string Description { get; set; }
        public string Arguments { get; set; }

        public CodeEditorAttribute(string description, string attributes = "")
        {
            Description = description;
            Arguments = attributes;
        }
    }
}

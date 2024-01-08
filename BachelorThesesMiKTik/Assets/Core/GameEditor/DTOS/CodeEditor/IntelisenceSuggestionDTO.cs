using Assets.Core.GameEditor.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.DTOS
{
    public class IntelisenceSuggestionDTO
    {
        public string Suggestion { get; set; }
        public CodeEditorAttribute Info { get; set; }

        public IntelisenceSuggestionDTO(string suggestion, CodeEditorAttribute info)
        {
            Suggestion = suggestion;
            Info = info;
        }
    }
}

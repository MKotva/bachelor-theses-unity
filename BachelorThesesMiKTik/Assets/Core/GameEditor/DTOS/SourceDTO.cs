using Assets.Core.GameEditor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.DTOS
{
    public class SourceDTO
    {
        public SourceType Type { get; set; }
        public string URL { get; set; }

        public SourceDTO(SourceType type, string url)
        {
            Type = type;
            URL = url;
        }
    }
}

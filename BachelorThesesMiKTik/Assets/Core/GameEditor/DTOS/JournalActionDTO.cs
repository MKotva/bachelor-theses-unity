using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class JournalActionDTO
    {
        public delegate void ActionPerformer(string actionParams);

        public string Parameters;
        public ActionPerformer Performer; 

        public JournalActionDTO(string parameters, ActionPerformer performer) 
        {
            Parameters = parameters;
            Performer = performer;
        }
    }
}

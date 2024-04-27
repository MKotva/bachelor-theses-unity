using System;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class JournalActionDTO
    {
        public delegate void ActionPerformer(JournalActionDTO action);

        public ActionPerformer Performer; 

        public JournalActionDTO(ActionPerformer performer) 
        {
            Performer = performer;
        }
    }
}

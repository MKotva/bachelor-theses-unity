using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public interface ICreatorComponent
    {
        public void SetObject(GameObject ob);
    }
}

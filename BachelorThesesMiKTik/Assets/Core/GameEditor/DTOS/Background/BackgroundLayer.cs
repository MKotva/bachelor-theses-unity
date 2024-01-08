using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Background
{
    public class BackgroundLayer
    {
        public GameObject Instance { get; set; }
        public BackgroundLayerInfoDTO Info { get; set; }
        public BackgroundLayer(GameObject instance) 
        {
            Instance = instance;
        }

        public BackgroundLayer(GameObject instance, BackgroundLayerInfoDTO info) 
        {
            Instance = instance;
            Info = info;
        }
    }
}

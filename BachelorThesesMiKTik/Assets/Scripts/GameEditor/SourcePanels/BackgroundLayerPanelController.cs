using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.DTOS.Background;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels
{
    public class BackgroundLayerPanelController : SourcePanelController
    {
        [SerializeField] AssetPanelController AssetPanel;
        [SerializeField] public TMP_InputField ParalaxSpeed;

        /// <summary>
        /// This method returns proper SourceDTO class based on selected type in type dropdown.
        /// </summary>
        /// <returns></returns>
        public BackgroundReference GetData()
        {
            var source = AssetPanel.GetData();
            return new BackgroundReference(source, 0);
        }

        /// <summary>
        /// This is serves as init method for source panel, hence MonoBehavior does not provide callable
        /// contructor.
        /// </summary>
        /// <param name="source"></param>
        public void SetData(SourceReference source)
        {
            if (source is BackgroundReference)
            {
                AssetPanel.SetData(source);
                ParalaxSpeed.text = ( (BackgroundReference) source ).ParalaxSpeed.ToString();
            }
        }
    }
}

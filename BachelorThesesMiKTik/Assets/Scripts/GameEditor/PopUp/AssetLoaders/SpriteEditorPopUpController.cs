using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.Managers;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp.Managers
{
    public class SpriteEditorPopUpController : PopUpController
    {
        [SerializeField] TMP_InputField URLSource;
        [SerializeField] TMP_Text NameSource;
        [SerializeField] GameObject ImageSource;

        private AssetSourceDTO SpriteSource;

        public void OnEditClick()
        {
            var instance = SpriteManager.Instance;
            if (instance != null)
            {
                var source = GetComponent();
                SpriteManager.Instance.EditSprite(source);
            }
        }

        public async void OnPreviewClick()
        {
            await SpriteLoader.SetSprite(ImageSource, GetComponent());
        }
        public void SetData(AssetSourceDTO spriteSource)
        {
            if (spriteSource == null)
                return;

            SpriteSource = spriteSource;
            URLSource.text = spriteSource.URL;
            NameSource.text = spriteSource.Name;
        }


        #region PRIVATE
        private AssetSourceDTO GetComponent()
        {
            var name = "";
            if (SpriteSource != null)
                name = SpriteSource.Name;

            return new AssetSourceDTO(name, URLSource.text);
        }
        #endregion
    }
}

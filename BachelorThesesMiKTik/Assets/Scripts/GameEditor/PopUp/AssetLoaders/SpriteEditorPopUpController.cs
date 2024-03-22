using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.OutputControllers;
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
            await SpriteLoader.SetSprite(ImageSource, GetComponent().URL);
        }
        public void SetData(AssetSourceDTO spriteSource)
        {
            if (spriteSource == null)
                return;

            SpriteSource = spriteSource;
            URLSource.text = spriteSource.URL;
            NameSource.text = $"Name : {spriteSource.Name}";
        }


        #region PRIVATE
        private AssetSourceDTO GetComponent()
        {
            var name = "";
            if (SpriteSource != null)
                name = SpriteSource.Name;

            return new AssetSourceDTO(SourceType.Image, URLSource.text)
            {
                Name = name
            };
        }
        #endregion
    }
}

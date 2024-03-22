using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.OutputControllers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp
{
    public class SpriteLoaderPopUpController : PopUpController
    {
        [SerializeField] TMP_InputField URLSource;
        [SerializeField] TMP_InputField NameSource;
        [SerializeField] GameObject ImageSource;
        [SerializeField] OutputController OutputConsole;

        public delegate void OnCreateCall(string name);
        private List<OnCreateCall> callbacks;

        /// <summary>
        /// Enlist callback funtion of event Create. This method will hand over
        /// the created animation to this callback function.
        /// </summary>
        /// <param name="callbackFunction"></param>
        public void SetCallBack(OnCreateCall callbackFunction)
        {
            callbacks.Add(callbackFunction);
        }

        public async void OnCreateClick()
        {
            if (NameSource.text == "")
            {
                OutputConsole.ShowMessage("Empty name!");
                return;
            }

            if (!CheckName(NameSource.text))
            {
                OutputConsole.ShowMessage("Invalid name, name is already used!");
                return;
            }

            var instance = SpriteManager.Instance;
            if (instance != null)
            {
                var source = GetComponent();
                await SpriteManager.Instance.AddSprite(source);
                InvokeCallBacks(source.Name);
            }
        }

        public async void OnPreviewClick()
        {
            await SpriteLoader.SetSprite(ImageSource, GetComponent().URL);
        }

        #region PRIVATE
        private void Awake()
        {
            callbacks = new List<OnCreateCall>();
        }

        private void InvokeCallBacks(string name)
        {
            foreach (var item in callbacks)
            {
                item.Invoke(name);
            }
        }

        private AssetSourceDTO GetComponent()
        {
            return new AssetSourceDTO(SourceType.Image, URLSource.text)
            {
                Name = NameSource.text,
            };
        }

        private bool CheckName(string name)
        {
            var instance = SpriteManager.Instance;
            if (instance != null)
            {
                if (!instance.ContainsName(name))
                    return true;
            }
            return false;
        }
        #endregion
    }
}

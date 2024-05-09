using Assets.Core.GameEditor.Serializers;
using Assets.Scripts.GameEditor;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.JumpSystem
{
    public class LoaderController : MonoBehaviour
    {
        [SerializeField] LoadDataHandler LoadDataHandler;

        private void Start()
        {
            var instance = GameManager.Instance;
            if (Loader.Data != null && instance != null)
            {
                var task = Load();
            }
        }

        public async Task Load()
        {
            var instance = GameManager.Instance;
            if (Loader.Data != null && instance != null)
            {
                await instance.LoadLevel(Loader.Data);
                instance.LoadedLevel = Loader.Path;
            }

            var canvasInstane = EditorCanvas.Instance;
            if (canvasInstane)
                canvasInstane.OnDisable();
        }

        //public async Task Load()
        //{
        //    var instance = GameManager.Instance;
        //    if (Loader.Data != null && instance != null)
        //    {
        //        instance.ExitGame();
        //        await GameDataSerializer.Deserialize(Loader.Data);
        //        instance.IsInPlayMode = true;
        //    }

        //    var canvasInstane = EditorCanvas.Instance;
        //    if (canvasInstane)
        //    {
        //        canvasInstane.OnDisable();
        //        canvasInstane.IsDisabled = true;
        //    }

        //    instance.EnterGame();
        //    instance.StartGame();
        //}
    }
}

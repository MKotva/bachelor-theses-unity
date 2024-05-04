using Assets.Core.GameEditor.Serializers;
using Assets.Scripts.GameEditor;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.JumpSystem
{
    public class LoaderController : MonoBehaviour
    {
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
                instance.Clear();
                await GameDataSerializer.Deserialize(Loader.Data);
                instance.EnterGame();
                instance.StartGame();
            }

            var canvasInstane = EditorCanvas.Instance;
            if (canvasInstane)
                canvasInstane.OnDisable();
        }
    }
}

using Assets.Core.GameEditor.Serializers;
using Assets.Scripts.GameEditor;
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
                var task = LoadDataHandler.Load(Loader.Data);
            }
        }
    }
}

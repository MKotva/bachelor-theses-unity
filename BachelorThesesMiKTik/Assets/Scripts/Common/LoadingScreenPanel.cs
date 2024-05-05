using TMPro;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class LoadingScreenPanel : MonoBehaviour
    {
        [SerializeField] TMP_Text TextField;
        [SerializeField] float UpdateRate;
        [SerializeField] string Text;


        private float interval = 0f;
        private int charCount;
        private int index;
        private void Start()
        {
            index = 0;
            charCount = Text.Length;
        }

        private void Update()
        {
            interval += Time.deltaTime;
            if (interval > UpdateRate)
                interval = 0f;
            else
                return;

            if (index == charCount)
            {
                TextField.text = "";
                index = 0;
                return;
            }

            TextField.text += Text[index];
            index++;
        }

    }
}

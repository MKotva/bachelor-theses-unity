using Assets.Core.GameEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.Toolkit
{
    public class SliderController : MonoBehaviour
    {
        [SerializeField] Slider Slider;
        [SerializeField] TMP_InputField ActualValue;

        public float GetSliderValue()
        {
            return Slider.value;
        }

        private void Awake()
        {
            ActualValue.text = Slider.value.ToString();

            Slider.onValueChanged.AddListener(delegate
            {
                ActualValue.text = Slider.value.ToString();
            });

            ActualValue.onValueChanged.AddListener(delegate
            {
                var result = MathHelper.GetFloat(ActualValue.text);
                if (result >= Slider.minValue && result <= Slider.maxValue)
                {
                    Slider.value = result;
                }

            });
        }
    }
}

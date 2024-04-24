using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.PopUp;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels
{
    public class AssetPanelController : MonoBehaviour
    {
        [SerializeField] public TMP_Dropdown SourceT;
        [SerializeField] public TMP_Dropdown AnimationsDropdown;
        [SerializeField] public TMP_Dropdown SpritesDropdown;
        [SerializeField] public TMP_InputField XSize;
        [SerializeField] public TMP_InputField YSize;

        [SerializeField] private GameObject AnimationCreator;
        [SerializeField] private GameObject ImageCreator;
        [SerializeField] private float DefaultX;
        [SerializeField] private float DefaultY;

        private GameObject Canvas;

        /// <summary>
        /// This method returns proper SourceDTO class based on selected type in type dropdown.
        /// </summary>
        /// <returns></returns>
        public SourceReference GetData()
        {
            var x = MathHelper.GetPositiveFloat(XSize.text, DefaultX);
            var y = MathHelper.GetPositiveFloat(YSize.text, DefaultY);

            if (SourceT.value == 0)
                return new SourceReference(GetName(SpritesDropdown), SourceType.Image, x, y);
            else
                return new SourceReference(GetName(AnimationsDropdown), SourceType.Animation, x, y);

        }

        /// <summary>
        /// This is serves as init method for source panel, hence MonoBehavior does not provide callable
        /// contructor.
        /// </summary>
        /// <param name="source"></param>
        public void SetData(SourceReference source)
        {
            if(source.Type == SourceType.Image) 
            {
                SourceT.value = 0;
                SetSpriteDropDown(source.Name);
            }
            else
            {
                SourceT.value = 1;
                SetAnimationDropDown(source.Name);
            }

            XSize.text = source.XSize.ToString();
            YSize.text = source.YSize.ToString();
        }

        #region PRIVATE
        private void Awake()
        {
            SourceT.onValueChanged.AddListener(delegate { ChangeField(); });
            SpritesDropdown.onValueChanged.AddListener(SpriteDropDownChange);
            AnimationsDropdown.onValueChanged.AddListener(AnimationDropDownChange);

            var names = SpriteManager.Instance.Sprites.Keys.ToArray();
            SetDropdown(SpritesDropdown, names);

            Canvas = GameManager.Instance.PopUpCanvas.gameObject;
        }

        private string GetName(TMP_Dropdown dropdown)
        {
            string name = dropdown.options[dropdown.value].text;
            if (name == "Create")
                return "None";
            return name;
        }

        private void ChangeField()
        {
            if (SourceT.value == 1)
            {
                AnimationsDropdown.gameObject.SetActive(true);
                SpritesDropdown.gameObject.SetActive(false);
                SetAnimationDropDown();
            }
            else
            {
                AnimationsDropdown.gameObject.SetActive(false);
                SpritesDropdown.gameObject.SetActive(true);
                SetSpriteDropDown();
            }
        }

        private void SetAnimationDropDown(string defaultValue = "")
        {
            var instance = AnimationsManager.Instance;
            if (instance == null)
                return;

            var names = instance.Animations.Keys.ToArray();
            SetDropdown(AnimationsDropdown, names);

            if (defaultValue != "")
            {
                SetDropdownDefaultValue(AnimationsDropdown, defaultValue);
            }
        }

        private void SetSpriteDropDown(string defaultValue = "")
        {
            var instance = SpriteManager.Instance;
            if (instance == null)
                return;

            var names = instance.Sprites.Keys.ToArray();
            SetDropdown(SpritesDropdown, names);

            if (defaultValue != "")
            {
                SetDropdownDefaultValue(SpritesDropdown, defaultValue);
            }
        }

        private void SetDropdown(TMP_Dropdown dropdown, string[] foundNames)
        {
            dropdown.options.Clear();

            var names = new List<string> { "None" };
            foreach (var name in foundNames)
                names.Add(name);
            names.Add("Create");
            dropdown.AddOptions(names);
        }

        private void SetDropdownDefaultValue(TMP_Dropdown dropdown, string defaultValue)
        {
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (dropdown.options[i].text == defaultValue)
                {
                    dropdown.value = i;
                    dropdown.onValueChanged.Invoke(i);
                    return;
                }
            }
            dropdown.value = 0;
        }

        private void AnimationDropDownChange(int newValue)
        {
            var value = AnimationsDropdown.options[newValue].text;
            if (value == "Create")
            {
                var controller = Instantiate(AnimationCreator, Canvas.transform)
                                    .GetComponent<AnimationCreatorPopUpController>();

                controller.SetCallback(AnimationCreatorExitHandler);
            }
        }

        private void SpriteDropDownChange(int newValue)
        {
            var value = SpritesDropdown.options[newValue].text;
            if (value == "Create")
            {
                var controller = Instantiate(ImageCreator, Canvas.transform)
                                    .GetComponent<SpriteLoaderPopUpController>();

                controller.SetCallBack(SpriteCreatorExitHandler);
            }
        }

        //TODO: Async void is realy bad!! Find better way
        private void AnimationCreatorExitHandler(string name)
        {
            var manager = AnimationsManager.Instance;
            if (manager != null)
            {
                SetAnimationDropDown(name);
            }
        }

        private void SpriteCreatorExitHandler(string name)
        {
            var manager = SpriteManager.Instance;
            if (manager != null)
            {
                SetSpriteDropDown(name);
            }
        }
        #endregion
    }
}

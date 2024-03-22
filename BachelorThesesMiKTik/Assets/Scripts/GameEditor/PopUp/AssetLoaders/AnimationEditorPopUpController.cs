using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AnimationControllers;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.OutputControllers;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.PopUp.AssetLoaders
{
    public class AnimationEditorPopUpController : PopUpController
    {
        [SerializeField] public GameObject LinePrefab;
        [SerializeField] public GameObject ContentView;
        [SerializeField] TMP_Text NameField;
        [SerializeField] Image Image;
        [SerializeField] OutputController OutputConsole;

        private string animaName;
        private List<GameObject> lines;
        private ImageAnimator animatior;

        /// <summary>
        /// Adds line to creator table.
        /// </summary>
        public void OnAddClick()
        {
            var line = Instantiate(LinePrefab, ContentView.transform);
            line.GetComponent<SourcePanelController>().onDestroyClick += DestroyPanel;
            lines.Add(line);
        }

        /// <summary>
        /// Destroys all lines in Creator table.
        /// </summary>
        public void OnClearClick()
        {
            foreach (var line in lines)
                Destroy(line);

            lines.Clear();
        }

        /// <summary>
        /// If the name is not used or empty, invokes all callback methods and gives them 
        /// generated AnimationDTO
        /// </summary>
        public async void OnEditClick()
        { 
            if(lines.Count == 0)
            {
                OutputConsole.ShowMessage("You cant create animation with no frames!");
                return;
            }
            
            var instance = AnimationsManager.Instance;
            if (instance != null)
            {
                var data = GetData();
                await instance.EditAnimation(animaName, data);
            }
        }

        public async void OnPreviewClick()
        {
            var animationDTO = GetData();
            if (animationDTO.AnimationData.Count == 0)
                return;

            var animation = await AnimationLoader.LoadAnimation(animationDTO);

            if (animation == null)
            {
                return;
            }

            animatior = new ImageAnimator(Image, animation, true);
        }

        /// <summary>
        /// Initializes the Animation Creator based on given animation.
        /// </summary>
        /// <param name="data"></param>
        public void SetData(AnimationSourceDTO data)
        {
            animaName = data.Name;
            NameField.text = $"Name: {animaName}";

            foreach (var item in data.AnimationData)
            {
                var line = Instantiate(LinePrefab, ContentView.transform);
                line.transform.GetChild(1).GetComponentInChildren<TMP_InputField>().text = item.DisplayTime.ToString();
                line.transform.GetChild(2).GetComponentInChildren<TMP_InputField>().text = item.URL;

                lines.Add(line);
            }
        }
        #region PRIVATE

        private void Awake()
        {
            lines = new List<GameObject>();
        }

        private void Update()
        {
            if (animatior != null)
            {
                animatior.Animate(Time.deltaTime);
            }
        }

        /// <summary>
        /// Creates AnimationSourceDTO from values in panel items.
        /// </summary>
        /// <returns></returns>
        private AnimationSourceDTO GetData()
        {
            var data = new List<AnimationFrameDTO>();
            foreach (var line in lines)
            {
                var displayTime = line.transform.GetChild(1).GetComponent<TMP_InputField>().text;
                var URL = line.transform.GetChild(2).GetComponent<TMP_InputField>().text;

                if (double.TryParse(displayTime, out var time))
                    data.Add(new AnimationFrameDTO(time, URL));
            }

            return new AnimationSourceDTO(data, NameField.text, SourceType.Animation);
        }

        /// <summary>
        /// Handler of destroy panel event. Destroys panel based on given 
        /// instance ID;
        /// </summary>
        /// <param name="id"></param>
        private void DestroyPanel(int id)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].GetInstanceID() == id)
                {
                    Destroy(lines[i]);
                    lines.RemoveAt(i);
                }
            }
        }

        #endregion
    }

}

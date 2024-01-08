using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.PopUp.CodeEditor;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.CodeEditor
{
    public class IntelisenceController : MonoBehaviour
    {
        public delegate void SuggestionClickHandler(string selected);

        [SerializeField] GameObject ContentView;
        [SerializeField] GameObject SuggestionPrefab;
        [SerializeField] GameObject InfoPanelPrefab;

        public void ShowIntelisence(List<IntelisenceSuggestionDTO> suggestions, SuggestionClickHandler handler, Vector3 position)
        {
            transform.position = position;
            AddSuggestions(suggestions, handler);
        }

        /// <summary>
        /// Adds all found suggestion to the Intelisence Panel. Adds actions on Suggestion click (which will autocomplete the path)
        /// and on Info click (which will show infromations from attribute of suggested object).
        /// </summary>
        /// <param name="suggestions"></param>
        /// <param name="handler"></param>
        public void AddSuggestions(List<IntelisenceSuggestionDTO> suggestions, SuggestionClickHandler handler)
        {
            foreach (var suggestion in suggestions)
            {
                var line = Instantiate(SuggestionPrefab, ContentView.transform);
                
                var sugButton = line.transform.GetChild(0).GetComponent<Button>();
                sugButton.onClick.AddListener(delegate { handler(suggestion.Suggestion); });
                sugButton.transform.GetChild(0).GetComponent<TMP_Text>().text = suggestion.Suggestion;

                var infoButton = line.transform.GetChild(1).GetComponent<Button>();
                infoButton.onClick.AddListener(delegate { OnInfoClick(suggestion.Info); });
            }
        }

        private void OnInfoClick(CodeEditorAttribute attribute)
        {
            var instance = Instantiate(InfoPanelPrefab, transform);
            instance.GetComponent<IntelisenceInfoPanelController>().Set(attribute);
        }
    }
}

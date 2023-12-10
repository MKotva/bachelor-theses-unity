using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace Assets.Scripts.GameEditor.ItemView
{
    public class GameItemController : Singleton<GameItemController>
    {
        [SerializeField] public GameObject ViewPanelPrefab;
        [SerializeField] public GameObject SearchItemField;
        [SerializeField] public GameObject ActualSelectedItemView;
        [SerializeField] public TMP_Dropdown GroupViewSelector;
        [SerializeField] private List<ItemData> DefaultItems;

        public ItemData ActualSelectedItem { get; private set; }
        public Dictionary<string, GameObject> GroupViews {  get; private set; }
        public List<ItemData> Items { get; private set; }

        private GameObject activeGroup;

        private void Start()
        {
            GroupViewSelector.onValueChanged.AddListener( delegate { OnSelectorValueChanged(); });
            ActualSelectedItem = DefaultItems[0];
            GroupViews = new Dictionary<string, GameObject>();
            Items = new List<ItemData>();

            foreach (var item in DefaultItems) 
            {
                AddItem(item);
            }
            activeGroup = GroupViews.Values.First();
            activeGroup.SetActive(true);
        }

        public void AddItem(ItemData item)
        {
            var groupName = item.GroupName;
            if (!GroupViews.ContainsKey(groupName))
            {
                var group = Instantiate(ViewPanelPrefab, transform);
                group.SetActive(false);

                GroupViews.Add(groupName, group);
                GroupViewSelector.AddOptions(new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData(groupName) });
            }

            var controller = GroupViews[groupName].GetComponent<ItemGroupViewController>();
            controller.AddItemButton(item, ViewPlanelButtonClick);

            item.Id = Items.Count;
            Items.Add(item);
        }


        public UnityAction ViewPlanelButtonClick(ItemData item)
        {
            ActualSelectedItem = item;
            ChangeActualItemView(item);
            //MapCanvasController.SetPrefab(item);
            return null;
        }


        public ItemData FindClosestItem(string name, out GameObject itemGroup)
        {
            itemGroup = GroupViews.Values.First();
            ItemData itemData = Items[0];
            int minDistance = int.MaxValue;
         
            foreach (var group in GroupViews) 
            {
                var controller = group.Value.GetComponent<ItemGroupViewController>();
                var item = controller.FindClosestItem(name, out var distance);
                if(minDistance > distance)
                {
                    itemGroup = group.Value;
                    itemData = item;
                    minDistance = distance;
                }

            }
            return itemData;
        }

        public void OnFindClick()
        {
            var name = SearchItemField.GetComponent<TMP_InputField>().text;
            ActualSelectedItem = FindClosestItem(name, out var selectedGroup);
            //MapCanvasController.SetPrefab(ActualSelectedItem);
            SetActiveGroupView(selectedGroup);

            ChangeActualItemView(ActualSelectedItem);
        }

        public void OnSelectorValueChanged()
        {
            var selectedValue = GroupViewSelector.value;
            var selectedGroup = GroupViewSelector.options[selectedValue].text;
            if(!GroupViews.ContainsKey(selectedGroup)) 
            {
                //TODO: Exception handling
                return;
            }
            SetActiveGroupView(GroupViews[selectedGroup]);
        }

        private void ChangeActualItemView(ItemData item)
        {
            //TODO: Exception
            var image = ActualSelectedItemView.GetComponent<Image>();
            image.sprite = item.GetImage();
        }

        private void SetActiveGroupView(GameObject group)
        {
            activeGroup.SetActive(false);
            activeGroup = group;
            activeGroup.SetActive(true);
        }
    }
}

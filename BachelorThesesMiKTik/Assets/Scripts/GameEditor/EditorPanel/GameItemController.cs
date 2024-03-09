using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        public Dictionary<string, ItemGroupViewController> GroupViews {  get; private set; }
        public List<ItemData> Items { get; private set; }
        public Dictionary<string, int> ItemsNameIdPair { get; private set; }

        private ItemGroupViewController activeGroup;

        private void Start()
        {
            GroupViewSelector.onValueChanged.AddListener( delegate { OnSelectorValueChanged(); });
            ActualSelectedItem = DefaultItems[0];
            GroupViews = new Dictionary<string, ItemGroupViewController>();
            Items = new List<ItemData>();
            ItemsNameIdPair = new Dictionary<string, int>();

            foreach (var item in DefaultItems) 
            {
                AddItem(item);
            }

            activeGroup = GroupViews.Values.First();
            activeGroup.gameObject.SetActive(true);
        }

        public void AddItem(ItemData item)
        {
            AddToGroup(item);
            item.Id = Items.Count;
            Items.Add(item);
            ItemsNameIdPair.Add(item.ShownName, item.Id);
        }

        public void EditActualSelectedItem(ItemData oldData, ItemData newData)
        {
            EditItem(oldData, newData);
            ActualSelectedItem = newData;
        }

        public void EditItem(ItemData oldItem, ItemData newItem)
        {
            if(ItemsNameIdPair.ContainsKey(oldItem.ShownName)) 
            {
                var id = ItemsNameIdPair[oldItem.ShownName];
                ItemsNameIdPair.Remove(oldItem.ShownName);
                ItemsNameIdPair.Add(newItem.ShownName, id);

                GroupViews[oldItem.GroupName].RemoveItemButton(oldItem);
                AddToGroup(newItem);

                newItem.Id = id;
                Destroy(Items[id].Prefab);
                Items[id] = newItem;

                var group = MapCanvas.Instance.GetGroup(id).Keys.ToList();
                foreach (var itemPosition in group) 
                {
                    MapCanvas.Instance.ReplaceItem(newItem, itemPosition);
                }
            }
        }

        public UnityAction ViewPlanelButtonClick(ItemData item)
        {
            ActualSelectedItem = item;
            ChangeActualItemView(item);
            return null;
        }

        public bool TryFindIdByName(string itemName, out int id)
        {
            if (ItemsNameIdPair.ContainsKey(itemName))
            {
                id = ItemsNameIdPair[itemName];
                return false;
            }
                

            id = ItemsNameIdPair[itemName];
            return true;
        }

        public void OnFindClick()
        {
            var name = SearchItemField.GetComponent<TMP_InputField>().text;
            ActualSelectedItem = FindClosestItemName(name, out var selectedGroup);
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

        private void AddToGroup(ItemData newItem)
        {
            var groupName = newItem.GroupName;
            if (!GroupViews.ContainsKey(groupName))
            {
                var group = Instantiate(ViewPanelPrefab, transform).GetComponent<ItemGroupViewController>();
                group.gameObject.SetActive(false);

                GroupViews.Add(groupName, group);
                GroupViewSelector.AddOptions(new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData(groupName) });
            }

            var controller = GroupViews[groupName];
            controller.AddItemButton(newItem, ViewPlanelButtonClick);
        }

        private ItemData FindClosestItemName(string name, out ItemGroupViewController itemGroup)
        {
            itemGroup = GroupViews.Values.First();
            ItemData itemData = Items[0];
            int minDistance = int.MaxValue;

            foreach (var group in GroupViews)
            {
                ;
                var item = group.Value.FindClosestItem(name, out var distance);
                if (minDistance > distance)
                {
                    itemGroup = group.Value;
                    itemData = item;
                    minDistance = distance;
                }

            }
            return itemData;
        }

        private void ChangeActualItemView(ItemData item)
        {
            //TODO: Exception
            var image = ActualSelectedItemView.GetComponent<Image>();
            image.sprite = item.GetImage();
        }

        private void SetActiveGroupView(ItemGroupViewController group)
        {
            activeGroup.gameObject.SetActive(false);
            activeGroup = group;
            activeGroup.gameObject.SetActive(true);
        }
    }
}

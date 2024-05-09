using Assets.Scripts.GameEditor.Managers;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.EditorPanels
{
    public class ItemViewController : MonoBehaviour
    {
        [SerializeField] public GameObject ViewPanelPrefab;
        [SerializeField] public GameObject ActualSelectedItemView;
        [SerializeField] public TMP_InputField SearchItemField;
        [SerializeField] public TMP_Dropdown GroupViewSelector;

        public Dictionary<string, ItemGroupViewController> GroupViews {  get; private set; }

        private ItemGroupViewController activeGroup;
        private PrototypeManager manager;

        /// <summary>
        /// Adds new item to group view.
        /// </summary>
        /// <param name="item"></param>
        public void Add(ItemData item)
        {
            AddToGroup(item);
        }

        /// <summary>
        /// Removes all groupviews;
        /// default values.
        /// </summary>
        public void Clear()
        {
            GroupViewSelector.options.Clear();
            foreach (var groupView in GroupViews.Values) 
            {
                Destroy(groupView.gameObject);
            }

            GroupViews.Clear();
            activeGroup = null;
        }

        /// <summary>
        /// Replaces old item view with new one (if old item exists and it is not default).
        /// The old is destroyed.
        /// </summary>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
        public void Edit(ItemData oldItem, ItemData newItem)
        {
            if(GroupViews.ContainsKey(oldItem.GroupName)) 
            {
                GroupViews[oldItem.GroupName].RemoveItemButton(oldItem);
                AddToGroup(newItem);
            }
        }

        public void Initialize()
        {
            GroupViewSelector.onValueChanged.AddListener(OnSelectorValueChanged);
            GroupViews = new Dictionary<string, ItemGroupViewController>();
            manager = PrototypeManager.Instance;
        }

        /// <summary>
        /// Changes actual item and group preview to actual selected item in ItemManager.
        /// </summary>
        public void SetActualItem()
        {
            ChangeActualItemView(manager.ActualSelectedItem);
            var groupName = manager.ActualSelectedItem.GroupName;
            if (GroupViews.ContainsKey(groupName)) 
            {
                for(int i = 0; i < GroupViewSelector.options.Count; i++)
                {
                    if (GroupViewSelector.options[i].text == groupName)
                    {
                        GroupViewSelector.value = i;

                        if (activeGroup == null)
                            SetActiveGroupView(GroupViews[groupName]);
                    }
                }
            }
        }

        /// <summary>
        /// Destroys given item (removes it from group view) based on name (if exists and it is not default).
        /// </summary>
        /// <param name="item">Item to be removed.</param>
        public void Remove(ItemData item)
        {
            if(GroupViews.ContainsKey(item.GroupName))
                GroupViews[item.GroupName].RemoveItemButton(item);
        }

        /// <summary>
        /// Changes actual selected item to given item and changes actual item preview.
        /// </summary>
        /// <param name="item">New selected item.</param>
        /// <returns></returns>
        public UnityAction ViewPlanelButtonClick(ItemData item)
        {
            PrototypeManager.Instance.ActualSelectedItem = item;
            ChangeActualItemView(item);
            return null;
        }

        /// <summary>
        /// Handles Find button click. Based on name in search field finds group
        /// item with closest name and sets it as actual selected item and his group as
        /// actual group view.
        /// </summary>
        public void OnFindClick()
        {
            if (SearchItemField.text != "")
            {
                manager.ActualSelectedItem = FindClosestItemName(SearchItemField.text, out var selectedGroup);
                SetActiveGroupView(selectedGroup);
                ChangeActualItemView(manager.ActualSelectedItem);
            }
        }

        /// <summary>
        /// Handles group view dropdown change. Changes actual group view based on selected
        /// name.
        /// </summary>
        public void OnSelectorValueChanged(int value)
        {
            var selectedGroup = GroupViewSelector.options[value].text;
            
            if (!GroupViews.ContainsKey(selectedGroup))
                OutputManager.Instance.ShowMessage("Group cannot be displayed! Invalid name!");
            else
                SetActiveGroupView(GroupViews[selectedGroup]);
        }

        #region PRIVATE
        /// <summary>
        /// Finds group view based on given item group name and adds item to this group.
        /// If there is not group view with given name, new group is created.
        /// </summary>
        /// <param name="newItem">Item we want to add to group.</param>
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

        /// <summary>
        /// Based on given name of item, this method will loop thru all item groups and
        /// and finds group item with closest Levenstein distance.
        /// </summary>
        /// <param name="name">Given name.</param>
        /// <param name="itemGroup">Group with closest item.</param>
        /// <returns>Item with smallest Levenstein distance.</returns>
        private ItemData FindClosestItemName(string name, out ItemGroupViewController itemGroup)
        {
            itemGroup = GroupViews.Values.First();
            ItemData itemData = manager.Items.Values.First();
            int minDistance = int.MaxValue;

            foreach (var group in GroupViews)
            {
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

        /// <summary>
        /// Extracts the preview image of given item and selects it as actual
        /// item preview in editor.
        /// </summary>
        /// <param name="item"></param>
        private void ChangeActualItemView(ItemData item)
        {
            var image = ActualSelectedItemView.GetComponent<Image>();
            image.sprite = item.GetImage();
        }

        /// <summary>
        /// Disables actual group view and replaces it with given view.
        /// </summary>
        /// <param name="group"></param>
        private void SetActiveGroupView(ItemGroupViewController group)
        {
            if (activeGroup != null)
                activeGroup.gameObject.SetActive(false);

            activeGroup = group;
            activeGroup.gameObject.SetActive(true);
        }

        #endregion
    }
}

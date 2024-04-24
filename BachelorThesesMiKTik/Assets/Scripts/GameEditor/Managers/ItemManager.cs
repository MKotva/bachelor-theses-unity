using Assets.Scripts.GameEditor.EditorPanels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Managers
{
    public class ItemManager : Singleton<ItemManager>
    {
        [SerializeField] private List<ItemDataSource> DefaultItems;
        [SerializeField] private ItemViewController itemController;

        public ItemData ActualSelectedItem { get; set; }
        public Dictionary<string, List<ItemData>> Groups { get; private set; }
        public Dictionary<int, ItemData> Items { get; private set; }
        public Dictionary<string, int> ItemsNameIdPair { get; private set; }

        private HashSet<string> defaultItems;

        /// <summary>
        /// Adds new item to and all required data structures (Group, Items, ItemNameIdPair).
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(ItemData item)
        {
            if (!Items.ContainsKey(item.Id))
            {
                AddToGroup(item);
                Items.Add(item.Id, item);
                ItemsNameIdPair.Add(item.ShownName, item.Id);
            }

            if(itemController != null)
                itemController.Add(item);
        }

        /// <summary>
        /// Removes all created items and reinitializes manager (and item view controller if is present) 
        /// with default values.
        /// </summary>
        public void ClearItems()
        {
            foreach (var item in Items.Values)
            {
                if (!defaultItems.Contains(item.ShownName))
                    Destroy(item.Prefab);
            }

            Groups.Clear();
            Items.Clear();
            ItemsNameIdPair.Clear();

            if (itemController != null)
            {
                itemController.Clear();
                Initialize();
                itemController.SetActualItem();
            }
            else
            {
                Initialize();
            }
        }

        /// <summary>
        /// Replaces actual selected item with new one (edit).
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        public void EditActualSelectedItem(ItemData oldData, ItemData newData)
        {
            EditItem(oldData, newData);
            ActualSelectedItem = newData;
        }

        /// <summary>
        /// Replaces old item with new one (if old item exists and it is not default).
        /// The old is destroyed.
        /// </summary>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
        public void EditItem(ItemData oldItem, ItemData newItem)
        {
            if (CheckIfItemIsDefault(oldItem.ShownName))
            {
                ErrorOutputManager.Instance.ShowMessage("You cannot edit default item.");
                return;
            }

            if (ItemsNameIdPair.ContainsKey(oldItem.ShownName))
            {
                if (itemController != null)
                    itemController.Edit(oldItem, newItem);

                var id = ItemsNameIdPair[oldItem.ShownName];
                var positions = EditorCanvas.Instance.GetGroup(id).Keys.ToList();
                
                RemoveItem(oldItem);
                AddItem(newItem);

                foreach (var itemPosition in positions)
                {
                    EditorCanvas.Instance.Paint(newItem, itemPosition);
                }

                ChangeActual(oldItem, newItem);
            }
        }

        /// <summary>
        /// Destroys given item (removes it from Group,Items and ItemIdPair) based on name (if exists and it is not default).
        /// </summary>
        /// <param name="item">Item to be removed.</param>
        public void RemoveItem(ItemData item)
        {
            if (CheckIfItemIsDefault(item.ShownName))
            {
                ErrorOutputManager.Instance.ShowMessage("You cannot delete default item.");
                return;
            }

            if (ItemsNameIdPair.ContainsKey(item.ShownName))
            {
                if (itemController != null)
                    itemController.Remove(item);

                var id = ItemsNameIdPair[item.ShownName];
                ItemsNameIdPair.Remove(item.ShownName);
                
                if (!defaultItems.Contains(item.ShownName))
                    Destroy(item.Prefab);
                
                Items.Remove(id);
                EditorCanvas.Instance.RemoveGroupFromData(id);
                ChangeActual(item, Items.Values.First());
            }
        }

        /// <summary>
        /// Checks if item is default (non editable) 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CheckIfItemIsDefault(string name)
        {
            return defaultItems.Contains(name);
        }

        /// <summary>
        /// Finds item id based on given name.
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="id">Out parameter with item id.</param>
        /// <returns>True if </returns>
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

        protected override void Awake()
        {
            base.Awake();

            Groups = new Dictionary<string, List<ItemData>>();
            Items = new Dictionary<int, ItemData>();
            ItemsNameIdPair = new Dictionary<string, int>();

            if (itemController != null)
            {
                itemController.Initialize();
                Initialize();
                itemController.SetActualItem();
            }
            else
            {
                Initialize();
            }
        }

        /// <summary>
        /// Adds item to group (if group exists). Otherwise creates new group.
        /// </summary>
        /// <param name="item"></param>
        private void AddToGroup(ItemData item) 
        {
            if(Groups.ContainsKey(item.GroupName)) 
            {
                Groups[item.GroupName].Add(item);
            }
            else
            {
                Groups.Add(item.GroupName, new List<ItemData>());
            }
        } 

        /// <summary>
        /// Changes actual selected item. If item view controller present, invokes
        /// change of actual item view.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="changeTo"></param>
        private void ChangeActual(ItemData item, ItemData changeTo)
        {
            if(item == ActualSelectedItem) 
            {
                ActualSelectedItem = changeTo;
                if(itemController != null)
                {
                    itemController.SetActualItem();
                }    
            }
        }

        /// <summary>
        /// Initializes GameItemController with default items -> non editable!
        /// </summary>
        private void Initialize()
        {
            var listNames = new List<string>();
            foreach (var item in DefaultItems)
            {
                AddItem(new ItemData(item));
                listNames.Add(item.Name);
            }

            defaultItems = new HashSet<string>(listNames);
            ActualSelectedItem = Items[ItemsNameIdPair["Grass"]];
        }
    }
}

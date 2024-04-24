using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.EditorPanels
{
    public class ItemGroupViewController : MonoBehaviour
    {
        [SerializeField] public GameObject ButtonPrefab;
        [SerializeField] public GameObject ParentObject;

        public delegate UnityAction ItemButtonClickHandler(ItemData item);

        private Dictionary<ItemData, Button> ItemButtons;

        // Start is called before the first frame update
        void Awake()
        {
            ItemButtons = new Dictionary<ItemData, Button>();
        }

        /// <summary>
        /// Cretes new button with item image, sets onClick action handler and adds it to group view.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="clickHandler"></param>
        public void AddItemButton(ItemData item, ItemButtonClickHandler clickHandler)
        {
            if(!ItemButtons.ContainsKey(item)) 
            {
                Button button = Instantiate(ButtonPrefab, ParentObject.transform).GetComponent<Button>();
                button.image.sprite = item.GetImage();
                button.transform.name = item.ShownName;

                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = item.ShownName;

                button.onClick.AddListener(delegate { clickHandler(item); });

                ItemButtons.Add(item, button);
            }
        }

        /// <summary>
        /// Removes item button from group view.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItemButton(ItemData item) 
        {
            if(ItemButtons.ContainsKey(item)) 
            {
                Destroy(ItemButtons[item].gameObject);
                ItemButtons.Remove(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="minDistance"></param>
        /// <returns></returns>
        public ItemData FindClosestItem(string name, out int minDistance)
        {
            minDistance = int.MaxValue;

            ItemData closestItem = ItemButtons.Keys.First();
            foreach (var item in ItemButtons)
            {
                if (item.Key.ShownName == name)
                {
                    minDistance = 0;
                    return item.Key;
                }

                var distnace = LevenshteinDistance(item.Key.ShownName, name);
                if (minDistance > distnace)
                {
                    minDistance = distnace;
                    closestItem = item.Key;
                }
            }

            return closestItem;
        }

        //TODO:Check the requirement for this to be legit reference.
        /// <summary>
        /// This method calculates Levenshtein distance of strings s and t.
        /// This is was obtained from: https://stackoverflow.com/questions/13793560/find-closest-match-to-input-string-in-a-list-of-strings
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++)
                ;
            for (int j = 0; j <= m; d[0, j] = j++)
                ;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = ( t[j - 1] == s[i - 1] ) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }
}

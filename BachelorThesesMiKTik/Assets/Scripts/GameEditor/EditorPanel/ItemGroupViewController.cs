using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.ItemView
{
    public class ItemGroupViewController : MonoBehaviour
    {
        [SerializeField] public GameObject ButtonPrefab;
        [SerializeField] public GameObject ParentObject;

        public delegate UnityAction ItemButtonClickHandler(ItemData item);

        private Dictionary<ItemData, Button> Items;

        // Start is called before the first frame update
        void Awake()
        {
            Items = new Dictionary<ItemData, Button>();
        }

        public void AddItemButton(ItemData item, ItemButtonClickHandler clickHandler)
        {
            if(Items.ContainsKey(item)) 
            {
                //TODO: Exception handle;
                return;
            }

            Button button = Instantiate(ButtonPrefab, ParentObject.transform).GetComponent<Button>(); 
            button.image.sprite = item.GetImage();
            button.transform.name = item.ShownName;

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = item.ShownName;

            button.onClick.AddListener(delegate { clickHandler(item); });

            Items.Add(item, button);
        }

        public void RemoveItemButton(ItemData item) 
        {
            if(Items.ContainsKey(item)) 
            {
                Destroy(Items[item].gameObject);
                Items.Remove(item);
            }
        }


        public ItemData FindClosestItem(string name, out int minDistance)
        {
            minDistance = int.MaxValue;

            ItemData closestItem = Items.Keys.First();
            foreach (var item in Items)
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
                }
            }

            return closestItem;
        }

        //TODO:Check the requirement for this to be legit. This is code from StackOverlow
        //https://stackoverflow.com/questions/13793560/find-closest-match-to-input-string-in-a-list-of-strings
        //!!!
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

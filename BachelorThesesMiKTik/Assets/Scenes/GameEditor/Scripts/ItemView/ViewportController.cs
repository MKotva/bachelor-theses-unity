using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ViewportController : MonoBehaviour
{
    [SerializeField] public GameObject Grid;
    [SerializeField] public GameObject buttonPrefab;
    [SerializeField] public GameObject parentObject;
    [SerializeField] public GameObject viewBox;

    public List<ItemData> itemDatas;

    private Dictionary<ItemData, Button> _itemInstances;
    private GridController _controller;

    // Start is called before the first frame update
    void Awake()
    {
        _controller = Grid.GetComponent<GridController>();
        _itemInstances = new Dictionary<ItemData, Button>();

        foreach (var item in itemDatas)
        {
            Button button = Instantiate(buttonPrefab, parentObject.transform).GetComponent<Button>();
            button.image.sprite = item.Prefab.GetComponent<SpriteRenderer>().sprite;

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = item.ShownName;

            MenuButtonHandler handler = button.GetComponent<MenuButtonHandler>();
            handler.SetBuildingItem(item, _controller, viewBox);

            _itemInstances.Add(item, button);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool SelectBySearch(string name, out int minDistance)
    {
        minDistance = int.MaxValue;
        foreach(var item in _itemInstances)
        {
            if (item.Key.ShownName == name)
            {
                var controller = item.Value.GetComponent<MenuButtonHandler>();
                controller.SetActualItemPreview();
                return true;
            }

            var distnace = LevenshteinDistance(item.Key.ShownName, name);
            if(minDistance > distnace)
            {
                minDistance = distnace;
            }
        }
        return false;
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

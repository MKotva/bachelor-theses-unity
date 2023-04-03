using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ViewportController : MonoBehaviour
{
  public GameObject buttonPrefab;
  public GameObject parentObject;
  public List<ItemData> itemDatas;
  GameObject[] prefabs;

  // Start is called before the first frame update
  void Awake()
  {
    foreach(var item in itemDatas) 
    {
      Button button = Instantiate(buttonPrefab, parentObject.transform).GetComponent<Button>();
      button.image.sprite = item.Prefab.GetComponent<SpriteRenderer>().sprite;
      
      TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
      buttonText.text = item.ShownName;

      MenuButtonHandler handler = button.GetComponent<MenuButtonHandler>();
      handler.SetBuildingItem(item);
    }
  }

  // Update is called once per frame
  void Update()
  {
  }
}

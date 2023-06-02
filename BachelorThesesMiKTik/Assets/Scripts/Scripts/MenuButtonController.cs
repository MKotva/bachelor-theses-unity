using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  #region [SerializeField] public string SceneToBeLoaded { get; set; }

  /// <summary>
  /// Defines scene to be loaded
  /// </summary>
  [field: Tooltip("Defines scene to be loaded")]
  [field: SerializeField]
  public string SceneToBeLoaded { get; set; }

  #endregion


  [SerializeField] private AudioClip _compressClip, _uncompressClip;
  [SerializeField] private AudioSource _source;
  public void OnPointerDown(PointerEventData eventData)
  {
    gameObject.GetComponent<Image>().color = new Color(70, 70, 70, 255);
    _source.PlayOneShot(_compressClip);
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
    _source.PlayOneShot(_uncompressClip);
  }

  public void HandleClick()
  {
    SceneManager.LoadScene(sceneName: SceneToBeLoaded);
  }
}

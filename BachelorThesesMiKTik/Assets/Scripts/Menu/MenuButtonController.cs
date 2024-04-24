using Assets.Core.GameEditor.Serializers;
using Assets.Scenes.GameEditor.Core.DTOS;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private AudioClip PressClip, ReleaseClip;
    [SerializeField] private AudioSource AudioSource;

    private Color originalColor;

    private void Awake()
    {
        originalColor = gameObject.GetComponent<Image>().color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().color = new Color(70, 70, 70, 255);
        AudioSource.PlayOneShot(PressClip);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().color = originalColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanelButtonController : MonoBehaviour
{
    [SerializeField] public GameObject MainPanel;
    [SerializeField] public GameObject PanelToShow;

    private MainPanelController _controller;

    private void Awake()
    {
        _controller = MainPanel.GetComponent<MainPanelController>();
    }

    public void OnClick()
    {
        _controller.ShowPanel(PanelToShow);
    }
}

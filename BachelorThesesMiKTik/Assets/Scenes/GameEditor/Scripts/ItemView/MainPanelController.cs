using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanelController : MonoBehaviour
{
    private GameObject _activePanel;

    private void Awake()
    {
        _activePanel = null;
    }

    public void ShowPanel(GameObject panel)
    {
        if(_activePanel != null) 
        {
            _activePanel.SetActive(false);
        }
        _activePanel = panel;
        _activePanel.SetActive(true);
    }
}

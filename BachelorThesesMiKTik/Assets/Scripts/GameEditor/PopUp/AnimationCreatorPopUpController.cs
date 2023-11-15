using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AnimationCreatorPopUpController : PopUpController
{
    [SerializeField] public GameObject LinePrefab;
    [SerializeField] public GameObject ContentView;

    public delegate void OnExitCall();

    private Stack<GameObject> lines;
    private List<OnExitCall> exitCalls;

    private void Awake()
    {
        lines = new Stack<GameObject>();
        exitCalls = new List<OnExitCall>();
    }

    public override void OnExitClick() {}

    public void OnAddClick()
    {
        lines.Push(Instantiate(LinePrefab, ContentView.transform));
    }

    public void OnRemoveClick()
    {
        if (lines.Count > 0)
        {
            var line = lines.Pop();
            Destroy(line);
        }
    }

    public void OnClearClick()
    {
        foreach (var line in lines)
            Destroy(line);

        lines.Clear();
    }    

    public void OnCreateClick()
    {
        foreach (var callback in exitCalls)
        {
            callback.Invoke();
        }
    }

    public void SetCallback(OnExitCall callbackFunction)
    {
        exitCalls.Add(callbackFunction);
    }

    public List<AnimationFrameDTO> GetData()
    {
        var data = new List<AnimationFrameDTO>();
        foreach (var line in lines.Reverse())
        {
            var displayTime = line.transform.GetChild(0).GetComponentInChildren<TMP_InputField>().text;
            var URL = line.transform.GetChild(1).GetComponentInChildren<TMP_InputField>().text;

            if(double.TryParse(displayTime, out var time))
                data.Add(new AnimationFrameDTO(time, URL));
        }

        return data;
    }

    public void SetData(List<AnimationFrameDTO> data)
    {
        foreach(var item in data)
        {
            var line = Instantiate(LinePrefab, ContentView.transform);
            line.transform.GetChild(0).GetComponentInChildren<TMP_InputField>().text = item.DisplayTime.ToString();
            line.transform.GetChild(1).GetComponentInChildren<TMP_InputField>().text = item.URL;

            lines.Push(line);
        }
    }
}

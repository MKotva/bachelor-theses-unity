using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Controllers;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SingleImageComponentController : ObjectComponent
{
    [SerializeField] TMP_InputField XSizeField;
    [SerializeField] TMP_InputField YSizeField;
    [SerializeField] GameObject SourcePanel;

    private SourcePanelController sourcePanelController;

    private void Start()
    {
        sourcePanelController = SourcePanel.GetComponent<SourcePanelController>();
    }

    public override async Task SetItem(ItemData item)
    {
        var sourceDTO = sourcePanelController.GetData();
        var rendered = item.Prefab.GetComponent<SpriteRenderer>();

        switch (sourceDTO.Type) 
        {
            case SourceType.Image:
                await SpriteLoader.SetSprite(item.Prefab, sourceDTO.URL, GetSize(XSizeField), GetSize(YSizeField)); //TODO: Change values to some real stuff, this is just placeholder.
                break;
            case SourceType.Animation:
                await AnimationLoader.SetAnimation(item.Prefab, ( (AnimationSourceDTO) sourceDTO ).AnimationData, GetSize(XSizeField), GetSize(YSizeField)); //TODO: Change values to some real stuff, this is just placeholder.
                rendered.sprite = item.Prefab.GetComponent<CustomAnimationController>().GetAnimationPreview();
                break;
            case SourceType.Video:
                //TODO: Video loading
                break;
        }

        item.ShownImage = rendered.sprite;
        //TODO: Exception handle
    }

    private uint GetSize(TMP_InputField field)
    {
        try
        {
            return Convert.ToUInt32(field.text);
        }
        catch
        {
            //TODO: Exception
            return 0;
        }
    }
}

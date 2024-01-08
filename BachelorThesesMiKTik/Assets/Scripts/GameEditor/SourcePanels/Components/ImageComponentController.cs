using Assets.Core.GameEditor.DTOS.Components;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ImageComponentController : ObjectComponent
{
    [SerializeField] TMP_InputField XSizeField;
    [SerializeField] TMP_InputField YSizeField;
    [SerializeField] SourcePanelController SourcePanel;

    public override void SetComponent(ComponentDTO component)
    {
        if (component is ImageComponentDTO)
        {
            var imageComponent = (ImageComponentDTO) component;
            SourcePanel.SetData(imageComponent.Data);
            XSizeField.text = imageComponent.XSize.ToString();
            YSizeField.text = imageComponent.YSize.ToString();
        }
        else
        {
            InfoPanelController.Instance.ShowMessage("Image component parsing error!");
        }
    }

    public override async Task<ComponentDTO> GetComponent()
    {
        return CreateComponent();
    }

    #region PRIVATE
    private ComponentDTO CreateComponent()
    {
        var sourceDTO = SourcePanel.GetData();
        var xSize = GetSize(XSizeField);
        var ySize = GetSize(YSizeField);
        return new ImageComponentDTO(xSize, ySize, sourceDTO);
    }

    private uint GetSize(TMP_InputField field)
    {
        try
        {
            return Convert.ToUInt32(field.text);
        }
        catch
        {
            return 30;
        }
    }
    #endregion
}

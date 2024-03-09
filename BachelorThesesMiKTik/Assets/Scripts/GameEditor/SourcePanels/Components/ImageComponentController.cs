using Assets.Core.GameEditor;
using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Components;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ImageComponentController : ObjectComponent
{
    [SerializeField] TMP_InputField XSizeField;
    [SerializeField] TMP_InputField YSizeField;
    [SerializeField] AssetSourcePanelController SourcePanel;
    [SerializeField] GameObject PreviewObject;

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
        return await Task.Run(() => CreateComponent());
    }

    public async void OnPreviewPress()
    {
        var component = (ImageComponentDTO)CreateComponent();
        if(component.Data.Type == SourceType.Image)
        {
            await SpriteLoader.SetSprite(PreviewObject, ((SourceDTO)component.Data).URL);
        }
        else
        {
            await AnimationLoader.SetAnimation(PreviewObject, (AnimationSourceDTO)component.Data, true, true);
        }
    }

    #region PRIVATE
    private ComponentDTO CreateComponent()
    {
        var sourceDTO = SourcePanel.GetData();
        var xSize = MathHelper.GetUInt(XSizeField.text, 30, "Image x Size", "Object creator");
        var ySize = MathHelper.GetUInt(YSizeField.text, 30, "Image y Size", "Object creator");
        return new ImageComponentDTO(xSize, ySize, sourceDTO);
    }


    #endregion
}

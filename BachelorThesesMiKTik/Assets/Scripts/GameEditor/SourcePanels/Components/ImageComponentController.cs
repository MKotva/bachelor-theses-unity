using Assets.Core.GameEditor.Components;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Controllers;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.PopUp.CodeEditor;
using Assets.Scripts.GameEditor.SourcePanels;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using UnityEngine;
using UnityEngine.UI;

public class ImageComponentController : ObjectComponent
{
    [SerializeField] AssetPanelController SourcePanel;
    [SerializeField] GameObject PreviewObject;

    public override void SetComponent(CustomComponent component)
    {
        if (component is ImageComponent)
        {
            var imageComponent = (ImageComponent) component;
            SourcePanel.SetData(imageComponent.Data);
        }
        else
        {
            ErrorOutputManager.Instance.ShowMessage("Image component parsing error!", "ObjectCreate");
        }
    }

    public override CustomComponent GetComponent()
    {
        return CreateComponent();
    }

    #region PRIVATE
    private void Awake()
    {
        PreviewManager.Instance.previewGetter = SourcePanel.GetData;
        SourcePanel.AnimationsDropdown.onValueChanged.AddListener(OnAnimationValueChange);
        SourcePanel.SpritesDropdown.onValueChanged.AddListener(OnSpriteValueChange);
        SourcePanel.XSize.onEndEdit.AddListener(OnEditEnd);
        SourcePanel.YSize.onEndEdit.AddListener(OnEditEnd);
    }

    private void OnEditEnd(string _) 
    {
        if (SourcePanel.SourceT.value == 0)
        {
            OnSpriteValueChange(SourcePanel.SpritesDropdown.value);
        }
        else
        {
            OnAnimationValueChange(SourcePanel.AnimationsDropdown.value);
        }
    }

    private void OnAnimationValueChange(int id)
    {
        var value = SourcePanel.AnimationsDropdown.options[id].text;
        if(value == "None" || value == "Create")
        {
            if(TryGetComponent<AnimationsController>(out var controller)) 
            {
                controller.RemoveAnimation();
            }
            return;
        }

        var source = SourcePanel.GetData();
        var instance = AnimationsManager.Instance;
        if (instance != null)
            instance.SetAnimation(PreviewObject, source);

        ScalePreview(source);
    }

    private void OnSpriteValueChange(int id)
    {
        var value = SourcePanel.SpritesDropdown.options[id].text;
        if (value == "None" || value == "Create")
        {
            PreviewObject.GetComponent<Image>().sprite = null;
            return;
        }

        var instance = SpriteManager.Instance;
        if (instance != null)
        {
            if(instance.Sprites.ContainsKey(value))
            {
                var source = SourcePanel.GetData();
                PreviewObject.GetComponent<Image>().sprite = SpriteManager.Instance.Sprites[source.Name];
                ScalePreview(source);
            }
        }
    }

    public void ScalePreview(SourceDTO source)
    {
        var preview = PreviewObject.GetComponent<Image>();
        var transform = preview.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(source.XSize, source.YSize);

        var xScale = 1f;
        if (source.XSize > 140)
            xScale = 140f / source.XSize;

        var yScale = 1f;
        if (source.YSize > 100)
            yScale = 100f / source.YSize;

        if (yScale >= xScale)
        {
            transform.localScale = new Vector2(xScale, xScale);
        }
        else
        {
            transform.localScale = new Vector2(yScale, yScale);
        }

        var previewManager = PreviewManager.Instance;
        if(previewManager != null) 
        {
            previewManager.ChangePreview(source);
        }
    }

    private CustomComponent CreateComponent()
    {
        var sourceDTO = SourcePanel.GetData();
        return new ImageComponent(sourceDTO);
    }


    #endregion
}

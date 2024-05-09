using Assets.Core.GameEditor.Components;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Controllers;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.PopUp.CodeEditor;
using Assets.Scripts.GameEditor.SourcePanels;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ImageComponentController : ObjectComponent
{
    [SerializeField] AssetPanelController SourcePanel;
    [SerializeField] GameObject PreviewObject;


    private Image previewImage;
    private AnimationsController animationControler;

    /// <summary>
    /// Sets component panel based on given component class.
    /// </summary>
    /// <param name="component"></param>
    public override void SetComponent(CustomComponent component)
    {
        if (component is VisualComponent)
        {
            var imageComponent = (VisualComponent) component;
            SourcePanel.SetData(imageComponent.Data);
        }
        else
        {
            OutputManager.Instance.ShowMessage("Image component parsing error!", "ObjectCreate");
        }
    }

    /// <summary>
    /// Scales selected sprite proportionaly to fit in preview image of this
    /// component panel.
    /// </summary>
    /// <param name="source"></param>
    public void ScalePreview(SourceReference source)
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
    }

    /// <summary>
    /// Returns component new class from data, in component panel. 
    /// </summary>
    /// <returns></returns>
    public override CustomComponent GetComponent()
    {
        return CreateComponent();
    }

    #region PRIVATE
    private void Awake()
    {
        previewImage = PreviewObject.GetComponent<Image>();
        animationControler = PreviewObject.GetComponent<AnimationsController>();

        PreviewManager.Instance.previewGetter = SourcePanel.GetData;
        SourcePanel.AnimationsDropdown.onValueChanged.AddListener(OnAnimationValueChange);
        SourcePanel.SpritesDropdown.onValueChanged.AddListener(OnSpriteValueChange);
        SourcePanel.XSize.onEndEdit.AddListener(OnEditEnd);
        SourcePanel.YSize.onEndEdit.AddListener(OnEditEnd);
    }

    private void OnDestroy()
    {
        ChangePreview(null);
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

    /// <summary>
    /// Handles animation menu dropdown value. If value is not default, changes
    /// actual preview image to animation preview image.
    /// </summary>
    /// <param name="id"></param>
    private void OnAnimationValueChange(int id)
    {
        var value = SourcePanel.AnimationsDropdown.options[id].text;
        if(IsDefaultValue(value))
        {
            animationControler.RemoveAnimation();
        }

        var source = SourcePanel.GetData();
        var instance = AnimationsManager.Instance;
        if (instance != null)
        {
            if (!instance.ContainsName(source.Name))
                return;


            animationControler.SetCustomAnimation(instance.Animations[source.Name], source, true, true);

            ScalePreview(source);
            ChangePreview(source);
        }
    }   

    /// <summary>
    /// Handles sprite menu dropdown value. If value is not default, changes
    /// actual preview image.
    /// </summary>
    /// <param name="id"></param>
    private void OnSpriteValueChange(int id)
    {
        if (PreviewObject.TryGetComponent(out AnimationsController control))
        {
            control.Stop();
        }

        var value = SourcePanel.SpritesDropdown.options[id].text;

        var instance = SpriteManager.Instance;
        if (instance != null)
        {
            if(!instance.Sprites.ContainsKey(value))
            {
                return;
            }

            var source = SourcePanel.GetData();
            previewImage.sprite = SpriteManager.Instance.Sprites[source.Name];
            ScalePreview(source);
            ChangePreview(source);
        }
    }

    /// <summary>
    /// Invokes preview manager change.
    /// </summary>
    /// <param name="source"></param>
    private void ChangePreview(SourceReference source)
    {
        var previewManager = PreviewManager.Instance;
        if (previewManager != null)
        {
            previewManager.ChangePreview(source);
        }
    }

    /// <summary>
    /// Creates component class based on data in component panel.
    /// </summary>
    /// <returns></returns>
    private CustomComponent CreateComponent()
    {
        var sourceDTO = SourcePanel.GetData();
        return new VisualComponent(sourceDTO);
    }

    private bool IsDefaultValue(string value)
    {
        if (value == "None" || value == "Create")
        {
            previewImage.sprite = null;
            ChangePreview(null);
            return true;
        }
        return false;
    }
    #endregion
}

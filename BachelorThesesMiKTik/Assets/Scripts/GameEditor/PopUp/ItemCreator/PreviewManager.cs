using Assets.Core.GameEditor.DTOS.Assets;

namespace Assets.Scripts.GameEditor.PopUp.CodeEditor
{
    public class PreviewManager : Singleton<PreviewManager>
    {
        public delegate SourceDTO PreviewGetter();
        public delegate void PreviewChangeHangler(SourceDTO source);
        public event PreviewChangeHangler onPreviewChange;

        public PreviewGetter previewGetter;

        public void ChangePreview(SourceDTO source)
        {
            if(onPreviewChange != null) 
                onPreviewChange.Invoke(source);
        }

        public SourceDTO GetPreview()
        {
            if (previewGetter != null)
                return previewGetter.Invoke();
            return null;
        }

        protected override void Awake()
        {
            base.Awake();
        }
    }
}

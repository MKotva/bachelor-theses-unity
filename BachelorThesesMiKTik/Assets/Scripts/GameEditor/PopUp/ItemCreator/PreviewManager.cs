using Assets.Core.GameEditor.DTOS.Assets;

namespace Assets.Scripts.GameEditor.PopUp.CodeEditor
{
    public class PreviewManager : Singleton<PreviewManager>
    {
        public delegate SourceReference PreviewGetter();
        public delegate void PreviewChangeHangler(SourceReference source);
        public event PreviewChangeHangler onPreviewChange;

        public PreviewGetter previewGetter;

        public void ChangePreview(SourceReference source)
        {
            if(onPreviewChange != null) 
                onPreviewChange.Invoke(source);
        }

        public SourceReference GetPreview()
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

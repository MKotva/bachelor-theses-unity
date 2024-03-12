using Assets.Core.GameEditor.Animation;

namespace Assets.Core.GameEditor.AnimationControllers
{
    public interface IAnimator
    {
        public void Animate(float timeDelta);
        public void PauseAnimation();
        public void ResumeAnimation();
        public void ResetAnimation();
        public bool HasFinished();
        public bool Animating();
        public float GetXScaling();
        public float GetYScaling();
        public CustomAnimation GetAnimation();
    }
}

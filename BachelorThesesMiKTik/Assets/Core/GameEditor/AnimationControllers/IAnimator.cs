using Assets.Core.GameEditor.Animation;

namespace Assets.Core.GameEditor.AnimationControllers
{
    public interface IAnimator
    {
        public void EditAnimation(CustomAnimation newAnimation);
        public void Animate(float timeDelta);
        public void PauseAnimation();
        public void ResumeAnimation();
        public void ResetAnimation();
        public void RemoveAnimation();
        public void Stop();
        public bool HasFinished();
        public bool Animating();
    }
}

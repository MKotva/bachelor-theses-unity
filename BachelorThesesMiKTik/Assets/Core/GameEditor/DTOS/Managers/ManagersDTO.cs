using System;

namespace Assets.Core.GameEditor.DTOS.Managers
{
    [Serializable]
    public class ManagersDTO
    {
        public ManagerDTO SpritesManager;
        public ManagerDTO AnimationManager;
        public ManagerDTO AudioManager;

        public ManagersDTO()
        {
            SpritesManager = new ManagerDTO();
            AnimationManager = new ManagerDTO();
            AudioManager = new ManagerDTO();
        }

        public ManagersDTO(ManagerDTO spritesManager, ManagerDTO animationManager, ManagerDTO audioManager)
        {
            SpritesManager = spritesManager;
            AnimationManager = animationManager;
            AudioManager = audioManager;
        }
    }
}

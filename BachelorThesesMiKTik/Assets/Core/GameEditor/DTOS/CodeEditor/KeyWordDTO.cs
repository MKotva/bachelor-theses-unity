using UnityEngine;

namespace Assets.Core.GameEditor.DTOS
{
    public class KeyWordDTO
    {
        public int Index {  get;  private set; }
        public int Lenght { get; private set; }
        public Color32 Color { get; private set; }

        public KeyWordDTO(int index, int lenght, Color32 color = new Color32())
        {
            Index = index;
            Lenght = lenght;
            Color = color;
        }
    }
}

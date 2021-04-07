using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Palette", menuName = "Color Palettes", order = 51)]
    public class ColorPalette : ScriptableObject
    {
        public Color shapeColor1 = Color.magenta;
        public Color shapeColor2 = Color.magenta;
        public Color shapeColor3 = Color.magenta;
        public Color shapeColor4 = Color.magenta;
        public Color shapeColor5 = Color.magenta;

        public Color backgroundColor = Color.magenta;
    }
}
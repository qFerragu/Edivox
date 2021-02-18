using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Edivox.Runtime
{
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "ScriptableObjects/ColorPalette", order = 1)]
    public class ColorPalette : ScriptableObject
    {
        public List<Color> colors;

        public ColorPalette()
        {
            colors = new List<Color>();
        }

        public ColorPalette(Color[] _colors)
        {
            colors = _colors.ToList();
        }

        public ColorPalette(Color color)
        {
            colors.Add(color);
        }

        public Color GetColor(int colorIndex)
        {
            if (colorIndex < 0 || colorIndex >= colors.Count)
                return Color.white;

            return colors[colorIndex];
        }

        public int AddColor(Color _color)
        {
            colors.Add(_color);
            return colors.Count;
        }

        public int RemoveColor(int colorIndex)
        {
            if (IsColorValid(colorIndex))
                colors.RemoveAt(colorIndex);
            return colors.Count;
        }


        public void SetColor(Color color, int colorIndex)
        {
            if (IsColorValid(colorIndex))
                colors[colorIndex] = color;
        }

        public bool IsColorValid(int colorIndex)
        {
            return colorIndex >= 0 && colorIndex < colors.Count;
        }

        public bool LoadFromTexture(Texture2D texture, bool replace = false)
        {
            if (replace)
            {
                colors.Clear();
            }

            Color[] pixels = texture.GetPixels();

            foreach (var p in pixels)
            {
                if (!colors.Contains(p))
                {
                    colors.Add(p);
                }
            }

            return true;
        }

        public Texture2D CreateTexture(int rectWidth = 1, int rectHeight = 1)
        {
            if (colors.Count > 0)
            {
                Texture2D colorTexture = new Texture2D(colors.Count * rectWidth, rectHeight);
                colorTexture.filterMode = FilterMode.Point;
                int i = 0;
                foreach (var col in colors)
                {
                    int xBlock = i * rectWidth;

                    for (int x = 0; x < rectWidth; x++)
                    {
                        for (int y = 0; y < rectHeight; y++)
                        {
                            colorTexture.SetPixel(xBlock + x, y, col);
                        }
                    }
                    i++;
                }
                colorTexture.Apply();
                return colorTexture;
            }

            return null;
        }


    }

}


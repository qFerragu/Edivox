using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{
    public class ColorPaletteDrawer
    {

        public static void DrawColorPalette(ColorPalette palette)
        {
            if (palette == null || palette.colors.Count == 0)
                return;

            var lastRect = GUILayoutUtility.GetLastRect();

            int colorPerRow = 16;

            if (palette.colors.Count > 0)
            {
                paletteTexture = TextureWithColors(palette.colors.ToArray(), colorPerRow);
            }
            else
            {
                paletteTexture = null;
            }

            int numColors = palette.colors.Count;
            int numPerRow = colorPerRow;
            int numInBottomRow = numColors % numPerRow;
            int numRow = Mathf.CeilToInt((float)numColors / (float)numPerRow);
            int cellWidth = Mathf.CeilToInt((lastRect.width - lastRect.x) / (float)numPerRow);
            int cellHeight = (int)EditorGUIUtility.singleLineHeight;

            var textureRect = new Rect(lastRect.x, lastRect.y + lastRect.height, 0.0f, 0.0f);

            if (paletteTexture != null)
            {
                //textureRect = new Rect(lastRect.x, lastRect.y + lastRect.height, paletteTexture.width * EditorGUIUtility.singleLineHeight, paletteTexture.height * EditorGUIUtility.singleLineHeight);
                textureRect = new Rect(lastRect.x, lastRect.y + lastRect.height, cellWidth * numPerRow, paletteTexture.height * cellHeight);
                GUILayoutUtility.GetRect(textureRect.width, textureRect.height);
                DrawTexture(paletteTexture, textureRect);
            }
            return;
        }

        public static bool DrawColorPalette(ColorPalette palette, ref int colorKey)
        {
            if (palette == null || palette.colors.Count == 0)
                return false;

            var lastRect = GUILayoutUtility.GetLastRect();

            int colorPerRow = 16;

            if (palette.colors.Count > 0)
            {
                paletteTexture = TextureWithColors(palette.colors.ToArray(), colorPerRow);
            }
            else
            {
                paletteTexture = null;
            }

            int numColors = palette.colors.Count;
            int numPerRow = colorPerRow;
            int numInBottomRow = numColors % numPerRow;
            int numRow = Mathf.CeilToInt((float)numColors / (float)numPerRow);
            int cellWidth = Mathf.CeilToInt((lastRect.width - lastRect.x) / (float)numPerRow);
            int cellHeight = (int)EditorGUIUtility.singleLineHeight;

            var textureRect = new Rect(lastRect.x, lastRect.y + lastRect.height, 0.0f, 0.0f);

            if (paletteTexture != null)
            {

                textureRect.width = cellWidth * numPerRow;
                textureRect.height = paletteTexture.height * cellHeight;
            }

            if (paletteTexture != null)
            {
                GUILayoutUtility.GetRect(textureRect.width, textureRect.height);
                DrawTexture(paletteTexture, textureRect);
                DrawGrid(textureRect.x, textureRect.y, palette.colors.Count, paletteTexture.width, paletteTexture.height, cellWidth, cellHeight, Color.black);
            }


            if (IsClickInRect(textureRect))
            {
                var e = Event.current;
                Vector2 rectClickPosition = e.mousePosition - textureRect.position;
                int cellXIndex = (int)(rectClickPosition.x / cellWidth);
                int cellYIndex = (int)(rectClickPosition.y / cellHeight);
                int clickedOnKey = cellYIndex * numPerRow + cellXIndex;
                if (numColors > 0 && clickedOnKey < numColors)
                {
                    colorKey = clickedOnKey;
                }
            }

            DrawOnSelectedCell(colorKey, textureRect, numPerRow, cellWidth, cellHeight);

            //EditorGUI.DrawRect(textureRect, Color.white); //utilise ça

            return true;
        }

        static Texture2D paletteTexture;
        static GUIStyle tempDrawTextureStyle;


        static Texture2D TextureWithColors(Color[] colors, int numColorRow)
        {
            if (colors == null || colors.Length == 0)
            {
                return new Texture2D(0, 0, TextureFormat.RGBA32, false, true);
            }
            // figure out our texture size based on the itemsPerRow and color count
            int totalRows = Mathf.CeilToInt((float)colors.Length / (float)numColorRow);
            var tex = new Texture2D(numColorRow, totalRows, TextureFormat.RGBA32, false, true);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.hideFlags = HideFlags.HideAndDontSave;
            int x = 0;
            int y = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                x = i % numColorRow;
                y = totalRows - 1 - Mathf.CeilToInt(i / numColorRow);
                tex.SetPixel(x, y, colors[i]);
            }
            for (x++; x < tex.width; x++)
            {
                tex.SetPixel(x, y, Color.clear);
            }

            tex.Apply();

            return tex;
        }

        static void DrawGrid(float startingPointX, float startingPointY, int numColors, int cellsX, int cellsY, int cellsW, int cellsH, Color color)
        {
            if (cellsX == 0 && cellsY == 0)
            {
                return;
            }
            // draw vertical lines
            Rect currentRect = new Rect(startingPointX, startingPointY, 2, cellsH * cellsY);
            int fullHeight = cellsH * cellsY + 1; // +1 to get the corners
            int oneLessHeight = cellsH * (cellsY - 1) + 1;
            // oneLessHeight will be 1 if theres only one row
            if (cellsY == 1)
            {
                oneLessHeight = 0;
            }
            int numInBottomRow = numColors % cellsX;
            for (int i = 0; i <= cellsX; i++)
            {
                // height will be 1 unit shorter if bottom cell does not exist
                currentRect.x = startingPointX + cellsW * i;
                bool bottomCellExists = numInBottomRow == 0 || i <= numInBottomRow;
                currentRect.height = bottomCellExists ? fullHeight : oneLessHeight;
                EditorGUI.DrawRect(currentRect, color);
                //DrawTexture(colorTexture, currentRect);
            }

            // draw horizontal lines
            currentRect.x = startingPointX;
            currentRect.height = 1;
            currentRect.width = cellsW * cellsX;
            for (int i = 0; i <= cellsY; i++)
            {
                currentRect.y = startingPointY + cellsH * i;
                if ((i == cellsY || cellsY == 1) && numInBottomRow > 0)
                {
                    currentRect.width = numInBottomRow * cellsW;
                }
                //DrawTexture(colorTexture, currentRect);
                EditorGUI.DrawRect(currentRect, color);
            }
        }

        static void DrawOnSelectedCell(int selectedCell, Rect textureRect, int colorPerRow, int cellW, int cellH)
        {
            int selectedCellY = selectedCell / colorPerRow;
            int selectedCellX = selectedCell - (colorPerRow * selectedCellY);
            Vector2 posRect = new Vector2(textureRect.x + selectedCellX * cellW, textureRect.y + selectedCellY * cellH);
            DrawGrid(posRect.x - 1, posRect.y - 1, 1, 1, 1, cellW, cellH + 2, Color.black);
            DrawGrid(posRect.x, posRect.y, 1, 1, 1, cellW, cellH, Color.white);
        }

        static void DrawTexture(Texture2D texture, Rect rect)
        {
            if (tempDrawTextureStyle == null)
            {
                tempDrawTextureStyle = new GUIStyle();
            }
            tempDrawTextureStyle.normal.background = texture;
            EditorGUI.LabelField(rect, "", tempDrawTextureStyle);
        }

        static bool IsClick()
        {
            Event e = Event.current;
            return e != null && e.type == EventType.MouseDown && e.button == 0;
        }

        static bool IsClickInRect(Rect rect)
        {
            Event e = Event.current;
            return e != null && e.type == EventType.MouseDown && e.button == 0 && rect.Contains(e.mousePosition);
        }
    }
}
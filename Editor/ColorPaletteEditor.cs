using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Edivox.Runtime;

namespace Edivox.Editor
{
    [CustomEditor(typeof(ColorPalette))]
    public class ColorPaletteEditor : UnityEditor.Editor
    {

        int colorSelected = 0;
        int colorBlockSize = 1;

        bool isDirty = true;

        public override void OnInspectorGUI()
        {
            ColorPalette palette = (ColorPalette)target;


            if (!EditorUtility.IsDirty(palette) && isDirty)
            {
                Undo.RecordObject(palette, palette.name);
            }
            isDirty = EditorUtility.IsDirty(palette);

            int lastColor = colorSelected;
            DrawPaletteGUI(palette, ref colorSelected);
            if (lastColor != colorSelected)
            {
                Repaint();
            }



            EditorGUILayout.BeginHorizontal();
            colorBlockSize = EditorGUILayout.IntField("Block Size", colorBlockSize);
            colorBlockSize = Mathf.Abs(colorBlockSize);

            if (GUILayout.Button("Export PNG"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Export ColorPalette", target.name, "png", "");
                if (path != string.Empty)
                {

                    Texture2D texture = CreateTextureFile(palette, path, colorBlockSize);
                    DestroyImmediate(texture);
                }


            }
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawPaletteGUI(ColorPalette palette, ref int idColor)
        {
            if (palette == null)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add PNG"))
            {
                string path = EditorUtility.OpenFilePanel("Open Palette Color", Application.dataPath, "png");
                if (path.Length != 0)
                {
                    Texture2D texture = ReadPNGFile(path);
                    palette.LoadFromTexture(texture);
                }
            }


            if (GUILayout.Button("Replace PNG"))
            {
                string path = EditorUtility.OpenFilePanel("Open Palette Color", Application.dataPath, "png");
                if (path.Length != 0)
                {
                    Texture2D texture = ReadPNGFile(path);
                    palette.LoadFromTexture(texture, true);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Color Palette", EditorStyles.boldLabel);

            ColorPaletteDrawer.DrawColorPalette(palette, ref idColor);

            EditorGUILayout.BeginHorizontal();

            Color color = palette.GetColor(idColor);

            Color colorModified = EditorGUILayout.ColorField(color);
            if (colorModified != color)
            {
                palette.SetColor(colorModified, idColor);
                EditorUtility.SetDirty(palette);
            }

            if (GUILayout.Button("+"))
            {
                palette.AddColor(color);
                idColor = palette.colors.Count - 1;
                EditorUtility.SetDirty(palette);
            }
            if (GUILayout.Button("-"))
            {
                palette.RemoveColor(idColor);
                if (idColor >= palette.colors.Count && palette.colors.Count > 0)
                    idColor = palette.colors.Count - 1;
                EditorUtility.SetDirty(palette);
            }
            EditorGUILayout.EndHorizontal();
        }

        static Texture2D ReadPNGFile(string path)
        {
            Texture2D texture = new Texture2D(0, 0);
            var fileContent = File.ReadAllBytes(path);
            texture.LoadImage(fileContent);
            return texture;
        }

        public static Texture2D CreateTextureFile(ColorPalette palette, string path, int colorBlockSize)
        {
            Texture2D texture = palette.CreateTexture(colorBlockSize, colorBlockSize);
            byte[] bytesText = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytesText);
            AssetDatabase.Refresh();
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.spritePixelsPerUnit = 100;
            importer.textureType = TextureImporterType.Sprite;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
            return texture;
        }

    }
}

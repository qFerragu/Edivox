using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{

    public class VoxelCreatorWindow : EditorWindow
    {
        [MenuItem("Tools/VoxEdit/Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(VoxelCreatorWindow));
        }

        ColorPalette paletteSelected;

        Vector3Int newMeshSize = Vector3Int.one;
        bool isFill = true;
        void OnGUI()
        {
            newMeshSize = EditorGUILayout.Vector3IntField("Size", newMeshSize);
            isFill = EditorGUILayout.Toggle("Fill", isFill);

            paletteSelected = (ColorPalette)EditorGUILayout.ObjectField("Color Palette", paletteSelected, typeof(ColorPalette), false);

            if (paletteSelected != null)
            {
                EditorGUILayout.LabelField("Preview");
                ColorPaletteDrawer.DrawColorPalette(paletteSelected);
            }

            if (GUILayout.Button("Create"))
            {
                if (paletteSelected == null)
                {
                    string path = EditorUtility.SaveFilePanelInProject("Save new Color Palette", "ColorPalette", "asset", "");
                    if (path != string.Empty)
                    {
                        paletteSelected = ScriptableObject.CreateInstance<ColorPalette>();
                        paletteSelected.AddColor(Color.white);
                        AssetDatabase.CreateAsset(paletteSelected, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                }

                if (paletteSelected != null)
                {
                    InstanciateNewVoxelMesh();
                    Close();
                }

            }
        }


        void InstanciateNewVoxelMesh()
        {
            GameObject newMesh = new GameObject("VoxelMesh");
            VoxelMesh voxelMesh = newMesh.AddComponent<VoxelMesh>();
            voxelMesh.Init(newMeshSize, isFill, paletteSelected);
            Selection.activeGameObject = newMesh;
            EditorUtility.SetDirty(voxelMesh);

            //for (int x = 0; x < newMeshSize.x; x++)
            //{
            //    for (int y = 0; y < newMeshSize.y; y++)
            //    {
            //        for (int z = 0; z < newMeshSize.z; z++)
            //        {
            //            VoxelData vd = voxelMesh.GetVoxel(x, y, z);
            //            EditorUtility.SetDirty(vd.render);
            //        }
            //    }
            //}

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Edivox.Runtime;

namespace Edivox.Editor
{

    public class VoxelEditWindow : EditorWindow
    {
        VoxelMesh voxelMesh = null;

        Tool tool = new Brush();
        Tool.ToolMode mode = Tool.ToolMode.Add;


        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(VoxelEditWindow));
        }

        void OnGUI()
        {

            if (Selection.activeGameObject != null && voxelMesh == null)
            {
                voxelMesh = Selection.activeGameObject.GetComponent<VoxelMesh>();
                tool.SetVoxelMesh(voxelMesh);
            }


            if (voxelMesh != null)
            {

                GUILayout.Label(voxelMesh.gameObject.name);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(mode.ToString());




                EditorGUI.BeginDisabledGroup(mode == Tool.ToolMode.Add);
                if (GUILayout.Button("Add"))
                {
                    mode = Tool.ToolMode.Add;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(mode == Tool.ToolMode.Sub);
                if (GUILayout.Button("Sub"))
                {
                    mode = Tool.ToolMode.Sub;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(mode == Tool.ToolMode.Paint);
                if (GUILayout.Button("Paint"))
                {
                    mode = Tool.ToolMode.Paint;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();

                GUIToolSelection();


                GUIColorPalette();


                GUIExport();
            }
            else
            {
                GUILayout.Label("No VoxelMesh Selected");
            }

            //Debug.Log("Fonction");

        }

        void OnFocus()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI; // Just in case
            Undo.undoRedoPerformed -= this.OnUndo;
            SceneView.duringSceneGui += this.OnSceneGUI;
            Undo.undoRedoPerformed += this.OnUndo;

        }


        bool isPress;

        void OnSceneGUI(SceneView sceneView)
        {

            if (voxelMesh == null)
            {
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(0);
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && tool != null)
            {

                tool.Mode = mode;
                tool.SetVoxelMesh(voxelMesh);

                if (!Event.current.alt)
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !isPress)
                    {
                        tool.OnClickPress(hit);
                        isPress = true;
                    }
                    else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                    {

                        tool.OnClickRelease(hit);
                        isPress = false;
                    }
                    else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                    {
                        tool.OnDrag(hit);

                    }
                }
                tool.DisplayVisualHelp(hit);
                SceneView.RepaintAll();
            }
            else
            {
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && isPress)
                {
                    tool.CancelAction();
                    isPress = false;
                }
            }
        }

        void OnSelectionChange()
        {
            if (Selection.activeGameObject != null)
            {
                voxelMesh = Selection.activeGameObject.GetComponent<VoxelMesh>();

                if (voxelMesh != null)
                {
                    voxelMesh.Refresh();
                    tool.SetVoxelMesh(voxelMesh);
                }
                Repaint();
            }
            else
            {
                voxelMesh = null;
            }
        }

        void OnDestroy()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
            Undo.undoRedoPerformed -= this.OnUndo;
        }


        bool foldoutTool = true;

        void GUIToolSelection()
        {

            foldoutTool = EditorGUILayout.Foldout(foldoutTool, "Tools");
            if (!foldoutTool)
                return;



            if (tool == null)
            {
                GUILayout.Label("None");
            }
            else
            {
                GUILayout.Label(tool.GetType().Name);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(tool.GetType().Name == "Brush");
            if (GUILayout.Button("Brush"))
            {
                tool = new Brush();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(tool.GetType().Name == "Box");
            if (GUILayout.Button("Box"))
            {
                tool = new Box();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(tool.GetType().Name == "Cylinder");
            if (GUILayout.Button("Cylinder"))
            {
                tool = new Cylinder();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(tool.GetType().Name == "Sphere");
            if (GUILayout.Button("Sphere"))
            {
                tool = new Sphere();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(tool.GetType().Name == "Laser");
            if (GUILayout.Button("Laser"))
            {
                tool = new Laser();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Fill"))
            {
                Undo.RecordObject(voxelMesh, "Fill VoxelMesh");
                EditorUtility.SetDirty(voxelMesh);
                voxelMesh.Fill(true);
            }

            if (GUILayout.Button("Empty"))
            {
                Undo.RecordObject(voxelMesh, "Empty VoxelMesh");
                EditorUtility.SetDirty(voxelMesh);
                voxelMesh.Fill(false);
            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(voxelMesh);
                SerializedObject serializedObject = new SerializedObject(voxelMesh);
                serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button("Refresh"))
            {
                if (voxelMesh != null)
                {
                    Undo.RecordObject(voxelMesh, "VoxelMesh");
                    voxelMesh.Refresh();
                }
            }

            EditorGUILayout.EndHorizontal();

        }

        bool foldoutColor = true;

        void GUIColorPalette()
        {

            foldoutColor = EditorGUILayout.Foldout(foldoutColor, "Color Palette");
            if (!foldoutColor)
                return;
            if (voxelMesh != null && voxelMesh.ColorPalette != null)
            {
                ColorPalette palette = (ColorPalette)EditorGUILayout.ObjectField("Color Palette", voxelMesh.ColorPalette, typeof(ColorPalette), false);
                if (palette != null && palette != voxelMesh.ColorPalette)
                {
                    voxelMesh.ColorPalette = palette;
                    voxelMesh.Refresh();
                }
                int colorSelected = voxelMesh.voxelTemplate.colorId;
                ColorPaletteDrawer.DrawColorPalette(voxelMesh.ColorPalette, ref colorSelected);
                if (voxelMesh.voxelTemplate.colorId != colorSelected)
                {
                    Repaint();
                }
                voxelMesh.voxelTemplate.colorId = colorSelected;
            }
        }

        bool foldoutExport = false;
        ExportSettings exportSettings = new ExportSettings();


        void GUIExport()
        {
            foldoutExport = EditorGUILayout.Foldout(foldoutExport, "Export");
            if (!foldoutExport)
                return;

            exportSettings.voxelSize = EditorGUILayout.Vector3Field("Voxel Size", exportSettings.voxelSize);

            exportSettings.pivot = EditorGUILayout.Vector3Field("Pivot", exportSettings.pivot);
            exportSettings.pivot.x = Mathf.Clamp(exportSettings.pivot.x, 0f, 1f);
            exportSettings.pivot.y = Mathf.Clamp(exportSettings.pivot.y, 0f, 1f);
            exportSettings.pivot.z = Mathf.Clamp(exportSettings.pivot.z, 0f, 1f);

            if (GUILayout.Button("Export To Obj"))
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                string path = EditorUtility.SaveFilePanel("Export VoxelMesh", Application.dataPath, voxelMesh.gameObject.name, "obj");
                MeshFilter mesh = cube.GetComponent<MeshFilter>();
                mesh.name = Path.GetFileNameWithoutExtension(path);

                VoxelMeshExport meshExport = new VoxelMeshExport(exportSettings);
                meshExport.GenerateMesh(voxelMesh, mesh);


                ObjExporterEditor.MeshToFile(mesh, Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                DestroyImmediate(cube);

                AssetDatabase.Refresh();
            }



        }

        void OnUndo()
        {
            if (voxelMesh != null)
            {
                voxelMesh.Refresh();
            }
        }

    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{
    [CustomEditor(typeof(VoxelMesh))]
    public class VoxelMeshEditor : UnityEditor.Editor
    {

        List<GameObject> planesHelper = new List<GameObject>();

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            if (GUILayout.Button("Voxel Mesh Window"))
            {
                VoxelEditWindow.ShowWindow();
            }
            serializedObject.ApplyModifiedProperties();
        }

        UnityEditor.Tool lastTool;

        private void OnEnable()
        {
            VoxelEditWindow.ShowWindow();
            lastTool = Tools.current;
            Tools.current = UnityEditor.Tool.None;
            AddPlanesHelper();
        }

        private void OnDisable()
        {
            Tools.current = lastTool;
            ClearPlaneHealper();
        }

        void OnDestroy()
        {
            ClearPlaneHealper();
        }

        void OnSceneGUI()
        {
        }

        void AddPlanesHelper()
        {

            VoxelMesh voxelMesh = (VoxelMesh)target;
            if (voxelMesh == null)
            {
                Debug.LogError("VoxelMesh target is null");
                return;
            }

            ClearPlaneHealper();

            for (VoxelMesh.Direction dir = 0; dir < VoxelMesh.Direction.MAX; dir++)
            {
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Quad);
                Vector3 normal = VoxelMesh.GetVectorFromDirection(dir);
                plane.transform.rotation = Quaternion.LookRotation(normal);

                Vector3 center = (voxelMesh.meshSize);
                center.Set(center.x / 2f, center.y / 2f, center.z / 2f);
                plane.transform.position = center;

                switch (dir)
                {
                    case VoxelMesh.Direction.Up:
                        plane.transform.position = plane.transform.position + Vector3.up * (voxelMesh.meshSize.y / 2f);
                        plane.transform.localScale = new Vector3(voxelMesh.meshSize.x, voxelMesh.meshSize.z, 1);
                        break;
                    case VoxelMesh.Direction.Down:
                        plane.transform.position = plane.transform.position + Vector3.down * (voxelMesh.meshSize.y / 2f);
                        plane.transform.localScale = new Vector3(voxelMesh.meshSize.x, voxelMesh.meshSize.z, 1);
                        break;
                    case VoxelMesh.Direction.Left:
                        plane.transform.position = plane.transform.position + Vector3.left * (voxelMesh.meshSize.x / 2f);
                        plane.transform.localScale = new Vector3(voxelMesh.meshSize.z, voxelMesh.meshSize.y, 1);
                        break;
                    case VoxelMesh.Direction.Right:
                        plane.transform.position = plane.transform.position + Vector3.right * (voxelMesh.meshSize.x / 2f);
                        plane.transform.localScale = new Vector3(voxelMesh.meshSize.z, voxelMesh.meshSize.y, 1);
                        break;
                    case VoxelMesh.Direction.Front:
                        plane.transform.position = plane.transform.position + Vector3.forward * (voxelMesh.meshSize.z / 2f);
                        plane.transform.localScale = new Vector3(voxelMesh.meshSize.x, voxelMesh.meshSize.y, 1);
                        break;
                    case VoxelMesh.Direction.Back:
                        plane.transform.position = plane.transform.position + Vector3.back * (voxelMesh.meshSize.z / 2f);
                        plane.transform.localScale = new Vector3(voxelMesh.meshSize.x, voxelMesh.meshSize.y, 1);
                        break;
                    default:
                        Debug.Log("Error Direction");
                        break;
                }
                planesHelper.Add(plane);
            }
        }

        void ClearPlaneHealper()
        {
            if (planesHelper != null && planesHelper.Count > 0)
            {
                foreach (var item in planesHelper)
                {
                    DestroyImmediate(item);
                }
                planesHelper.Clear();
            }
        }

    }
}

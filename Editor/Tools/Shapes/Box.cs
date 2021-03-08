using Edivox.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Edivox.Editor
{
    public class Box : Shape
    {

        public override void OnClickRelease(RaycastHit hit)
        {
            Vector3Int min = Vector3Int.zero;
            Vector3Int max = Vector3Int.zero;
            GetMinMaxSelection(startPos, lastPos, ref min, ref max);

            Undo.RecordObject(voxelMesh, "Box VoxelMesh");
            EditorUtility.SetDirty(voxelMesh);
            MakeBox(min, max);
            firstVoxel = null;
        }

        public override void DisplayVisualHelp(RaycastHit hit)
        {
            if (voxelMesh == null)
                return;

            if (firstVoxel == null)
            {
                base.DisplayVisualHelp(hit);
            }
            else
            {
                Vector3Int min = Vector3Int.zero;
                Vector3Int max = Vector3Int.zero;

                GetMinMaxSelection(startPos, lastPos, ref min, ref max);
                Vector3 center = (Vector3)(max - min) / 2f + min;

                Handles.color = Color.red;
                Vector3 posVoxel = center;
                Handles.DrawWireCube(posVoxel + Vector3.one * 0.5f, (max - min) + Vector3Int.one);
            }

        }

        void MakeBox(Vector3Int start, Vector3Int end)
        {
            for (int x = start.x; x <= end.x; x++)
            {
                for (int y = start.y; y <= end.y; y++)
                {
                    for (int z = start.z; z <= end.z; z++)
                    {
                        VoxelData voxel = voxelMesh.GetVoxel(x, y, z);
                        if (voxel != null)
                        {
                            bool isVisible = voxel.IsVisible;

                            voxel.Copy(voxelMesh.voxelTemplate);

                            if (mode == ToolMode.Sub)
                            {
                                isVisible = false;
                            }
                            else if (mode == ToolMode.Add)
                            {
                                isVisible = true;
                            }
                            voxelMesh.SetVoxelVisible(voxel.Position, isVisible);
                        }
                    }

                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{
    public class Cylinder : Shape
    {
        // Start is called before the first frame update
        public override void OnClickRelease(RaycastHit hit)
        {
            Vector3Int min = Vector3Int.zero;
            Vector3Int max = Vector3Int.zero;

            GetMinMaxSelection(startPos, lastPos, ref min, ref max);

            Undo.RecordObject(voxelMesh, "Cylinder VoxelMesh");
            EditorUtility.SetDirty(voxelMesh);
            MakeCylinder(min, max);
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
                Vector3 centrer = (Vector3)(max - min) / 2f + min;

                Handles.color = Color.red;
                Vector3 posVoxel = centrer;
                Handles.DrawWireCube(posVoxel, (max - min) + Vector3Int.one);
            }

        }

        List<Vector2Int> GetEllipse(int width, int height)
        {
            List<Vector2Int> points = new List<Vector2Int>();

            int w = width / 2;
            int h = height / 2;

            int dx = width % 2 == 0 ? 1 : 0;
            int dy = height % 2 == 0 ? 1 : 0;

            int cx = w;
            int cy = h;

            for (int y = 0; y <= h - dy; y++)
            {
                for (int x = 0; x <= w - dx; x++)
                {

                    if (x * x * (h + dx) * (h + dx) + y * y * (w + dy) * (w + dy) <= h * h * w * w)
                    {

                        points.Add(new Vector2Int(cx + x, cy + y));
                        points.Add(new Vector2Int(cx - x - dx, cy + y));
                        points.Add(new Vector2Int(cx + x, cy - y - dy));
                        points.Add(new Vector2Int(cx - x - dx, cy - y - dy));
                    }
                }
            }

            return points;
        }



        void MakeCylinder(Vector3Int start, Vector3Int end)
        {
            Vector3Int size = end - start + Vector3Int.one;
            Vector3Int normalInt = new Vector3Int(
               Mathf.Abs(Mathf.RoundToInt(normal.x)),
                Mathf.Abs(Mathf.RoundToInt(normal.y)),
                Mathf.Abs(Mathf.RoundToInt(normal.z)));

            VoxelMesh.Direction dir = VoxelMesh.Direction.MAX;

            List<Vector2Int> points = null;
            if (normalInt.x != 0)
            {
                dir = VoxelMesh.Direction.Right;
                points = GetEllipse(size.z, size.y);
            }
            else if (normalInt.y != 0)
            {
                dir = VoxelMesh.Direction.Up;
                points = GetEllipse(size.x, size.z);
            }
            else if (normalInt.z != 0)
            {
                dir = VoxelMesh.Direction.Front;
                points = GetEllipse(size.x, size.y);
            }


            if (dir == VoxelMesh.Direction.Right)
            {
                for (int i = 0; i < size.x; i++)
                {
                    foreach (var item in points)
                    {
                        VoxelData voxel = voxelMesh.GetVoxel(start.x + i, start.y + item.y, start.z + item.x);
                        if (voxel != null)
                        {
                            voxel.Copy(voxelMesh.voxelTemplate);
                            voxelMesh.SetVoxelVisible(voxel.Position, mode != ToolMode.Sub);
                        }
                    }
                }
            }
            else if (dir == VoxelMesh.Direction.Up)
            {
                for (int i = 0; i < size.y; i++)
                {
                    foreach (var item in points)
                    {
                        VoxelData voxel = voxelMesh.GetVoxel(start.x + item.x, start.y + i, start.z + item.y);
                        if (voxel != null)
                        {
                            voxel.Copy(voxelMesh.voxelTemplate);
                            voxelMesh.SetVoxelVisible(voxel.Position, mode != ToolMode.Sub);
                        }
                    }
                }

            }
            else if (dir == VoxelMesh.Direction.Front)
            {
                for (int i = 0; i < size.z; i++)
                {
                    foreach (var item in points)
                    {
                        VoxelData voxel = voxelMesh.GetVoxel(start.x + item.x, start.y + item.y, start.z + i);
                        if (voxel != null)
                        {
                            voxel.Copy(voxelMesh.voxelTemplate);
                            voxelMesh.SetVoxelVisible(voxel.Position, mode != ToolMode.Sub);
                        }
                    }
                }
            }

        }


    }
}

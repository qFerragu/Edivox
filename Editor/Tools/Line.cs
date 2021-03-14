using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{
    public class Line : Tool
    {

        VoxelData startLine;
        VoxelData endLine;
        bool isPressed = false;
        public override void OnClickPress(RaycastHit hit)
        {
            isPressed = true;
 

            if (voxelMesh == null)
                return;
            Vector3 pos = GetHitPosFromMode(hit, mode);
            VoxelData voxel = voxelMesh.GetVoxel(pos);
            if (voxel != null)
            {
                startLine = voxel;
            }

        }

        public override void OnDrag(RaycastHit hit)
        {

            if (voxelMesh == null)
                return;

            Vector3 pos = GetHitPosFromMode(hit, mode);
            endLine = voxelMesh.GetVoxel(pos);
            
            if (startLine == null)
            {
                startLine = endLine;
            }
        }

        public override void OnClickRelease(RaycastHit hit)
        {
            //Undo.RecordObject(voxelMesh, "Line VoxelMesh");
            //EditorUtility.SetDirty(voxelMesh);
            if (startLine != null && endLine  != null)
            {
                MakeLine(startLine.Position, endLine.Position);
            }
            startLine = null;
            endLine = null;
            isPressed = false;
        }

        void MakeLine(Vector3Int start, Vector3Int end)
        {
            Vector3Int pos = start;


            int dx = Mathf.Abs(end.x - pos.x);
            int dy = Mathf.Abs(end.y - pos.y);
            int dz = Mathf.Abs(end.z - pos.z);
            int stepX = pos.x < end.x ? 1 : -1;
            int stepY = pos.y < end.y ? 1 : -1;
            int stepZ = pos.z < end.z ? 1 : -1;
            double hypotenuse = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
            double tMaxX = hypotenuse * 0.5 / dx;
            double tMaxY = hypotenuse * 0.5 / dy;
            double tMaxZ = hypotenuse * 0.5 / dz;
            double tDeltaX = hypotenuse / dx;
            double tDeltaY = hypotenuse / dy;
            double tDeltaZ = hypotenuse / dz;

            SetVoxel(voxelMesh.GetVoxel(pos));


            while (pos.x != end.x || pos.y != end.y || pos.z != end.z)
            {
                if (tMaxX < tMaxY)
                {
                    if (tMaxX < tMaxZ)
                    {
                        pos.x = pos.x + stepX;
                        tMaxX = tMaxX + tDeltaX;
                    }
                    else if (tMaxX > tMaxZ)
                    {
                        pos.z = pos.z + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;
                    }
                    else
                    {
                        pos.x = pos.x + stepX;
                        tMaxX = tMaxX + tDeltaX;
                        pos.z = pos.z + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;
                    }
                }
                else if (tMaxX > tMaxY)
                {
                    if (tMaxY < tMaxZ)
                    {
                        pos.y = pos.y + stepY;
                        tMaxY = tMaxY + tDeltaY;
                    }
                    else if (tMaxY > tMaxZ)
                    {
                        pos.z = pos.z + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;
                    }
                    else
                    {
                        pos.y = pos.y + stepY;
                        tMaxY = tMaxY + tDeltaY;
                        pos.z = pos.z + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;

                    }
                }
                else
                {
                    if (tMaxY < tMaxZ)
                    {
                        pos.y = pos.y + stepY;
                        tMaxY = tMaxY + tDeltaY;
                        pos.x = pos.x + stepX;
                        tMaxX = tMaxX + tDeltaX;
                    }
                    else if (tMaxY > tMaxZ)
                    {
                        pos.z = pos.z + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;
                    }
                    else
                    {
                        pos.x = pos.x + stepX;
                        tMaxX = tMaxX + tDeltaX;
                        pos.y = pos.y + stepY;
                        tMaxY = tMaxY + tDeltaY;
                        pos.z = pos.z + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;

                    }
                }
                SetVoxel(voxelMesh.GetVoxel(pos));
            }
        }

        public override void DisplayVisualHelp(RaycastHit hit)
        {
            if (voxelMesh == null)
                return;

            Handles.color = Color.red;

            if (isPressed)
            {
                if (startLine != null)
                {
                    Handles.DrawWireCube(startLine.Position + Vector3.one * 0.5f, Vector3.one);
                }

                if (endLine != null)
                {
                    Handles.DrawWireCube(endLine.Position + Vector3.one * 0.5f, Vector3.one);
                }
            }
            else
            {
                Vector3 pos = GetHitPosFromMode(hit, mode);
                VoxelData voxel = voxelMesh.GetVoxel(pos);
                if (voxel != null)
                {
                    Vector3 posVoxel = voxel.Position;
                    Handles.DrawWireCube(posVoxel + Vector3.one * 0.5f, Vector3.one);
                }
            }

          
        }

        void SetVoxel(VoxelData voxel)
        {
            if (voxel != null)
            {
                voxel.Copy(voxelMesh.voxelTemplate);
                voxelMesh.SetVoxelVisible(voxel.Position, mode != ToolMode.Sub);
            }
        }

    }
}
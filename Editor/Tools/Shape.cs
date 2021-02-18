using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{
    public class Shape : Tool
    {

        protected VoxelData firstVoxel = null;
        protected VoxelData lastVoxel = null;
        protected Vector3 normal;

        public override void OnClickPress(RaycastHit hit)
        {
            if (voxelMesh == null)
                return;

            Vector3 pos = GetHitPosFromMode(hit, mode);

            VoxelData voxel = GetVoxelFromHitMode(hit, mode);
            if (voxel != null)
            {
                startPos = voxel.Position;
                firstVoxel = voxel;
                normal = hit.normal;
                lastPos = voxel.Position;
            }
        }

        public override void OnDrag(RaycastHit hit)
        {
            if (voxelMesh == null)
                return;

            VoxelData voxel = GetVoxelFromHitMode(hit, mode);

            if (voxel == null)
                return;

            if (firstVoxel != null)
            {
                if (voxel != firstVoxel)
                {
                    lastPos = voxel.Position;
                    lastVoxel = voxel;
                }
            }
            else
            {
                startPos = voxel.Position;
                firstVoxel = voxel;
                normal = hit.normal;
                lastPos = voxel.Position;
            }
        }

        public override void OnClickRelease(RaycastHit hit)
        {
            firstVoxel = null;
        }

        public override void DisplayVisualHelp(RaycastHit hit)
        {
            if (voxelMesh == null)
                return;

            VoxelData voxel = GetVoxelFromHitMode(hit, mode);
            if (voxel != null)
            {
                Handles.color = Color.red;
                Vector3 posVoxel = voxel.Position;
                Handles.DrawWireCube(posVoxel, Vector3Int.one);
            }
        }

        protected void GetMinMaxSelection(Vector3Int start, Vector3Int end, ref Vector3Int min, ref Vector3Int max)
        {
            if (start.x >= end.x)
            {
                min.x = end.x;
                max.x = start.x;
            }
            else
            {
                min.x = start.x;
                max.x = end.x;
            }

            if (start.y >= end.y)
            {
                min.y = end.y;
                max.y = start.y;
            }
            else
            {
                min.y = start.y;
                max.y = end.y;
            }

            if (start.z >= end.z)
            {
                min.z = end.z;
                max.z = start.z;
            }
            else
            {
                min.z = start.z;
                max.z = end.z;
            }
        }

    }
}
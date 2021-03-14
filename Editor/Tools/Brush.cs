using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{
    public class Brush : Tool
    {

        VoxelData lastVoxelModified;

        public override void OnClickPress(RaycastHit hit)
        {
            //Undo.RecordObject(voxelMesh, "Brush VoxelMesh");

            if (voxelMesh == null)
                return;
            Vector3 pos = GetHitPosFromMode(hit, mode);
            VoxelData voxel = voxelMesh.GetVoxel(pos);
            if (voxel != null)
            {
                lastVoxelModified = voxel;
                SetVoxel(voxel);
            }

        }

        public override void OnDrag(RaycastHit hit)
        {

            //if (lastVoxelModified == null)
            //{
            //    VoxelData voxel = GetVoxelFromHitMode(hit, mode);
            //    lastVoxelModified = voxel;
            //    SetVoxel(voxel);
            //}
            //else
            //{

            //    ToolMode checkMode = mode;
            //    if (mode == ToolMode.TM_Add)
            //        checkMode = ToolMode.TM_Sub;
            //    else if (mode == ToolMode.TM_Sub)
            //        checkMode = ToolMode.TM_Add;



            //    VoxelData voxel = GetVoxelFromHitMode(hit, checkMode);

            //    if (voxel != null)
            //    {
            //        Debug.Log("Check = " + voxel.Position);
            //        Debug.Log("Last = " + lastVoxelModified.Position);
            //    }

            //    if (voxel != null && voxel != lastVoxelModified)
            //    {



            //        voxel = GetVoxelFromHitMode(hit, mode);
            //        if (voxel != null)
            //        {
            //            lastVoxelModified = voxel;
            //            SetVoxel(voxel);
            //        }

            //    }
            //}
        }

        public override void OnClickRelease(RaycastHit hit)
        {
            EditorUtility.SetDirty(voxelMesh);
            lastVoxelModified = null;
        }

        public override void DisplayVisualHelp(RaycastHit hit)
        {
            if (voxelMesh == null)
                return;

            Vector3 pos = GetHitPosFromMode(hit, mode);


            VoxelData voxel = voxelMesh.GetVoxel(pos);
            if (voxel != null)
            {
                Handles.color = Color.red;
                Vector3 posVoxel = voxel.Position;
                Handles.DrawWireCube(posVoxel + Vector3.one * 0.5f, Vector3.one);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{
    public class Tool
    {

        public enum ToolMode
        {
            Add,
            Sub,
            Paint,
            TM_MAX
        }


        protected Vector3Int startPos;
        protected Vector3Int lastPos;
        protected VoxelMesh voxelMesh;
        protected ToolMode mode;

        public ToolMode Mode { get => mode; set => mode = value; }

        public virtual void SetVoxelMesh(VoxelMesh _mesh)
        {
            voxelMesh = _mesh;
        }


        public virtual void OnClickPress(RaycastHit hit)
        {
        }

        public virtual void OnDrag(RaycastHit hit)
        {
        }

        public virtual void OnClickRelease(RaycastHit hit)
        {
        }


        public virtual void DisplayVisualHelp(RaycastHit hit)
        {
        }

        public virtual void CancelAction()
        {
        }

        protected Vector3 GetHitPosFromMode(RaycastHit hit, ToolMode _mode)
        {
            Vector3 pos = hit.point;
            switch (_mode)
            {
                case ToolMode.Add:
                    pos += hit.normal * 0.5f;
                    break;
                case ToolMode.Sub:
                    pos -= hit.normal * 0.5f;
                    break;
                case ToolMode.Paint:
                    pos -= hit.normal * 0.5f;
                    break;
                default:
                    break;
            }
            return pos;
        }

        protected VoxelData GetVoxelFromHitMode(RaycastHit hit, ToolMode _mode)
        {
            if (voxelMesh == null)
                return null;

            Vector3 pos = GetHitPosFromMode(hit, mode);

            VoxelData voxel = voxelMesh.GetVoxel(pos);
            if (voxel != null)
            {
                return voxel;
            }

            return voxel = voxelMesh.GetVoxel(hit.point + hit.normal * 0.5f);

        }



    }
}

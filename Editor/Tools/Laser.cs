using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{

    public class Laser : Tool
    {
        public override void OnClickPress(RaycastHit hit)
        {
            if (voxelMesh == null)
                return;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RemoveAllVerticeInRay(hit, ray);
        }

        public override void OnDrag(RaycastHit hit)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RemoveAllVerticeInRay(hit, ray);
        }

        void RemoveAllVerticeInRay(RaycastHit hit, Ray ray)
        {
            //VoxelRender render = null;

            //RaycastHit[] hitArray = Physics.RaycastAll(ray);

            //if (hitArray.Length > 0)
            //{
            //    Undo.RecordObject(voxelMesh, "Laser VoxelMesh");
            //    EditorUtility.SetDirty(voxelMesh);
            //}


            //foreach (var item in hitArray)
            //{
            //    if (item.collider.gameObject.TryGetComponent<VoxelRender>(out render))
            //    {
            //        RemoveVoxelHit(item);
            //    }
            //}

        }



        void RemoveAllVerticeInRay(Ray ray)
        {
            //RaycastHit hit;
            //VoxelRender render = null;
            //bool isValid = true;
            //while (isValid && Physics.Raycast(ray, out hit))
            //{
            //    isValid = hit.collider.gameObject.TryGetComponent<VoxelRender>(out render);
            //    if (isValid)
            //    {
            //        RemoveVoxelHit(hit);
            //    }
            //}
        }

        void RemoveVoxelHit(RaycastHit hit)
        {
            Vector3 pos = hit.point;
            pos -= hit.normal * 0.5f;
            VoxelData voxel = voxelMesh.GetVoxel(pos);
            if (voxel != null)
            {
                voxelMesh.SetVoxelVisible(voxel.Position, false);
            }
        }



    }
}

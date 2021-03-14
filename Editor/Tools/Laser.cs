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


            RaycastHit[] hitArray = Physics.RaycastAll(ray);

            if (hitArray.Length >= 2)
            {
                VoxelData start = voxelMesh.GetVoxel(GetHitPosFromMode(hitArray[hitArray.Length - 1], ToolMode.Sub));
                VoxelData end = voxelMesh.GetVoxel(GetHitPosFromMode(hitArray[0], ToolMode.Add));

                if (start != null && end != null)
                {
                    LaserVoxels(start.Position, end.Position);
                }
            }
        }


        void LaserVoxels(Vector3Int start, Vector3Int end)
        {
            int x1 = end.x, y1 = end.y, z1 = end.z, x0 = start.x, y0 = start.y, z0 = start.z;
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int dz = Mathf.Abs(z1 - z0);
            int stepX = x0 < x1 ? 1 : -1;
            int stepY = y0 < y1 ? 1 : -1;
            int stepZ = z0 < z1 ? 1 : -1;
            double hypotenuse = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
            double tMaxX = hypotenuse * 0.5 / dx;
            double tMaxY = hypotenuse * 0.5 / dy;
            double tMaxZ = hypotenuse * 0.5 / dz;
            double tDeltaX = hypotenuse / dx;
            double tDeltaY = hypotenuse / dy;
            double tDeltaZ = hypotenuse / dz;

            voxelMesh.SetVoxelVisible(new Vector3Int(x0, y0, z0), false);

            while (x0 != x1 || y0 != y1 || z0 != z1)
            {
                if (tMaxX < tMaxY)
                {
                    if (tMaxX < tMaxZ)
                    {
                        x0 = x0 + stepX;
                        tMaxX = tMaxX + tDeltaX;
                    }
                    else if (tMaxX > tMaxZ)
                    {
                        z0 = z0 + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;
                    }
                    else
                    {
                        x0 = x0 + stepX;
                        tMaxX = tMaxX + tDeltaX;
                        z0 = z0 + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;
                    }
                }
                else if (tMaxX > tMaxY)
                {
                    if (tMaxY < tMaxZ)
                    {
                        y0 = y0 + stepY;
                        tMaxY = tMaxY + tDeltaY;
                    }
                    else if (tMaxY > tMaxZ)
                    {
                        z0 = z0 + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;
                    }
                    else
                    {
                        y0 = y0 + stepY;
                        tMaxY = tMaxY + tDeltaY;
                        z0 = z0 + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;

                    }
                }
                else
                {
                    if (tMaxY < tMaxZ)
                    {
                        y0 = y0 + stepY;
                        tMaxY = tMaxY + tDeltaY;
                        x0 = x0 + stepX;
                        tMaxX = tMaxX + tDeltaX;
                    }
                    else if (tMaxY > tMaxZ)
                    {
                        z0 = z0 + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;
                    }
                    else
                    {
                        x0 = x0 + stepX;
                        tMaxX = tMaxX + tDeltaX;
                        y0 = y0 + stepY;
                        tMaxY = tMaxY + tDeltaY;
                        z0 = z0 + stepZ;
                        tMaxZ = tMaxZ + tDeltaZ;

                    }
                }

                voxelMesh.SetVoxelVisible(new Vector3Int(x0, y0, z0), false);
            }
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

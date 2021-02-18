using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{
    public class Sphere : Shape
    {
        // Start is called before the first frame update
        public override void OnClickRelease(RaycastHit hit)
        {
            Vector3Int min = Vector3Int.zero;
            Vector3Int max = Vector3Int.zero;
            GetMinMaxSelection(startPos, lastPos, ref min, ref max);
            Undo.RecordObject(voxelMesh, "Sphere VoxelMesh");
            EditorUtility.SetDirty(voxelMesh);
            MakeSphere(min, max);
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

        List<Vector3Int> GetSphere(int width, int height, int depth)
        {
            List<Vector3Int> points = new List<Vector3Int>();


            //float ratioXZ = ((float)width) / ((float)depth);
            //float ratioXY = ((float)width) / ((float)height);
            //float radius = width / 2f;

            //Vector3 center = new Vector3(radius, height/2f, depth/2f);

            //for (int x = 0 ; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        for (int z = 0; z < depth; z++)
            //        {
            //            Vector3 pos = new Vector3(x, y * ratioXY, z * ratioXZ);
            //            pos -= center;
            //            if (pos.sqrMagnitude < radius * radius)
            //            {
            //                points.Add(new Vector3Int(x, y, z));
            //            }
            //        }
            //    }
            //}

            float ratioZX = (float)width / (float)depth;
            float ratioZY = (float)height / (float)depth;
            float radius = ((float)depth) / 2f;


            int dd = depth % 2 == 0 ? 1 : 0;
            int dx = width % 2 == 0 ? 1 : 0;
            int dy = height % 2 == 0 ? 1 : 0;


            int d = depth / 2;
            int cd = depth / 2;



            for (int i = 0; i <= d - dd; i++)
            {

                float w = Mathf.Sqrt((radius * radius) - (i * i)) * ratioZX * 2f;


                float h = Mathf.Sqrt((radius * radius) - (i * i)) * ratioZY * 2f;

                int wInt = ArroundToInt(w);

                if (wInt % 2 == dx)
                    wInt -= 1;


                int hInt = ArroundToInt(h);
                if (hInt % 2 == dy)
                    hInt -= 1;

                //Debug.Log("w:" + w.ToString());
                //Debug.Log("Arround:" + wInt.ToString());



                List<Vector2Int> po = GetEllipse(wInt, hInt);
                foreach (var item in po)
                {
                    int cx = (width / 2 - wInt / 2);
                    int cy = (height / 2 - hInt / 2);

                    points.Add(new Vector3Int(cx + item.x, cy + item.y, cd + i));
                    points.Add(new Vector3Int(cx + item.x, cx + item.y, cd - i - dd));

                }

            }


            return points;
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



        void MakeSphere(Vector3Int start, Vector3Int end)
        {
            Vector3Int size = end - start + Vector3Int.one;
            Vector3Int normalInt = new Vector3Int(
               Mathf.Abs(Mathf.RoundToInt(normal.x)),
                Mathf.Abs(Mathf.RoundToInt(normal.y)),
                Mathf.Abs(Mathf.RoundToInt(normal.z)));

            VoxelMesh.Direction dir = VoxelMesh.Direction.MAX;

            List<Vector3Int> points = null;
            if (normalInt.x != 0)
            {
                dir = VoxelMesh.Direction.Right;
                points = GetSphere(size.z, size.y, size.x);
            }
            else if (normalInt.y != 0)
            {
                dir = VoxelMesh.Direction.Up;
                points = GetSphere(size.x, size.z, size.y);
            }
            else if (normalInt.z != 0)
            {
                dir = VoxelMesh.Direction.Front;
                points = GetSphere(size.x, size.y, size.z);
            }

            foreach (var item in points)
            {
                VoxelData voxel = null;
                if (dir == VoxelMesh.Direction.Right)
                {
                    voxel = voxelMesh.GetVoxel(start.x + item.z, start.y + item.y, start.z + item.x);
                }
                else if (dir == VoxelMesh.Direction.Up)
                {
                    voxel = voxelMesh.GetVoxel(start.x + item.x, start.y + item.z, start.z + item.y);
                }
                else if (dir == VoxelMesh.Direction.Front)
                {
                    voxel = voxelMesh.GetVoxel(start.x + item.x, start.y + item.y, start.z + item.z);
                }


                if (voxel != null)
                {
                    //Debug.Log(item + start);


                    voxel.Copy(voxelMesh.voxelTemplate);
                    voxelMesh.SetVoxelVisible(voxel.Position, mode != ToolMode.Sub);
                }
            }

        }

        int ArroundToInt(float value)
        {
            return Mathf.CeilToInt(value - 0.5f);
        }
    }
}
using Edivox.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edivox.Runtime
{

    public class ExportSettings
    {
        public Vector3 pivot;
        public Vector3 voxelSize;

        public ExportSettings()
        {
            pivot = Vector3.one / 2f;
            voxelSize = Vector3.one;
        }

    }

    public class VoxelMeshExport
    {

        ExportSettings settings;
        Vector2 tileSize = Vector2.zero;
        Vector3 centerPos;

        public VoxelMeshExport(ExportSettings exportSettings)
        {
            settings = exportSettings;
        }

        public void GenerateMesh(VoxelMesh voxelMesh, MeshFilter filter, MeshCollider collider = null)
        {

            if (settings == null)
                return;
            MeshData meshData = new MeshData();
            meshData.useRenderDataForCol = collider != null;
            tileSize.x = 1f / voxelMesh.ColorPalette.colors.Count;
            tileSize.y = 1f;

            centerPos = new Vector3(voxelMesh.meshSize.x * settings.voxelSize.x * settings.pivot.x,
                voxelMesh.meshSize.y * settings.voxelSize.y * settings.pivot.y,
               voxelMesh.meshSize.z * settings.voxelSize.z * settings.pivot.z);

            for (int x = 0; x < voxelMesh.meshSize.x; x++)
            {
                for (int y = 0; y < voxelMesh.meshSize.y; y++)
                {
                    for (int z = 0; z < voxelMesh.meshSize.z; z++)
                    {
                        AddVoxelToMesh(new Vector3Int(x, y, z), voxelMesh, meshData);
                    }
                }
            }

            filter.sharedMesh.Clear();
            filter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            filter.sharedMesh.vertices = meshData.vertices.ToArray();
            filter.sharedMesh.triangles = meshData.triangles.ToArray();
            filter.sharedMesh.uv = meshData.uv.ToArray();
            filter.sharedMesh.RecalculateNormals();
            filter.sharedMesh.RecalculateTangents();
            filter.sharedMesh.Optimize();
            if (meshData.useRenderDataForCol)
            {
                collider.sharedMesh = null;
                Mesh mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                mesh.vertices = meshData.colVertices.ToArray();
                mesh.triangles = meshData.colTriangles.ToArray();
                mesh.RecalculateNormals();
                collider.sharedMesh = mesh;
            }

        }

        void AddVoxelToMesh(Vector3Int posInt, VoxelMesh voxelMesh, MeshData meshData)
        {
            VoxelData voxel = voxelMesh.GetVoxel(posInt);
            if (voxel == null)
                return;

            Vector3 pos = new Vector3(posInt.x * settings.voxelSize.x, posInt.y * settings.voxelSize.y, posInt.z * settings.voxelSize.z);
            pos -= centerPos;

            if (!voxel.IsVisible)
            {
                return;
            }

            VoxelData voxelUp = voxelMesh.GetVoxel(posInt.x, posInt.y + 1, posInt.z);
            if (voxelUp == null || !voxelUp.IsVisible)
            {
                //UP
                meshData.AddVertex(new Vector3(pos.x, pos.y + settings.voxelSize.y, pos.z + settings.voxelSize.z));
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y + settings.voxelSize.y, pos.z + settings.voxelSize.z));
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y + settings.voxelSize.y, pos.z));
                meshData.AddVertex(new Vector3(pos.x, pos.y + settings.voxelSize.y, pos.z));
                meshData.AddQuadTriangles();
                SetUVsVoxel(voxel, meshData.uv);

            }

            VoxelData voxelDown = voxelMesh.GetVoxel(posInt.x, posInt.y - 1, posInt.z);
            if (voxelDown == null || !voxelDown.IsVisible)
            {
                //Down
                meshData.AddVertex(new Vector3(pos.x, pos.y, pos.z));
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y, pos.z));
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y, pos.z + settings.voxelSize.z));
                meshData.AddVertex(new Vector3(pos.x, pos.y, pos.z + settings.voxelSize.z));
                meshData.AddQuadTriangles();
                SetUVsVoxel(voxel, meshData.uv);
            }

            VoxelData voxelFront = voxelMesh.GetVoxel(posInt.x, posInt.y, posInt.z + 1);
            if (voxelFront == null || !voxelFront.IsVisible)
            {
                //Front
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y, pos.z + settings.voxelSize.z));
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y + settings.voxelSize.y, pos.z + settings.voxelSize.z));
                meshData.AddVertex(new Vector3(pos.x, pos.y + settings.voxelSize.y, pos.z + settings.voxelSize.z));
                meshData.AddVertex(new Vector3(pos.x, pos.y, pos.z + settings.voxelSize.z));

                meshData.AddQuadTriangles();
                SetUVsVoxel(voxel, meshData.uv);
            }

            VoxelData voxelBack = voxelMesh.GetVoxel(posInt.x, posInt.y, posInt.z - 1);
            if (voxelBack == null || !voxelBack.IsVisible)
            {
                //Back
                meshData.AddVertex(new Vector3(pos.x, pos.y, pos.z));
                meshData.AddVertex(new Vector3(pos.x, pos.y + settings.voxelSize.y, pos.z));
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y + settings.voxelSize.y, pos.z));
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y, pos.z));

                meshData.AddQuadTriangles();
                SetUVsVoxel(voxel, meshData.uv);
            }

            VoxelData voxelRight = voxelMesh.GetVoxel(posInt.x + 1, posInt.y, posInt.z);
            if (voxelRight == null || !voxelRight.IsVisible)
            {
                //Right
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y, pos.z));
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y + settings.voxelSize.y, pos.z));
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y + settings.voxelSize.y, pos.z + settings.voxelSize.z));
                meshData.AddVertex(new Vector3(pos.x + settings.voxelSize.x, pos.y, pos.z + settings.voxelSize.z));

                meshData.AddQuadTriangles();
                SetUVsVoxel(voxel, meshData.uv);
            }

            VoxelData voxelLeft = voxelMesh.GetVoxel(posInt.x - 1, posInt.y, posInt.z);
            if (voxelLeft == null || !voxelLeft.IsVisible)
            {
                //Left
                meshData.AddVertex(new Vector3(pos.x, pos.y, pos.z + settings.voxelSize.z));
                meshData.AddVertex(new Vector3(pos.x, pos.y + settings.voxelSize.y, pos.z + settings.voxelSize.z));
                meshData.AddVertex(new Vector3(pos.x, pos.y + settings.voxelSize.y, pos.z));
                meshData.AddVertex(new Vector3(pos.x, pos.y, pos.z));

                meshData.AddQuadTriangles();
                SetUVsVoxel(voxel, meshData.uv);
            }

        }

        void SetUVsVoxel(VoxelData voxel, List<Vector2> uv)
        {
            Vector2 tilePos = new Vector2(voxel.colorId, 0f);

            //uv.Add(new Vector2(tileSize.x * tilePos.x + tileSize.x,
            //    tileSize.y * tilePos.y));
            //uv.Add(new Vector2(tileSize.x * tilePos.x + tileSize.x,
            //    tileSize.y * tilePos.y + tileSize.y));
            //uv.Add(new Vector2(tileSize.x * tilePos.x,
            //    tileSize.y * tilePos.y + tileSize.y));
            //uv.Add(new Vector2(tileSize.x * tilePos.x,
            //    tileSize.y * tilePos.y));

            uv.Add(new Vector2(tileSize.x * tilePos.x + tileSize.x - 0.001f,
        tileSize.y * tilePos.y + 0.001f));
            uv.Add(new Vector2(tileSize.x * tilePos.x + tileSize.x - 0.001f,
                tileSize.y * tilePos.y + tileSize.y - 0.001f));
            uv.Add(new Vector2(tileSize.x * tilePos.x + 0.001f,
                tileSize.y * tilePos.y + tileSize.y - 0.001f));
            uv.Add(new Vector2(tileSize.x * tilePos.x + 0.001f,
                tileSize.y * tilePos.y + 0.001f));

        }
    }
}

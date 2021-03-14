using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Edivox.Runtime
{

    public class VoxelMesh : MonoBehaviour
    {

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            Front,
            Back,
            MAX
        }

        static public Vector3 GetVectorFromDirection(Direction _dir)
        {
            switch (_dir)
            {
                case Direction.Up:
                    return Vector3.up;
                case Direction.Down:
                    return Vector3.down;
                case Direction.Left:
                    return Vector3.left;
                case Direction.Right:
                    return Vector3.right;
                case Direction.Front:
                    return Vector3.forward;
                case Direction.Back:
                    return Vector3.back;
                default:
                    return Vector3.zero;
            }
        }

        public MultiArrayVoxelData voxelsArray;
        public Vector3Int meshSize;
        public VoxelData voxelTemplate;
        public ColorPalette colorPalette;
        public ColorPalette ColorPalette { get => colorPalette; set => colorPalette = value; }
        public MeshFilter meshFiltrer = null;
        public MeshRenderer meshRenderer = null;
        public MeshCollider meshCollider = null;
        public ExportSettings meshSettings = null;
        public VoxelMeshExport meshExport = null;

        bool voxelModified = true;

        void Start()
        {

        }


        void OnDestroy()
        {

        }

        public void Init(Vector3Int _size, bool isFill, ColorPalette colors)
        {

            if (colors == null)
            {
                colorPalette = ScriptableObject.CreateInstance<ColorPalette>();
                colorPalette.AddColor(Color.white);
            }
            else
            {
                colorPalette = colors;
            }

            voxelTemplate = new VoxelData();
            voxelTemplate.IsVisible = isFill;
            voxelTemplate.colorId = 0;
            meshSize = _size;

            voxelsArray = new MultiArrayVoxelData(meshSize.x, meshSize.y, meshSize.z);


            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    for (int z = 0; z < _size.z; z++)
                    {

                        VoxelData voxel = voxelsArray.GetVoxel(x, y, z);
                        voxel.Copy(voxelTemplate);
                        voxel.Position = new Vector3Int(x, y, z);
                    }
                }
            }
            meshFiltrer = gameObject.AddComponent<MeshFilter>();
            meshFiltrer.sharedMesh = new Mesh();
            meshFiltrer.sharedMesh.name = gameObject.name;
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            Material mat = new Material(Shader.Find("Diffuse"));
            mat.mainTexture = colorPalette.CreateTexture();
            meshRenderer.material = mat;
            meshCollider = gameObject.AddComponent<MeshCollider>();
            RefreshMesh();
        }

        public bool IsInside(int _x, int _y, int _z)
        {
            return _x >= 0 && _x < meshSize.x &&
                _y >= 0 && _y < meshSize.y &&
                _z >= 0 && _z < meshSize.z;
        }

        public bool IsInside(Vector3Int _pos)
        {
            return _pos.x >= 0 && _pos.x < meshSize.x &&
                _pos.y >= 0 && _pos.y < meshSize.y &&
                _pos.z >= 0 && _pos.z < meshSize.z;
        }

        public VoxelData GetVoxel(int _x, int _y, int _z)
        {
            return voxelsArray.GetVoxel(_x, _y, _z);
        }

        public VoxelData GetVoxel(Vector3Int _pos)
        {
            return voxelsArray.GetVoxel(_pos);
        }

        public VoxelData GetVoxel(Vector3 _worldPos)
        {
            return voxelsArray.GetVoxel(_worldPos);
        }


        //public void UpdateVoxel(Vector3Int _pos)
        //{
        //    VoxelData voxel = GetVoxel(_pos);
        //    if (voxel != null)
        //    {
        //        voxel.Copy(voxelTemplate);
        //        voxel.render.UpdateVoxelRender();
        //    }
        //}

        public void SetVoxel(Vector3Int _pos, VoxelData _data)
        {
            if (_data == null)
                return;

            VoxelData voxel = GetVoxel(_pos);
            if (voxel != null)
            {
                voxel.Copy(_data);
                voxelModified = true;
                //voxel.render.UpdateVoxelRender();
            }
        }

        public void SetVoxelVisible(Vector3Int _pos, bool _visible)
        {
            VoxelData voxel = GetVoxel(_pos);
            if (voxel != null)
            {
                voxel.IsVisible = _visible;
                voxelModified = true;
                //voxel.render.UpdateVoxelRender();
            }
        }

        public void Refresh(bool forceRefreshMesh = false)
        {
            if (colorPalette != null)
            {
                if (!colorPalette.IsColorValid(voxelTemplate.colorId))
                {
                    voxelTemplate.colorId = 0;
                }
            }

            Debug.Log("RefreshObject");

            meshFiltrer = gameObject.GetComponent<MeshFilter>();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            Material mat = new Material(Shader.Find("Diffuse"));
            mat.mainTexture = colorPalette.CreateTexture();
            meshRenderer.material = mat;
            meshCollider = gameObject.GetComponent<MeshCollider>();
            RefreshMesh(forceRefreshMesh);
        }

        public void Fill(bool _visible)
        {

            for (int x = 0; x < meshSize.x; x++)
            {
                for (int y = 0; y < meshSize.y; y++)
                {
                    for (int z = 0; z < meshSize.z; z++)
                    {
                        VoxelData vo = voxelsArray.GetVoxel(x, y, z);

                        vo.IsVisible = _visible;
                        vo.colorId = voxelTemplate.colorId;
                        voxelModified = true;
                        //vo.render.UpdateVoxelRender();
                    }

                }
            }

        }

        public void RefreshMesh(bool forced = false)
        {
            if (meshSettings == null)
            {
                meshSettings = new ExportSettings();
                meshSettings.pivot = Vector3.zero;
            }

            if (meshExport == null)
            {
                meshExport = new VoxelMeshExport(meshSettings);
            }

            if ((voxelModified || forced) && meshExport != null)
            {

                meshExport.GenerateMesh(this, meshFiltrer, meshCollider);
                voxelModified = false;
                Debug.Log("RefreshMesh");

            }
        }

    }
}



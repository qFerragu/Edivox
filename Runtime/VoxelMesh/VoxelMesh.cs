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

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
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

                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        go.transform.parent = transform;
                        Vector3Int position = new Vector3Int(x, y, z);
                        go.transform.position = position;
                        VoxelRender render = go.AddComponent<VoxelRender>();
                        render.voxelData = voxel;

                        voxel.Position = position;
                        voxel.render = render;
                        render.voxelMesh = this;
                        render.UpdateVoxelRender();
                    }

                }
            }
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


        public void UpdateVoxel(Vector3Int _pos)
        {
            VoxelData voxel = GetVoxel(_pos);
            if (voxel != null)
            {
                voxel.Copy(voxelTemplate);
                voxel.render.UpdateVoxelRender();
            }
        }

        public void SetVoxel(Vector3Int _pos, VoxelData _data)
        {
            if (_data == null)
                return;

            VoxelData voxel = GetVoxel(_pos);
            if (voxel != null)
            {
                voxel.Copy(_data);
                voxel.render.UpdateVoxelRender();
            }
        }

        public void SetVoxelVisible(Vector3Int _pos, bool _visible)
        {
            VoxelData voxel = GetVoxel(_pos);
            if (voxel != null)
            {
                voxel.IsVisible = _visible;
                voxel.render.UpdateVoxelRender();
            }
        }

        public void Refresh()
        {
            if (colorPalette != null)
            {
                if (!colorPalette.IsColorValid(voxelTemplate.colorId))
                {
                    voxelTemplate.colorId = 0;
                }
            }

            for (int x = 0; x < meshSize.x; x++)
            {
                for (int y = 0; y < meshSize.y; y++)
                {

                    for (int z = 0; z < meshSize.z; z++)
                    {
                        VoxelData vo = voxelsArray.GetVoxel(x, y, z);

                        if (vo.render.voxelData != vo)
                        {
                            vo.render.voxelData = vo;
                        }
                        vo.render.UpdateVoxelRender();
                    }
                }
            }
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
                        vo.render.UpdateVoxelRender();
                    }

                }
            }

        }

    }
}



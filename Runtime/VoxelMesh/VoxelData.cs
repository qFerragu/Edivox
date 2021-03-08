using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edivox.Runtime
{
    [Serializable]
    public class VoxelData
    {
        [SerializeField]
        Vector3Int position;

        [SerializeField]
        bool isVisible = true;

        public Vector3Int Position { get => position; set => position = value; }
        public bool IsVisible { get => isVisible; set => isVisible = value; }
        public int colorId;
        //public VoxelRender render;

        static public VoxelData Clone(VoxelData _voxel)
        {
            VoxelData newVoxel = new VoxelData();
            newVoxel.colorId = _voxel.colorId;
            newVoxel.isVisible = _voxel.isVisible;
            return newVoxel;
        }

        public void Copy(VoxelData _voxel)
        {
            if (_voxel == null)
                return;

            isVisible = _voxel.isVisible;
            colorId = _voxel.colorId;
        }
    }


    [Serializable]
    public class MultiArrayVoxelData
    {
        public VoxelData[] voxels;
        public int width, height, length;

        public MultiArrayVoxelData(int _width, int _height, int _length)
        {
            width = _width;
            height = _height;
            length = _length;
            voxels = new VoxelData[width * height * length];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        voxels[(z * height + y) * width + x] = new VoxelData();
                    }
                }
            }
        }


        public bool IsInside(int _x, int _y, int _z)
        {
            return _x >= 0 && _x < width &&
                _y >= 0 && _y < height &&
                _z >= 0 && _z < length;
        }

        public bool IsInside(Vector3Int _pos)
        {
            return _pos.x >= 0 && _pos.x < width &&
                _pos.y >= 0 && _pos.y < height &&
                _pos.z >= 0 && _pos.z < length;
        }

        public VoxelData GetVoxel(int _x, int _y, int _z)
        {
            if (IsInside(_x, _y, _z))
            {
                return voxels[(_z * height + _y) * width + _x];
            }

            return null;
        }

        public VoxelData GetVoxel(Vector3Int _pos)
        {
            if (IsInside(_pos))
            {

                return voxels[(_pos.z * height + _pos.y) * width + _pos.x];
            }
            return null;
        }

        public VoxelData GetVoxel(Vector3 _worldPos)
        {
            Vector3Int pos = new Vector3Int(
                Mathf.FloorToInt(_worldPos.x),
                Mathf.FloorToInt(_worldPos.y),
                Mathf.FloorToInt(_worldPos.z));

            return GetVoxel(pos);
        }


    }

}
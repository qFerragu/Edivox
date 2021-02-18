using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edivox.Runtime
{
    public class VoxelRender : MonoBehaviour
    {
        public VoxelData voxelData;
        public VoxelMesh voxelMesh;

        MeshRenderer meshRenderer;
        [SerializeField]
        Material matInstance;
        // Start is called before the first frame update
        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateVoxelRender()
        {

            gameObject.SetActive(voxelData.IsVisible);
            if (voxelData.IsVisible && voxelMesh != null && voxelMesh.ColorPalette != null)
            {
                if (matInstance == null)
                {
                    Material sharedMat = GetComponent<MeshRenderer>().sharedMaterial;
                    matInstance = new Material(sharedMat);
                }
                if (!voxelMesh.ColorPalette.IsColorValid(voxelData.colorId))
                {
                    voxelData.colorId = 0;
                }

                matInstance.color = voxelMesh.ColorPalette.GetColor(voxelData.colorId);
                GetComponent<MeshRenderer>().sharedMaterial = matInstance;
            }
        }

    }
}

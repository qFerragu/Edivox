using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Edivox.Runtime;

namespace Edivox.Editor
{
    [CustomEditor(typeof(VoxelData))]
    public class VoxelDataEditor : UnityEditor.Editor
    {
        void OnSceneGUI()
        {
            //HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }


    }
}

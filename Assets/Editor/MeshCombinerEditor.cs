using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor {
    void OnSceneGUI()
    {
        MeshCombiner mc = target as MeshCombiner;
        if (Handles.Button(mc.transform.position + Vector3.up * .5f, Quaternion.LookRotation(Vector3.up), 1, 1, Handles.CylinderHandleCap))
        {
            mc.CombineMeshes();
        }

    }
}

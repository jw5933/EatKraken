using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Location))]
public class LocationEditor : Editor
{

    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        serializedObject.Update();
        Location i = (Location) target;

        if (GUILayout.Button("Set Modified Variables")){
            serializedObject.ApplyModifiedProperties();
        }
    }
}

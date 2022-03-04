using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Customer))]
public class CustomerEditor : Editor
{

    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        serializedObject.Update();
        Customer i = (Customer) target;

        if (GUILayout.Button("Set Modified Variables")){
            serializedObject.ApplyModifiedProperties();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ingredient))]
public class IngredientEditor : Editor
{
    SerializedProperty state;
    SerializedProperty imgStates;
    SerializedProperty motions;

    void OnEnable(){
        state = serializedObject.FindProperty("myImageState");
        imgStates = serializedObject.FindProperty("imageStates");
        motions = serializedObject.FindProperty("myMotionsLeft");
    }

    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        serializedObject.Update();
        Ingredient i = (Ingredient) target;

        if (GUILayout.Button("Set Initial State")){
            i.OnEnable();
            state.intValue = -1;
            motions.intValue = 0;
            serializedObject.ApplyModifiedProperties();
            i.ChangeState();
        }
        if (GUILayout.Button("Set End State")){
            i.OnEnable();
            state.intValue = imgStates.arraySize-2;
            motions.intValue = 0;
            serializedObject.ApplyModifiedProperties();
            i.InactivateToolLines();
            i.ChangeState();
        }
    }
}

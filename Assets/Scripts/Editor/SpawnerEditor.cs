using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IngredientSpawner))]
public class SpawnerEditor : Editor
{
    public override void OnInspectorGUI(){
        IngredientSpawner s = (IngredientSpawner) target;

        if(GUILayout.Button("Spawn Ingredient")){
            s.SpawnIngredient();
        }
    }
}

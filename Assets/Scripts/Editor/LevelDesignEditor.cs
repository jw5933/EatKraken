using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(LevelDesignScript))]
public class LevelDesignEditor : Editor
{
    List<string> labels = new List<string>();
    Dictionary <int, Location> locMap = new Dictionary<int, Location>();
    int index = 0;
    Object currLoc;
    LevelDesignScript level;
    
    SerializedProperty phases;
    private ReorderableList list;

    public override void OnInspectorGUI(){
        //gameInfo
        DrawDefaultInspector();

        level = (LevelDesignScript)target;
        

        /*if (GUILayout.Button("Wake Up Level")){
            level.Awake();
        }
        
        if (GUILayout.Button("Update Locations")){
            labels.Clear();
            locMap.Clear();
            Location[] locations = FindObjectsOfType<Location>(true);
            int i = 0;
            foreach(Location l in locations){
                labels.Add(l.gameObject.name);
                locMap.Add(i++, l);
            }
            index = labels.IndexOf(level.GetLocationName());
        }

        index = EditorGUILayout.Popup("Locations", index, labels.ToArray());
        currLoc = EditorGUILayout.ObjectField(currLoc, typeof(Location), false);

        if (GUILayout.Button("Set New Location")){
            UpdateLocation();
        }

        
        if (GUILayout.Button("Go To Next Phase")){

        }

        if (GUILayout.Button("Go To Previous Phase")){

        }*/
    }

    void UpdateLocation(){
        Debug.Log(index);
        if (level != null) level.GoNextLocation(locMap[index]);
        Debug.Log("editor will update the location");
    }
    
}

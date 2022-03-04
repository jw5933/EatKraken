using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Phase))]
public class PhaseDrawer : PropertyDrawer
{
    public bool showPhases = true;
    public string status = "Select a GameObject";

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.LabelField("Phase #: ", property.FindPropertyRelative("phaseNum").intValue.ToString());
        GUILayout.Label("Economy (Coins) Overview");
        EditorGUILayout.LabelField("Max", property.FindPropertyRelative("moneyMax").floatValue.ToString());
        EditorGUILayout.LabelField("Earned", property.FindPropertyRelative("moneyEarned").floatValue.ToString());
        EditorGUILayout.LabelField("Lost", property.FindPropertyRelative("moneyLost").floatValue.ToString());
        GUILayout.Space(10);
        GUILayout.Label("Customer Overview");
        EditorGUILayout.LabelField("Total", property.FindPropertyRelative("customerMax").intValue.ToString());
        EditorGUILayout.LabelField("Served", property.FindPropertyRelative("customersServed").intValue.ToString());
        EditorGUILayout.LabelField("Left", property.FindPropertyRelative("customersLeft").intValue.ToString());
        GUILayout.Space(40);
    }
}
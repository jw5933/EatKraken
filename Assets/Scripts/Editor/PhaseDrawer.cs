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
        GUILayout.Label("Economy");
        EditorGUILayout.LabelField("Max Coins", property.FindPropertyRelative("moneyMax").floatValue.ToString());
        EditorGUILayout.LabelField("Coins Earned", property.FindPropertyRelative("moneyEarned").floatValue.ToString());
        EditorGUILayout.LabelField("Coins Not Earned", property.FindPropertyRelative("moneyLost").floatValue.ToString());
        GUILayout.Space(10);
        GUILayout.Label("Customer");
        EditorGUILayout.LabelField("Customers This Phase", property.FindPropertyRelative("customerMax").intValue.ToString());
        EditorGUILayout.LabelField("Customers Served", property.FindPropertyRelative("customersServed").intValue.ToString());
        EditorGUILayout.LabelField("Customers Left", property.FindPropertyRelative("customersLeft").intValue.ToString());
        GUILayout.Space(40);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    // ==============   variables   ==============
    public enum Required {None, Slice}
    [SerializeField] private Required myRequired = Required.None;

    [SerializeField] private SharedArea myArea;
    public SharedArea area{get{return myArea;} set{myArea = value;}}

    private Player player;

    private List<Ingredient> hovered = new List<Ingredient>();

    // ==============   functions   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
    }
    //pick up item
    private void OnMouseDown(){
        //Debug.Log(this.gameObject.name);
        
        if (!player.handFree) return;
        if (myArea != null) myArea.HandlePickUp();
        player.PickUpItem(this.gameObject);
    }

    public bool ValidateMotion(Ingredient i){
        if (myRequired == i.required) return true;
        return false;
    }

    public void AddToHovered(Ingredient i){
        hovered.Add(i);
    }
    
    public void ResetVars(){
        foreach(Ingredient i in hovered){
            i.ResetVars();
        }
        hovered.Clear();
    }
}

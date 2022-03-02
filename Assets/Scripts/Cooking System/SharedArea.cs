using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedArea: MonoBehaviour
{
    public enum AreaType {None, CuttingBoard, Base, IngredientOnly}
    [SerializeField] AreaType myType = AreaType.None;
    public AreaType type {get {return myType;}}

    private GameObject myItem;

    private bool freeArea = true;
    private Player player;
    private GameManager gm;

    // ==============   functions   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<GameManager>();
    }

    public void OnMouseDown(){
        Debug.Log(this.name);

        if(player!=null && !player.handFree){
            if (freeArea) PlaceObjectOnShared();
        }
    }

    // public void OnMouseOver(){
    //     //turn green if free area; else red
    // }

    // public void OnMouseExit(){
    //     //turn off alpha
    // }

    //shared board
    private void PlaceObjectOnShared(){
        if (myType == AreaType.IngredientOnly|| myType == AreaType.CuttingBoard){
            myItem = player.DropItem("ingredient");
            if (myItem == null) return;
            
            Ingredient i = myItem.GetComponent<Ingredient>();
            i.ResetVars();
            i.area = this;
        }
        else if (myType == AreaType.Base){
            myItem = player.DropItem("base");
            if (myItem == null) return;
            myItem.GetComponent<DragDropObject>().area = this;
        }
        else{
            myItem = player.DropItem("tool");
            if (myItem == null) return;
            
            Tool t = myItem.GetComponent<Tool>();
            t.ResetVars();
            t.area = this;
        }
        freeArea = false;
        myItem.transform.position = this.transform.position + (gm.in3d? new Vector3(0,0.1f,0): new Vector3(0,0,-1));
    }

    public void HandlePickUp(){
        freeArea = true;
        myItem = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SharedArea: MonoBehaviour
{
    public enum AreaType {None, CuttingBoard, Base, IngredientOnly}
    [SerializeField] AreaType myType = AreaType.None;
    public AreaType type {get {return myType;}}

    [SerializeField] private GameObject myItem;
    private Appliance myAppliance;

    private bool freeArea = true;
    private Player player;
    private GameManager gm;

    Collider myCollider;

    public UnityEvent FreedAreaEvent = new UnityEvent();

    // ==============   functions   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<GameManager>();
        myCollider = GetComponent<Collider>();
        InitialSnapToArea();
    }

    private void InitialSnapToArea(){
        if (myItem !=null) {
            freeArea = false;
            myCollider.enabled = false;
            myItem.transform.position = myCollider.bounds.center; // + (gm.in3d? new Vector3(0,0,-0.01f): new Vector3(0,0,-1));
            BaseObject o = myItem.GetComponent<BaseObject>();
            if (o !=null) o.area = this;
        }
    }

    public void OnMouseDown(){
        if (!freeArea) return;
        if(player!=null && !player.handFree){
            PlaceObjectOnShared();

            if(myAppliance !=null){
                myAppliance.OnMouseDown();
            }
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
            myItem.GetComponent<BaseObject>().area = this;
        }
        else{
            myItem = player.DropItem("tool");
            if (myItem == null) return;
            
            Tool t = myItem.GetComponent<Tool>();
            t.ResetVars();
            t.area = this;
        }
        freeArea = false;
        myCollider.enabled = false;
        myItem.transform.position = myCollider.bounds.center; // + (gm.in3d? new Vector3(0,0,-0.01f): new Vector3(0,0,-1));
    }

    public void HandlePickUp(){
        freeArea = true;
        FreedAreaEvent.Invoke();
        myCollider.enabled = true;
        myItem = null;
    }
}

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

    private bool freeArea = true;
    public bool free {get{return freeArea;}}
    private Player player;
    private GameManager gm;

    Collider myCollider;

    //[HideInInspector] public UnityEvent FreedAreaEvent = new UnityEvent();

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
            if (myCollider !=null){
                myCollider.enabled = false;
                myItem.transform.position = myCollider.bounds.center; // + (gm.in3d? new Vector3(0,0,-0.01f): new Vector3(0,0,-1));
            }
            BaseObject o = myItem.GetComponent<BaseObject>();
            if (o !=null) o.area = this;
        }
    }

    public void OnMouseDown(){
        if (!freeArea) return;
        if(player!=null && !player.handFree){
            PlaceObjectOnShared();
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
            if (!player.holdingIngredient) return;
            myItem = player.DropItem("ingredient");
            Ingredient i = myItem.GetComponent<Ingredient>();

            if (i.type == Ingredient.Type.Base){
                player.PickUpItem(myItem);
                myItem = null;
                return;
            }

            i.ResetVars();
            i.area = this;
            i.SetParent(this.transform.parent);
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
        if (myCollider !=null) {
            myCollider.enabled = false;
            myItem.transform.position = myCollider.bounds.center; // + (gm.in3d? new Vector3(0,0,-0.01f): new Vector3(0,0,-1));
        }
    }

    public void HandlePickUp(){
        myItem = null;
        freeArea = true;
        //FreedAreaEvent.Invoke();
        if (myCollider !=null) myCollider.enabled = true;
    }

    public bool CheckSwapIngredient(){
        Debug.Log("check swap");
        if (!player.holdingIngredient || (myType != AreaType.IngredientOnly && myType != AreaType.CuttingBoard)) return false;
        //see if the player is holding an ingredient
        Ingredient i = player.DropItem("ingredient").GetComponent<Ingredient>();
        Debug.Log(i.name);
        player.PickUpItem(myItem);
        myItem = i.gameObject;

        i.ResetVars();
        i.area = this;
        i.SetParent(this.transform.parent);
        myItem.transform.position = myCollider.bounds.center;

        return true;
    }
}

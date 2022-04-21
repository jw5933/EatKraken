using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SharedArea: MonoBehaviour
{
    public enum AreaType {None, CuttingBoard, BaseHolder, IngredientOnly, BaseObject}
    [SerializeField] AreaType myType = AreaType.None;
    public AreaType type {get {return myType;}}

    [SerializeField] private GameObject myItem;
    [SerializeField] private Transform overridePosition;
    [SerializeField] private float overrideScale3 = 1;
    private Vector3 overrideScale;
    [SerializeField] private Vector3 overrideAngle = Vector3.one;

    [SerializeField] private bool freeArea = true;
    public bool free {get{return freeArea;}}
    private Player player;
    private GameManager gm;

    Collider myCollider;

    // ==============   functions   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<GameManager>();
        myCollider = GetComponent<Collider>();
        InitialSnapToArea();
        overrideScale = Vector3.one * overrideScale3;
    }

    private void InitialSnapToArea(){
        if (myItem !=null) {
            freeArea = false;
            if (myCollider !=null){
                myCollider.enabled = false;
                myItem.transform.position = (overridePosition != null? overridePosition.position: myCollider.bounds.center); // + (gm.in3d? new Vector3(0,0,-0.01f): new Vector3(0,0,-1));
            }
            BaseHolder o = myItem.GetComponent<BaseHolder>();
            if (o !=null) o.area = this;
        }
    }

    public void OnMouseDown(){
        if (!freeArea) return;
        if(player!=null && !player.handFree){
            PlaceObjectOnShared();
        }
    }

    //shared board
    private void PlaceObjectOnShared(){
        if (player.holdingIngredient && (myType == AreaType.IngredientOnly|| myType == AreaType.CuttingBoard)){
            myItem = player.DropItem("ingredient");
            Ingredient i = myItem.GetComponent<Ingredient>();

            if (i.type == Ingredient.Type.Base){
                player.PickUpItem(myItem);
                myItem = null;
                return;
            }

            i.ResetVars();
            i.area = this;
            //i.SetParent(this.transform.parent);
            i.SetTransform(overrideScale, overrideAngle);
        }
        else if (player.holdingBase && (myType == AreaType.BaseHolder || myType == AreaType.BaseObject)){
            myItem = player.DropItem("base");
            myItem.GetComponent<BaseObject>().area = this;
        }
        /* else if (player.holdingTool){
            myItem = player.DropItem("tool");
            if (myItem == null) return;
            
            Tool t = myItem.GetComponent<Tool>();
            t.ResetVars();
            t.area = this;
        } */
        else return;
        freeArea = false;

        if (myCollider !=null) {
            myCollider.enabled = false;
            myItem.transform.position = (overridePosition != null? overridePosition.position: myCollider.bounds.center); // + (gm.in3d? new Vector3(0,0,-0.01f): new Vector3(0,0,-1));
        }
    }

    public void HandlePickUp(){
        myItem = null;
        freeArea = true;
        //FreedAreaEvent.Invoke();
        if (myCollider !=null) myCollider.enabled = true;
    }

    public bool CheckSwapIngredient(){
        //Debug.Log("check swap");
        if (!player.holdingIngredient || (myType != AreaType.IngredientOnly && myType != AreaType.CuttingBoard)) return false;
        //see if the player is holding an ingredient
        Ingredient i = player.DropItem("ingredient").GetComponent<Ingredient>();
        //Debug.Log(i.name);
        player.PickUpItem(myItem);
        myItem = i.gameObject;

        i.ResetVars();
        i.area = this;
        //i.SetParent(this.transform.parent);
        i.SetTransform(overrideScale, overrideAngle);
        myItem.transform.position = (overridePosition != null? overridePosition.position: myCollider.bounds.center);

        return true;
    }

    public void CheckSwapBaseObject(){
        GameObject o = player.DropItem("base");
        player.PickUpItem(myItem.gameObject);
        myItem.GetComponent<BaseObject>().area = null;

        myItem = o;
        myItem.GetComponent<BaseObject>().area = this;
        myItem.transform.position = (overridePosition != null? overridePosition.position: myCollider.bounds.center);
    }
}

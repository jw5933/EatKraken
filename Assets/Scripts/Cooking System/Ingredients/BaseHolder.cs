using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHolder : Draggable
{
    // ==============   variables   ==============
    [SerializeField] private float minPercentOfDist;
    private Collider myCollider;
    private Vector3 initialPos;

    private SharedArea myArea;
    public SharedArea area{set{myArea = value;}}

    bool finalizeOrder;
    bool addingObject;
    [SerializeField] private List <Vector3> ingredientPositions = new List<Vector3>();
    private int positionIndex;

    [SerializeField] private GameObject orderObject;
    public GameObject orderobj {get{return orderObject;}}
    
    private Animator anim;

    
    // ==============   functions   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        myCollider = GetComponent<Collider>();
        anim = GetComponent<Animator>();

        ResetVars();

        if (minPercentOfDist <= 0){
            finalizeOrder = false;
            return;
        }
        finalizeOrder = true;
        minDistance = Mathf.Max(myCollider.bounds.size.y, myCollider.bounds.size.x) * minPercentOfDist;
    }

    private void Start(){
        foreach(Transform child in transform){
            ingredientPositions.Add(child.position);
        }
    }

    private void OnMouseExit(){
        if (player.handFree) return;
    }

    private void OnMouseDown(){
        if (!player.handFree){
            if (player.holdingBase){
                player.DropItem("base");
                orderObject.transform.position = transform.position;
            }
            else {
                if (positionIndex >= ingredientPositions.Count) return;
                if (player.AddToCurrentOrder(ingredientPositions[positionIndex], this.transform.eulerAngles, orderObject != null? orderObject.transform: this.transform)){
                    if (orderObject !=null) orderObject.SetActive(true);
                    positionIndex++;
                }
                addingObject = true;
            }
        } 
        else{
            //pick up base
            if (finalizeOrder)
                initialPos = base.GetProjectionOnPlane();
            else
                HandlePickUp(orderObject != null? orderObject: this.gameObject);
        }
    }   

    private void OnMouseUpAsButton(){
        if (!player.handFree || !finalizeOrder || addingObject){
            addingObject = false;
            return;
        }
        Vector3 endPos = base.GetProjectionOnPlane();
        base.VerifyDistance(endPos, initialPos);
    }

    protected override void HandleDragged(){
        //FIX: change visual as the player drags
        AnimateFinalize();
    }

    protected override void HandleNotDragged(){
    }

    private void AnimateFinalize(){
        if (anim != null) anim.SetTrigger("Finalize");
    }

    public void HandleAnimation(){
        if (player.order.Count <= 0) return;
        orderObject.transform.position = transform.position;
        orderObject.SetActive(true);

        foreach(Ingredient i in player.order){
            i.HandleAddToOrder();
            i.transform.SetParent(orderObject.transform, true);
            i.transform.position = orderObject.transform.position;
        }
        HandlePickUp(orderObject);
    }

    private void HandlePickUp(GameObject o){
        if (player.order.Count <= 0) return;
        if (orderObject == null && myArea != null) myArea.HandlePickUp();
        player.PickUpItem(o);
    }

    public void ResetVars(){
        orderObject.SetActive(false);
        orderObject.transform.position = transform.position;
        positionIndex = 0;
    }
}

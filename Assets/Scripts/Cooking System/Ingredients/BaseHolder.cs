using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHolder : Draggable
{
    // ==============   variables   ==============
    [SerializeField] BaseObject baseObject;
    public bool hasBase{get{return baseObject != null;}}

    [SerializeField] private float minPercentOfDist;
    private Collider myCollider;
    private Vector3 initialPos;

    private SharedArea myArea;
    public SharedArea area{set{myArea = value;}}

    bool finalizeOrder;
    bool addingObject;

    private Animator anim;

    
    // ==============   functions   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        myCollider = GetComponent<Collider>();
        anim = GetComponent<Animator>();

        if (minPercentOfDist <= 0){
            finalizeOrder = false;
            return;
        }
        finalizeOrder = true;
        minDistance = Mathf.Max(myCollider.bounds.size.y, myCollider.bounds.size.x) * minPercentOfDist;
    }

    private void OnMouseUp(){
        if (!player.handFree){
            if (player.holdingBase){
                if (hasBase) CheckSwap();
                else
                    baseObject = player.DropItem("base").GetComponent<BaseObject>();
                baseObject.UnsetCollider();
                baseObject.transform.position = transform.position;
                baseObject.PlaySound();
            }
            else if (player.holdingIngredient){
                if (hasBase && baseObject.AddToOrder(this.transform.eulerAngles))
                    addingObject = true;
            }
        } 
        else{
            //pick up base
            if (finalizeOrder)
                initialPos = base.GetProjectionOnPlane();
            else
                HandlePickUp();
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
        if (player.baseObject.order.Count <= 0) return;
        baseObject.transform.position = transform.position;

        foreach(Ingredient i in player.baseObject.order){
            i.HandleAddToOrder();
            i.transform.SetParent(baseObject.transform, true);
            i.transform.position = baseObject.transform.position;
        }
        HandlePickUp();
    }

    private void HandlePickUp(){
        if (baseObject == null) return;
        //if (!hasBase && myArea != null) myArea.HandlePickUp();
        baseObject.PlaySound();
        player.PickUpItem(baseObject.gameObject);
        baseObject = null;
    }

    private void CheckSwap(){
        GameObject o = player.DropItem("base");
        player.PickUpItem(baseObject.gameObject);
        baseObject = o.GetComponent<BaseObject>();
    }
}

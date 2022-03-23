using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : Draggable
{
    // ==============   variables   ==============
    [SerializeField] private float minPercentOfDist;
    private Collider myCollider;
    private Vector3 initialPos;

    private SharedArea myArea;
    public SharedArea area{set{myArea = value;}}

    bool finalizeOrder;
    bool addingObject;
    Vector3 nextIngredientPosition;

    [SerializeField] private GameObject orderObject;
    public GameObject orderobj {get{return orderObject;}}

    private Player player;
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
        Debug.Log(minDistance);
    }

    private void Start(){
        nextIngredientPosition = new Vector3(this.transform.position.x, this.GetComponent<Renderer>().bounds.max.y, this.transform.position.z-0.01f);
        UpdatePlane();
    }

    private void UpdatePlane(){
        Vector3 center = myCollider.bounds.center;
        //get the vector sides
        Vector3 side1 = transform.right + center;
        Vector3 side2 = transform.up + center;
        
        //get the perpendicular vector
        Vector3 perp = Vector3.Cross(side1, side2);
        
        //normalize perp vector
        Vector3 norm = perp.normalized;
        this.myPlane = new Plane(norm, center);

        //debugging
        // Debug.DrawLine(transform.right + center, center, Color.red, 100f);
        // Debug.DrawLine(transform.up + center, center, Color.green, 100f);
        //Debug.DrawLine(norm + center, center, Color.blue, 100f);
    }

    private void OnMouseExit(){
        if (player.handFree) return;
    }

    private void OnMouseDown(){
        if (!player.handFree){
            nextIngredientPosition = player.AddToCurrentOrder(nextIngredientPosition, this.transform.eulerAngles, this.transform);
            addingObject = true;
        } 
        else{
            //pick up base
            if (finalizeOrder) 
                initialPos = base.GetProjectionOnPlane();
            else
                HandlePickUp(this.gameObject);
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
        orderObject.transform.position = this.transform.position;
        orderObject.SetActive(true);

        foreach(Ingredient o in player.order){
            o.HandleAddToOrder();
            o.transform.SetParent(orderObject.transform);
            o.transform.position = orderObject.transform.position;
        }
        HandlePickUp(orderObject);
    }

    private void HandlePickUp(GameObject o){
        if (orderObject == null && myArea != null) myArea.HandlePickUp();
        player.PickUpItem(o);
    }
}

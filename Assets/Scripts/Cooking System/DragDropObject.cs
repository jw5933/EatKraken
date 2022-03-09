using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropObject : MonoBehaviour
{
    // ==============   variables   ==============
    enum Type { //the types of drag + drop
        Base,
        Serve,
        Consume,
        Trash
    };
    [SerializeField] private Type myType;
    [SerializeField] bool finalizeOrder;
    Vector3 nextIngredientPosition;

    private Player player;
    private CustomerManager cm;
    private HealthManager hm;

    private SharedArea myArea;
    public SharedArea area{set{myArea = value;}}

    // ==============   methods   ==============
    private void Start(){
        player = FindObjectOfType<Player>();
        cm = FindObjectOfType<CustomerManager>();
        hm = FindObjectOfType<HealthManager>();
        if (myType == Type.Base) nextIngredientPosition = new Vector3(0, this.GetComponent<Renderer>().bounds.max.y, this.transform.position.z-0.01f);
    }

    public void OnMouseDown(){
        Debug.Log(this.name);
        switch (myType){
            case Type.Base:
                if (!player.handFree) nextIngredientPosition = player.AddToCurrentOrder(nextIngredientPosition, this.transform.eulerAngles, this.transform);
                else{
                    //pick up base
                    if (myArea != null) myArea.HandlePickUp();
                    HandlePickUp();
                }
            break;

            case Type.Serve:
                if (player.holdingBase && player.order.Count > 0){
                    //Debug.Log("serving");
                    List<string> order = new List<string>();
                    foreach(Ingredient i in player.order){
                        order.Add(i.name);
                    }
                    cm.ServeCustomer(order);
                    player.ClearOrder(); 
                }
            break;

            case Type.Consume:
                if (player.holdingBase && player.order.Count > 0){
                    hm.CheckConsume(player.order);
                    player.ClearOrder();
                }
            break;

            case Type.Trash:
                if (!player.handFree){
                    GameObject o = player.DropItem("ingredient");
                    if (o==null) return;
                    Destroy(o);
                }
            break;
        }
    }

    private void HandlePickUp(){
        if (!finalizeOrder){
            player.PickUpItem(this.gameObject);
            return;
        }
    }
}

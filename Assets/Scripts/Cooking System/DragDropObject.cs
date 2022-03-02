using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropObject : MonoBehaviour
{
    // ==============   variables   ==============
    enum Type { //the types of drag + drop
        Base,
        Serve,
        Consume
    };
    [SerializeField] private Type myType;

    private Player player;
    private CustomerManager cm;
    private HealthManager hm;

    private SharedArea myArea;
    public SharedArea area{set{myArea = value;}}

    // ==============   methods   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        cm = FindObjectOfType<CustomerManager>();
        hm = FindObjectOfType<HealthManager>();
    }

    public void OnMouseDown(){
        Debug.Log(this.name);
        switch (myType){
            case Type.Base:
                if (!player.handFree) player.AddToCurrentOrder();
                else{
                    //pick up base
                    if (myArea != null) myArea.HandlePickUp();
                    player.PickUpItem(this.gameObject);
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
        }
    }
}

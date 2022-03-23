using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    // ==============   variables   ==============
    enum Type { //the types of drag + drop
        Serve,
        Consume,
        Trash
    };
    [SerializeField] private Type myType;

    private Player player;
    private CustomerManager cm;
    private HealthManager hm;
    private BaseObject baseObj;

    private SharedArea myArea;
    public SharedArea area{set{myArea = value;}}

    // ==============   methods   ==============
    private void Start(){
        player = FindObjectOfType<Player>();
        cm = FindObjectOfType<CustomerManager>();
        hm = FindObjectOfType<HealthManager>();
        baseObj = FindObjectOfType<BaseObject>();
    }

    public void OnMouseDown(){
        //Debug.Log(this.name);
        switch (myType){
            case Type.Serve:
                if (player.holdingBase && player.order.Count > 0){
                    //Debug.Log("serving");
                    cm.ServeCustomer(player.order);

                    if (baseObj.orderobj !=null){
                        player.DropItem("any");
                        baseObj.orderobj.SetActive(false);
                    }
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
}

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

    public void OnMouseUp(){
        List<Ingredient> orderCopy = new List<Ingredient>(player.order);
        //Debug.Log(this.name);
        switch (myType){
            case Type.Serve:
                if (player.holdingBase && player.order.Count > 0){
                    //Debug.Log("serving");
                    
                    if (cm.ServeCustomer(orderCopy)){
                        if (baseObj.orderobj !=null){
                            player.DropItem("any");
                            baseObj.ResetVars();
                        }
                        player.ClearOrder(); 
                    }
                }
            break;

            case Type.Consume:
                if (player.holdingIngredient){
                    Ingredient o = player.DropItem("ingredient").GetComponent<Ingredient>();
                    orderCopy.Clear();
                    orderCopy.Add(o);
                    
                    hm.CheckConsume(orderCopy);
                    Destroy(o.gameObject);
                }
                else if (player.holdingBase && player.order.Count > 0){
                    hm.CheckConsume(orderCopy);
                    if (baseObj.orderobj !=null){
                        player.DropItem("any");
                        baseObj.ResetVars();
                    }
                    player.ClearOrder();
                }
            break;

            case Type.Trash:
                if (player.holdingIngredient){
                    GameObject o = player.DropItem("ingredient");
                    if (o==null) return;
                    Destroy(o);
                }
            break;
        }
    }
}

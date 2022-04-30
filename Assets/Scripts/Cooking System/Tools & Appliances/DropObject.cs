using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private BaseHolder baseHolder;

    private SharedArea myArea;
    public SharedArea area{set{myArea = value;}}
    private Material material;

    // ==============   methods   ==============
    private void Start(){
        player = FindObjectOfType<Player>();
        cm = FindObjectOfType<CustomerManager>();
        hm = FindObjectOfType<HealthManager>();
        baseHolder = FindObjectOfType<BaseHolder>();
        material = GetComponent<Image>().material;
    }

    public void OnMouseUp(){
        List<Ingredient> order = new List<Ingredient>();
        if (player.holdingBase){
            order = new List<Ingredient>(player.baseObject.order);
        }
        //Debug.Log(this.name);
        switch (myType){
            case Type.Serve:
                if (player.holdingBase && player.baseObject.order.Count > 0){
                    //Debug.Log("serving");
                    
                    if (cm.ServeCustomer(order)){
                        Destroy(player.DropItem("base"));
                    }
                }
            break;

            case Type.Consume:
                if (player.holdingIngredient){
                    Ingredient o = player.DropItem("ingredient").GetComponent<Ingredient>();
                    hm.CheckConsume(new List<Ingredient>{o});
                    Destroy(o.gameObject);
                }
                else if (player.holdingBase && player.baseObject.order.Count > 0){
                    hm.CheckConsume(order);
                    Destroy(player.DropItem("base"));
                }
            break;

            case Type.Trash:
                if (player.holdingIngredient){
                    Destroy(player.DropItem("ingredient"));
                }
            break;
        }
    }

    private void OnMouseEnter(){
        material.SetFloat("_Outline", 1);
    }

    private void OnMouseExit(){
        material.SetFloat("_Outline", 0);
    }
}

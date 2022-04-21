using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    // ==============   variables   ==============
    private List <Ingredient> myOrder = new List <Ingredient>();
    public List<Ingredient> order {get {return myOrder;}}
    Ingredient currIngredient;

    Player player;
    private SharedArea myArea;
    public SharedArea area{set{myArea = value;}}
    private Collider myCollider;

    [SerializeField] AudioClip sound;
    AudioManager am;

    // ==============   methods   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        am = FindObjectOfType<AudioManager>();
        myCollider = GetComponent<Collider>();
    }

    //add held ingredient to the order
    public bool AddToOrder(Vector3 angle){
        if (player.holdingIngredient && player.ingredient.AtEndState()){
            //check if the type is accepted, if it is then add the ingredient
            if (CheckCanAddIngredient(player.ingredient.type, myOrder.Count)){
                currIngredient = player.DropItem("ingredient").GetComponent<Ingredient>();
                myOrder.Add(currIngredient);
                currIngredient.GetComponent<Collider>().enabled = false;
                //Update the visuals to reflect addition of ingredient
                UpdateOrderVisual(this.transform.position, angle);
                return true;
            }
        }
        return false;
    }

    public void PlaySound(){
        if (am != null) am.PlaySFX(sound);
    }
    
    private void OnMouseUp(){ //if the player isnt holding anything, pick up this ingredient
        /* if (player.holdingBase){
            if (myArea != null && myArea.CheckSwapIngredient())
                myArea = null;
        } */
        if (!player.handFree){ //not used with baseholder, just shared areas (player cannot create order on these areas)
            if (player.holdingBase && myArea != null) myArea.CheckSwapBaseObject();
            else return;
        }
        else{
            player.PickUpItem(this.gameObject);
            if (myArea != null){
                myArea.HandlePickUp();
                myArea = null;
            }
        }
    }

    private void UpdateOrderVisual(Vector3 pos, Vector3 angle){
        currIngredient.HandleAddToOrder(); //tell ingredient to transform its sprites
        currIngredient.transform.Rotate(angle - currIngredient.transform.eulerAngles, Space.World);
        currIngredient.transform.parent = this.transform;
        currIngredient.transform.localPosition = Vector3.zero;
        currIngredient.transform.localScale = Vector3.one;
        currIngredient = null;
    }

    public bool CheckCanAddIngredient(Ingredient.Type t, int ingredientsAdded){
        if (!player.hasBaseIngredient) ingredientsAdded ++;
        if((ingredientsAdded == 0 && t == Ingredient.Type.Base) 
        || (ingredientsAdded == 1 && t == Ingredient.Type.Carb) 
        || (ingredientsAdded >=2 && (t != Ingredient.Type.Base && t != Ingredient.Type.Carb)))
            return true;
        return false;
    }

    public void ClearOrder(){
        // Debug.Log(myOrder.Count);
        foreach (Ingredient i in myOrder){
            Destroy(i.gameObject);
        }

        //FIX: reset vars -> destroy object
        myOrder.Clear();
    }

    public void UnsetCollider(){
        myCollider.enabled = false;
    }

    public void SetCollider(){
        myCollider.enabled = true;
    }
}

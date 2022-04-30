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
    private Material material;

    [SerializeField] AudioClip sound;
    AudioManager am;

    // ==============   methods   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        am = FindObjectOfType<AudioManager>();
        myCollider = GetComponent<Collider>();
        material = GetComponent<SpriteRenderer>().material;
    }

    //add held ingredient to the order
    public bool AddToOrder(Ingredient i = null){
        if ( i == null && player.holdingIngredient)
            i = player.ingredient;
        if (i != null && i.AtEndState()){
            //check if the type is accepted, if it is then add the ingredient
            if (CheckCanAddIngredient(i.type, myOrder.Count)){
                currIngredient = i;
                player.DropItem("ingredient");
                myOrder.Add(currIngredient);
                currIngredient.GetComponent<Collider>().enabled = false;
                //Update the visuals to reflect addition of ingredient
                UpdateOrderVisual(this.transform.position);
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

    private void OnMouseEnter(){
        material.SetFloat("_Outline", 1);
    }

    private void OnMouseExit(){
        material.SetFloat("_Outline", 0);
    }

    private void UpdateOrderVisual(Vector3 pos){
        currIngredient.HandleAddToOrder(); //tell ingredient to transform its sprites
        currIngredient.transform.Rotate(this.transform.eulerAngles - currIngredient.transform.eulerAngles, Space.World);
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

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

    // ==============   methods   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
    }

    //add held ingredient to the order
    public bool AddToCurrentOrder(Vector3 pos, Vector3 angle, Transform t){
        if (player.holdingIngredient && player.ingredient.AtEndState()){
            //check if the type is accepted, if it is then add the ingredient
            if (CheckCanAddIngredient(player.ingredient.type, myOrder.Count)){
                currIngredient = player.DropItem("ingredient").GetComponent<Ingredient>();
                myOrder.Add(currIngredient);
                currIngredient.GetComponent<Collider>().enabled = false;
                currIngredient.transform.SetParent(t, true);
                currIngredient.transform.localScale = Vector3.one;
                //Update the visuals to reflect addition of ingredient
                UpdateOrderVisual(pos, angle);
                return true;
            }
        }
        return false;
    }

    private void UpdateOrderVisual(Vector3 pos, Vector3 angle){
        currIngredient.HandleAddToOrder(); //tell ingredient to transform its sprites
        currIngredient.transform.position = pos;
        currIngredient.transform.Rotate(angle - currIngredient.transform.eulerAngles, Space.World);
        currIngredient = null;
    }

    private bool CheckCanAddIngredient(Ingredient.Type t, int ingredientsAdded){
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
}

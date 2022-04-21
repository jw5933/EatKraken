using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] private GameObject myIngredientPrefab;
    string myIngredientName;

    private Player player;
    private GameObject newIngredient;

    private void Awake(){
        //this.transform.SetParent(FindObjectOfType<GameManager>().ingredientParent);
        player = FindObjectOfType<Player>();
        myIngredientName = myIngredientPrefab.name;
    }

    private void OnMouseDown(){
        if (!player.handFree){
            CheckDespawn();
            return;
        }
        SpawnIngredient();
        player.PickUpItem(newIngredient);
    }

    private void CheckDespawn(){
        if (player.holdingIngredient && player.ingredient.name == myIngredientName){
            Destroy(player.DropItem("ingredient"));
        }
    }

    private void SpawnIngredient(){
        newIngredient = Instantiate(myIngredientPrefab, this.transform);
        newIngredient.transform.position = transform.position;
        newIngredient.GetComponent<Ingredient>().parent = this.transform;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] private GameObject myIngredientPrefab;
    string myIngredientName;

    private Player player;
    private GameObject newIngredient;
    private Material material;

    private void Awake(){
        //this.transform.SetParent(FindObjectOfType<GameManager>().ingredientParent);
        player = FindObjectOfType<Player>();
        myIngredientName = myIngredientPrefab.name;
        material = GetComponent<SpriteRenderer>().material;
    }

    private void OnMouseDown(){
        if (!player.handFree){
            CheckDespawn();
            return;
        }
        SpawnIngredient();
        player.PickUpItem(newIngredient);
    }

    private void OnMouseEnter(){
        material.SetFloat("_Outline", 1);
    }

    private void OnMouseExit(){
        material.SetFloat("_Outline", 0);
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

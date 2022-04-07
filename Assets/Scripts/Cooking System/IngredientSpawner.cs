using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] private GameObject myIngredientPrefab;

    private Player player;
    private GameObject newIngredient;

    private void Awake(){
        this.transform.SetParent(FindObjectOfType<GameManager>().ingredientParent);
        player = FindObjectOfType<Player>();
    }

    private void OnMouseDown(){
        if (!player.handFree) return;
        SpawnIngredient();
        player.PickUpItem(newIngredient);
    }

    public void SpawnIngredient(){
        newIngredient = Instantiate(myIngredientPrefab, this.transform);
        newIngredient.transform.position = transform.position;
        newIngredient.GetComponent<Ingredient>().parent = this.transform;
    }
}

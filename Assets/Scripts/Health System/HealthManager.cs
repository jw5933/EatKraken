using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
health manager needs a horizontal layout group
*/
public class HealthManager : MonoBehaviour
{
    // ==============   variables   ==============
    [SerializeField] private float playerHearts = 5;
    private float maxHearts;
    public float playerHealth{get{return playerHearts;}}

    private Image[] myHearts;
    private int prevHeart;
    private int currentHeart;

    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Sprite nullHeart;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;

    GameManager gm;
    
    // ==============   methods   ==============
    private void Awake(){
        gm = FindObjectOfType<GameManager>();
    }

    private void Start(){
        maxHearts = playerHearts;
        currentHeart = Mathf.CeilToInt(playerHearts - 1);
        myHearts = new Image[currentHeart+1];

        for (int i = 0; i < myHearts.Length; i++){
            myHearts[i] = Instantiate(heartPrefab, this.transform).GetComponent<Image>();
        }
    }

    public void MinusPlayerHearts(float hearts){//adjust player health
        if (playerHearts >= maxHearts && hearts < 0) return;
        playerHearts = Mathf.Min(playerHearts-hearts, maxHearts);
        Debug.Log("player health: " + playerHearts);
        
        if (playerHearts >= 0) 
            UpdateHealthUI();
        else 
            StartCoroutine(gm.HandleEndGame("You've overworked yourself and had to call in sick. Your boss didn't like that much, and fired you! Try again... \n Press <R> to retry.")); //check if the player has died
    }

    private void UpdateHealthUI(){
        int pIndex = prevHeart;
        prevHeart = currentHeart;
        currentHeart = Mathf.CeilToInt(playerHearts - 1);

        if (playerHearts%(currentHeart+1) > 0){ //half health
            if (currentHeart-1 >= 0) myHearts[currentHeart-1].sprite = fullHeart;
            if (currentHeart+1 < myHearts.Length) myHearts[currentHeart+1].sprite = nullHeart;
            myHearts[currentHeart].sprite = halfHeart;
        }
        else if (prevHeart > currentHeart){ //heart has decremented
            for (int h = pIndex; h > currentHeart; h--){
                myHearts[h].sprite = fullHeart;
            }
            //myHearts[currentHeart+1].sprite = nullHeart;
        }
        else { //heart has incremented
            for (int h = pIndex; h <= currentHeart; h++){
                myHearts[h].sprite = fullHeart;
            }
            //myHearts[currentHeart].sprite = fullHeart;
        }
    }

    public void CheckConsume(List<Ingredient> order){
        int i = 0;
        bool hasProtein = false;
        while (order.Count > 0){
            if (order[0].type != Ingredient.Type.Base) i++;
            if (order[0].type == Ingredient.Type.Protein) hasProtein = true;
            order.RemoveAt(0);
        }

        if (i >= gm.maxIngredients){// 1 heart
            MinusPlayerHearts(-2);
        }
        else if (i >= gm.maxIngredients -1){
            MinusPlayerHearts(-1);
        }
        if (hasProtein){
            MinusPlayerHearts(-0.5f);
        }
    }
}

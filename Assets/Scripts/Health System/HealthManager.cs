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

    bool decrement;

    GameManager gm;
    
    // ==============   methods   ==============
    private void Awake(){
        gm = FindObjectOfType<GameManager>();
    }

    private void Start(){
        maxHearts = playerHearts;
        //currentHeart = Mathf.FloorToInt(playerHearts);
        currentHeart = Mathf.CeilToInt(playerHearts - 1);
        myHearts = new Image[Mathf.FloorToInt(playerHearts)];

        for (int i = 0; i < myHearts.Length; i++){
            myHearts[i] = Instantiate(heartPrefab, this.transform).GetComponent<Image>();
        }
    }

    public void MinusPlayerHearts(float hearts){//adjust player health
        if (playerHearts >= maxHearts && hearts < 0) return;
        if (hearts > 0) decrement = true;
        else decrement = false;
        playerHearts = Mathf.Min(playerHearts-hearts, maxHearts);
        //Debug.Log("player health: " + playerHearts);
        
        if (playerHearts >= 0) 
            UpdateHealthUI();
        else 
            StartCoroutine(gm.HandleEndGame("You've overworked yourself and had to call in sick. Your boss didn't like that much, and fired you! Try again... \n Press <R> to retry.")); //check if the player has died
    }

    private void UpdateHealthUI(){
        prevHeart = currentHeart;
        int pIndex = prevHeart;
        Debug.Log("previous heart " + pIndex);
        currentHeart = Mathf.CeilToInt(playerHearts - 1);

        if (decrement){ //heart has decremented
            nullHearts(pIndex, currentHeart);
        }
        else{ //heart has incremented
            fullHearts(pIndex, currentHeart);
        }

        if (playerHearts%(currentHeart+1) > 0){ //half health
            myHearts[currentHeart].sprite = halfHeart;
        }
    }

    private void nullHearts(int s, int e){
        for (int h = s; h > e; h--){
                myHearts[h].sprite = nullHeart;
            }
    }
    private void fullHearts(int s, int e){
        for (int h = s; h <= e; h++){
                myHearts[h].sprite = fullHeart;
            }
    }

    public void CheckConsume(List<Ingredient> order){
        //cut or cooked .5
        int i = 0;
        int p = 0;
        while (order.Count > 0){
            Ingredient o = order[0];
            if ((o.type == Ingredient.Type.Vegetable || o.type == Ingredient.Type.Carb) && ((o.hasCutStage && o.finishedCutStage) || (o.hasCookStage && o.finishedCookedStage))) i++;
            if (o.type == Ingredient.Type.Protein) p++;
            order.RemoveAt(0);
        }
        MinusPlayerHearts(-(i*0.5f));
        /* if (i >= gm.maxIngredients){// 1 heart
            MinusPlayerHearts(-2);
        }
        else if (i >= gm.maxIngredients -1){
            MinusPlayerHearts(-1);
        } */
        MinusPlayerHearts(-(p*1));
    }
}

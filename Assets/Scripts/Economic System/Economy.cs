using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Economy : MonoBehaviour
{
    [SerializeField] private Text coinsText;
    private float playerCoins;
    public float playerAccount{get{return playerCoins;}} //for map
    private float tips;
    private float orderCoins;

    private void Awake(){
        EventManager em = FindObjectOfType<EventManager>();
        em.OnCoinChange += AddPlayerCoins;
    }
    public void AddPlayerCoins(Customer c, float coins, float x, int xx){//adjust player's coins
        //Debug.Log("adding coins to player account: " + coins);
        tips += (coins - c.orderPrice);
        orderCoins += c.orderPrice;
        playerCoins += coins;
        coinsText.text = "BANK\n" + playerCoins.ToString("c2");
    }

}

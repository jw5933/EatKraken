using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Economy : MonoBehaviour
{
    [SerializeField] private Text tipsText;
    [SerializeField] private Text bankText;
    [SerializeField] private Text lastOrderText;
    private float playerCoins;
    public float playerAccount{get{return playerCoins;}} //for map
    private float tips;

    private void Awake(){
        EventManager em = FindObjectOfType<EventManager>();
        em.OnCoinChange += AddPlayerCoins;
    }
    public void AddPlayerCoins(Customer c, float coins, float x, int xx){//adjust player's coins
        //Debug.Log("adding coins to player account: " + coins);
        float tip = coins - c.orderPrice;
        tips += (tip > 0 ? tip : 0);
        playerCoins += coins;
        tipsText.text = tips.ToString("0.00");
        bankText.text = playerCoins.ToString("0.00");
        lastOrderText.text = coins.ToString("0.00");
    }

}

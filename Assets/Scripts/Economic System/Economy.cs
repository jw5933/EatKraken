using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Economy : MonoBehaviour
{
    [SerializeField] private Image tipsImage;
    [SerializeField] private Sprite[] tipsImgs;
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
        if (coins == 0) return;
        float tip = coins - c.orderPrice;
        tips += (tip > 0 ? tip : 0);
        
        if (tips < 15){
            tipsImage.sprite = tipsImgs[0];
        }
        else if (tips < 30){
            tipsImage.sprite = tipsImgs[1];
        }
        else if (tips < 45){
            tipsImage.sprite = tipsImgs[2];
        }
        else {
            tipsImage.sprite = tipsImgs[3];
        }

        playerCoins += (coins-tip > 0? coins - tip: 0);
        tipsText.text = tips.ToString("0.00");
        if (bankText != null) bankText.text = playerCoins.ToString("0.00");
        if (lastOrderText != null) lastOrderText.text = coins.ToString("0.00");
    }

}

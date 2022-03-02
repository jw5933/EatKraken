using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Economy : MonoBehaviour
{
    [SerializeField] private Text coinsText;
    private float playerCoins;
    public float playerAccount{get{return playerCoins;}}

    public void AddPlayerCoins(float coins){//adjust player's coins
        playerCoins += coins;
        coinsText.text = "Balance: $" + playerCoins;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class EventManager : MonoBehaviour
{
    // ==============   variables   ==============
    //location change
    public Action<Location> OnLocationChange;
    //day time change
    public Action<float, int> OnTimeChange;
    //player coins change
    public Action<Customer, float, float, int> OnCoinChange;

    // ==============   methods   ==============
    public void ChangeLocation(Location next){
        //Debug.Log("called location change in Event Manager");
        if (OnLocationChange != null){
            //Debug.Log("On location change has subs");
            OnLocationChange(next); //if there is a subscriber
        }
    }

    public void ChangeTime(float nextTime, int phase){
        if (OnTimeChange != null) OnTimeChange(nextTime, phase);
    }

    public void ChangeCoins(Customer customer, float coinMade, float coinMax, int timePhase){
        if (OnCoinChange != null) OnCoinChange(customer, coinMade, coinMax, timePhase);
    }
}

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
    //customer leaves (what position is it in?)
    public Action<int, Customer.Mood> OnCustomerLeave;
    //action to tell generator to spawn a new custoemr (if there are any left soon)
    public Action OnCustomerNeutral;
    //dialogue adjustments
    public Action<int> OnDialogueChange;

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

    public void FreeCustomer(int position, Customer.Mood mood){
        if (OnCustomerLeave != null) OnCustomerLeave(position, mood);
    }

    public void ChangeCustomerMood(){
        if (OnCustomerNeutral != null) OnCustomerNeutral();
    }

    public void ChangeDialogue(int index){
        if (OnDialogueChange != null) OnDialogueChange(index);
    }
}

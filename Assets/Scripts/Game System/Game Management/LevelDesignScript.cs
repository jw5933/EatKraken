using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Phase {
    public int phaseNum;
    public float moneyEarned;
    public float moneyLost;
    public float moneyMax;

    public int customersServed;
    public int customersLeft;
    public int customerMax;
}


public class LevelDesignScript : MonoBehaviour
{
    DayManager dm;
    EventManager em;
    Generator g;
    Map map;
    public Phase[] phases;

    public void Awake(){
        dm = FindObjectOfType<DayManager>();
        g = FindObjectOfType<Generator>();
        map = FindObjectOfType<Map>();
        em = FindObjectOfType<EventManager>();
        em.OnCoinChange += UpdateOnCoinChange;
        //em.OnLocationChange += UpdateOnLocationChange;
        WakeUpManagers();
    }
    //economy
    //update the amount of money earned and customers served in current phase
    private void UpdateOnCoinChange(float made, float max){
        if (made <= 0) phases[dm.phase].customersLeft += 1;
        else phases[dm.phase].customersServed += 1;

        float lost = max - made;
        phases[dm.phase].moneyEarned += made;
        phases[dm.phase].moneyLost += lost;
    }

    //update phase info
    private void UpdateInfo(){
        for (int i = 0; i < phases.Length; i++){
            List<Customer> cs = g.GetCustomerListForPhase(i);

            foreach(Customer c in cs){
                phases[i].moneyMax += c.maxCoins;
                Debug.Log(c.gameObject.name + " " + c.maxCoins);
                phases[i].customerMax += 1;
            }
            Debug.Log(phases[i].moneyMax);
        }
    }

    //called by generator after it makes its customer by phase lists
    public void CreateNewArrays(int n){
        phases = new Phase[n];
        for(int i = 0; i < phases.Length; i++){
            phases[i] = new Phase();
            phases[i].phaseNum = i;
        }
        UpdateInfo();
    }

    //location
    public string GetLocationName(){
        if (map.location !=null) return map.location.gameObject.name;
        return "";
    }

    public void GoNextLocation(Location l){
        Debug.Log(l.gameObject.name);
        map.selectedLocation = l;
        map.goNextLocation();
    }

    public void WakeUpManagers(){
        dm.Awake();
    }

}

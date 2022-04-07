using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Phase {
    public int phaseNum;
    public float time;

    public float moneyEarned;
    public float moneyLost;
    public float moneyMax;

    public int customersServed;
    public int customersLeft;
    public int customerMax;
}


public class LevelDesignScript : MonoBehaviour
{
    //DayManager dm;
    EventManager em;
    GameManager gm;
    Generator g;
    Map map;
    public Phase[] phases;

    float[] timePerStage;
    float time;
    Timer timer;
    float timeScale;

    public void Awake(){
        //dm = FindObjectOfType<DayManager>();
        g = FindObjectOfType<Generator>();
        map = FindObjectOfType<Map>();
        gm = FindObjectOfType<GameManager>();
        em = FindObjectOfType<EventManager>();
        em.OnCoinChange += UpdateOnCoinChange;
        em.OnLocationChange += UpdateOnLocationChange;
        WakeUpManagers();
        timer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
    }
    //economy
    //update the amount of money earned and customers served in current phase
    private void UpdateOnCoinChange(Customer c, float made, float max, int phase){
        //Debug.Log(phases.Length);
        if (made <= 0) phases[phase].customersLeft += 1;
        else phases[phase].customersServed += 1;

        float lost = max - made;
        phases[phase].moneyEarned += made;
        phases[phase].moneyLost += lost;
    }

    private void UpdateOnLocationChange(Location next){
        timePerStage = next.timeStages;
        time = Time.time;
        if (phases.Length == 0) CreateNewArrays(timePerStage.Length);
        for(int i = 0; i < phases.Length; i++){
            phases[i].time = time;
            time += timePerStage[i];
        }
    }

    //update phase info
    public void UpdateInfo(int n){
        if (phases.Length == 0) CreateNewArrays(n);
        for (int i = 0; i < phases.Length; i++){
            List<Customer> cs = g.GetCustomerListForPhase(i);

            foreach(Customer c in cs){
                phases[i].moneyMax += c.maxCoins;
                //Debug.Log(c.gameObject.name + " " + c.maxCoins);
                phases[i].customerMax += 1;
            }
            //Debug.Log(phases[i].moneyMax);
        }
    }

    //called by generator after it makes its customer by phase lists
    public void CreateNewArrays(int n){
        phases = new Phase[n];
        for(int i = 0; i < n; i++){
            phases[i] = new Phase();
            phases[i].phaseNum = i;
        }
    }

    public void GoToPhase(int n){
        if (n >= phases.Length) return;
        float t = phases[n].time - Time.time;
        if (t<=0) return;
        timer.Init(t, HandleTimeSkip);
        timer.StartTimer();
        timeScale = Time.timeScale;
        Time.timeScale = 5;
    }
    private void HandleTimeSkip(){
        Time.timeScale = timeScale;
        //Debug.Log("time scale reset");
    }

    //location
    public string GetLocationName(){
        if (map.location !=null) return map.location.gameObject.name;
        return "";
    }

    public void GoNextLocation(Location l){
        //Debug.Log(l.gameObject.name);
        map.selectedLocation = l;
        map.goNextLocation();
    }

    public void WakeUpManagers(){
        //dm.Awake();
    }

}

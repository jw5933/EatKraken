using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesignScript : MonoBehaviour
{
    DayManager dm;
    EventManager em;
    Map map;

    public void Awake(){
        dm = FindObjectOfType<DayManager>();
        em = FindObjectOfType<EventManager>();
        map = FindObjectOfType<Map>();
        em.OnLocationChange += UpdateOnLocationChange;
        WakeUpManagers();
    }

    private void UpdateOnLocationChange(Location next){
        
    }

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

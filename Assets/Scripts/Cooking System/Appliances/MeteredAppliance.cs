using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteredAppliance : Appliance
{
    private Meter meter;
    SharedArea myArea;

    protected override void Awake(){
        base.Awake();
        base.myType = Appliance.Type.Metered;
        myArea = transform.GetChild(0).GetComponent<SharedArea>();
        if (myArea !=null){
            myArea.FreedAreaEvent.AddListener(HandlePickUp);
            myArea.appliance = this;
        }
    }

    protected override void StartMeter(){
        if (myArea==null || myArea.free) return;
        Debug.Log("starting meter");
        if (meter == null){
            meter = Instantiate(gm.stationaryMeterPrefab, gm.stationaryCanvas.transform).GetComponent<Meter>();
            meter.gameObject.transform.position = this.transform.position - new Vector3(0, 0, GetComponent<Collider>().bounds.min.z - 1);
        }

        if (myArea.free || myIngredient.type != myIngredientType)
            return;

        timer = meter.Init(myIngredient.raw, myIngredient.cooked, myIngredient.burnt, myIngredient.cookedTime, HandleEndTimer);
        meter.gameObject.SetActive(true);
        meter.StartMeter();
    }

    protected override void HandleEndTimer(){
        myIngredient.ChangeCookedState();
    }

    protected override void HandlePickUp(){
        meter.StopMeter();
        myIngredient.cookedTime = meter.callerTime;
    }
}

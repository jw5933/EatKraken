using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteredAppliance : Appliance
{
    private Meter meter;

    [SerializeField] float rawTime;
    [SerializeField] float cookedTime;
    [SerializeField] float burntTime;

    [SerializeField] SharedArea myArea;
    [SerializeField] Ingredient.Type myIngredientType;

    protected override void Awake(){
        base.Awake();
        if (myArea !=null) myArea.FreedAreaEvent.AddListener(HandlePickUp);
    }

    protected override void StartMeter(){
        if (meter == null) meter = Instantiate(gm.meterPrefab, this.transform).GetComponent<Meter>();

        if (!DropIngredient() || myIngredient.type != myIngredientType || myIngredient.imgState >= maxState)
            return;

        timer = meter.Init(rawTime, cookedTime, burntTime, myIngredient.cookedTime, HandleEndTimer);
        meter.gameObject.SetActive(true);
    }

    protected override void HandleEndTimer(){
        myIngredient.ChangeCookedState();
    }

    protected override void HandlePickUp(){
        meter.StopMeter();
        myIngredient.cookedTime = meter.callerTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteredAppliance : Appliance
{
    private Meter meter;

    [SerializeField] float rawTime;
    [SerializeField] float cookedTime;
    [SerializeField] float burntTime;

    SharedArea myArea;

    protected override void Awake(){
        base.Awake();
        base.myType = Appliance.Type.Metered;
        myArea = GetComponent<SharedArea>();
        if (myArea !=null) myArea.FreedAreaEvent.AddListener(HandlePickUp);
    }

    protected override void StartMeter(){
        if (meter == null) meter = Instantiate(gm.meterPrefab, this.transform).GetComponent<Meter>();

        if (!DropIngredient() || myIngredient.type != myIngredientType || myIngredient.cookedState >= Ingredient.CookedState.Burnt)
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

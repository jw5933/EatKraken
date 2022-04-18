using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpawner : MonoBehaviour
{
    [SerializeField] private GameObject basePrefab;
    private Player player;
    private GameObject newBase;

    public void Awake(){
        player = FindObjectOfType<Player>();
    }

    private void OnMouseUp(){
        if (!player.handFree){
            //Debug.Log("clicked");
            CheckDespawn();
            CheckAutoAdd();
            return;
        }
        SpawnBase();
        player.PickUpItem(newBase);
        newBase = null;
    }

    private void CheckDespawn(){
        if (player.holdingBase && player.baseObject.order.Count <= 0){
            Destroy(player.DropItem("base"));
        }
    }

    private void CheckAutoAdd(){
        if (!player.holdingIngredient) return;
        SpawnBase();
        BaseObject baseObj = newBase.GetComponent<BaseObject>();
        if (!baseObj.CheckCanAddIngredient(player.ingredient.type, 0)){
            Destroy(newBase);
            newBase = null;
            return;
        }
        baseObj.AddToOrder(newBase.transform.eulerAngles);
        player.PickUpItem(newBase);
    }

    private void SpawnBase(){
        newBase = Instantiate(basePrefab, this.transform);
        newBase.transform.position = transform.position;
    }
}

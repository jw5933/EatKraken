using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpawner : MonoBehaviour
{
    [SerializeField] private GameObject basePrefab;
    private Player player;
    private GameObject newBase;
    private Material material;

    public void Awake(){
        player = FindObjectOfType<Player>();
        material = GetComponent<SpriteRenderer>().material;
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

    private void OnMouseEnter(){
        material.SetFloat("_Outline", 1);
    }

    private void OnMouseExit(){
        material.SetFloat("_Outline", 0);
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
        if (!baseObj.AddToOrder()){
            Destroy(newBase);
            newBase = null;
            return;
        }
        baseObj.AddToOrder();
        player.PickUpItem(newBase);
    }

    private void SpawnBase(){
        newBase = Instantiate(basePrefab, this.transform);
        newBase.transform.position = transform.position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProteinManager : MonoBehaviour
{
    // ==============   variables   ==============
    [SerializeField] GameObject[] proteinPrefabs = new GameObject[9]; //max 9 types of protein

    [SerializeField] Animator anim;
    [SerializeField] AnimationClip animClip;

    [SerializeField] private float proteinCountdown = 2;
    public float countdown {get{return proteinCountdown;}}

    GameManager gm;
    Player player;
    HealthManager hm;

    // ==============   methods   ==============
    public void Start(){
        gm = FindObjectOfType<GameManager>();
        hm = FindObjectOfType<HealthManager>(true);
        player = GetComponent<Player>();
    }

    public void CreateProtein(int n){
        if (proteinPrefabs[n] != null && (player.handFree)){
            GameObject o = Instantiate(proteinPrefabs[n], gm.ingredientParent);
            hm.MinusPlayerHearts(1);
            player.PickUpItem(o);
        }
    }

    public void AnimateCreateProtein(){
        anim.speed = animClip.length/proteinCountdown;
        Debug.Log("speed anim will go at: " + anim.speed);
        anim.SetTrigger("Protein");
    }

    public void StopAnim(){
        anim.Rebind();
        
    }
}

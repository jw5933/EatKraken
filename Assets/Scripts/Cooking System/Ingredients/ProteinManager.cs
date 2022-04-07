using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProteinManager : MonoBehaviour
{
    // ==============   variables   ==============
    [SerializeField] GameObject[] proteinPrefabs = new GameObject[9]; //max 9 types of protein
    [SerializeField] Animator[] anims = new Animator[9];
    [SerializeField] AnimationClip[] animClips = new AnimationClip[9];

    [SerializeField] private float proteinCountdown = 1.5f;
    public float countdown {get{return proteinCountdown;}}

    //vars to create protein
    private float endTime;
    private bool canCreateProtein;
    private int wantedProtein;

    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3
         //FIX: add keycodes as needed
     };

    GameManager gm;
    Player player;
    HealthManager hm;

    // ==============   methods   ==============
    public void Start(){
        gm = FindObjectOfType<GameManager>();
        hm = FindObjectOfType<HealthManager>(true);
        player = GetComponent<Player>();
    }

    public void Update(){
        CheckInput();
    }

    private void CheckInput(){
        ValidateCreateProtein();
    }

    private void ValidateCreateProtein(){
        for(int n = 0; n < keyCodes.Length; n++){
            if (Input.GetKeyDown(keyCodes[n]) && !canCreateProtein) {
                if (!player.handFree) return;
                canCreateProtein = true;
                float startTime = Time.time;
                endTime = startTime + countdown;
                //Debug.Log(string.Format("start time: %d, end time: %d", startTime, endTime));
                wantedProtein = n;
                AnimateCreateProtein();
            }
            if (Input.GetKeyUp(keyCodes[n]) && canCreateProtein) {
                canCreateProtein = false;
                StopAnim();
            }
            if (Time.time >= endTime && canCreateProtein) {
                canCreateProtein = false;
                CreateProtein(wantedProtein);
                StopAnim();
            }
        } 
    }

    public void CreateProtein(int n){
        if (proteinPrefabs[n] != null && (player.handFree)){
            GameObject o = Instantiate(proteinPrefabs[n], gm.ingredientParent);
            hm.MinusPlayerHearts(1);
            player.PickUpItem(o);
        }
    }

    public void AnimateCreateProtein(){
        anims[wantedProtein].speed = animClips[wantedProtein].length/proteinCountdown;
        //Debug.Log("speed anim will go at: " + anim.speed);
        anims[wantedProtein].SetTrigger("Protein");
    }

    public void StopAnim(){
        anims[wantedProtein].Rebind();
        
    }
}

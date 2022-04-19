using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProteinSelector : MonoBehaviour
{
    private Player player;
    GameObject tentacle;
    private Animator tentacleAnim; //anims: ShowProtein - brings the tentacle up -> should be first child under this object
    [SerializeField] private GameObject proteinKnife; //will be a tool type
    private Animator knifeAnim; ///anims: CutProtein - show protein being cut
    private GameObject newProtein;

    private GameManager gameManager;
    public GameManager gm {get{return gameManager;}}
    HealthManager hm;

    private void Awake(){
        player = FindObjectOfType<Player>();
        hm = FindObjectOfType<HealthManager>();
        gameManager = FindObjectOfType<GameManager>();
        
        tentacle = transform.GetChild(0).gameObject;
        tentacleAnim = tentacle.GetComponent<Animator>();
        tentacleAnim.GetComponent<UIActivate>().AddAction(ActivateKnife);
        knifeAnim = proteinKnife.GetComponent<Animator>();
        knifeAnim.GetComponent<UIActivate>().AddAction(ActivateProtein);
        proteinKnife.SetActive(false);
        tentacle.SetActive(false);
    }

    private void OnMouseUpAsButton(){
        if (!player.handFree) return;
        tentacle.SetActive(true);
        tentacleAnim.SetTrigger("ShowProtein");
        //when player clicks on UI, show tentacle for cutting
        //tentacle has 3 parts -> iamges need to be manually sliced
            //colour of sprite will be changed upon hover
        //when tentacle moves closer, hand should shake (more vigorously)
        //animate tentacle when clicked -> showing knife/allow interaction after anim
    }

    //called by tentacle anim uiactivate at end of animation
    public void ActivateKnife(){
        proteinKnife.SetActive(true);
        player.PickUpItem(proteinKnife);
    }

    public void AnimateCutTentacle(GameObject o){
        o.SetActive(false);
        newProtein = o;
        knifeAnim.SetTrigger(("CutProtein"));
    }

    //called by knife anim uiactivate at end of animation
    private void ActivateProtein(){
        hm.MinusPlayerHearts(1);
        player.DropItem("any");
        proteinKnife.gameObject.SetActive(false);
        player.PickUpItem(newProtein);
        newProtein.SetActive(true);
        newProtein = null;
    }

    public void CloseSelector(){
        tentacle.SetActive(false);
        tentacleAnim.Rebind();
        knifeAnim.Rebind();
    }
}

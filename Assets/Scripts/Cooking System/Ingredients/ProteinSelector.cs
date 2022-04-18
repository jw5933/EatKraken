using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProteinSelector : MonoBehaviour
{
    private Player player;
    private Animator tentacleAnim; //anims: ShowProtein - brings the tentacle up
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
        
        tentacleAnim = transform.GetChild(0).GetComponent<Animator>();
        tentacleAnim.GetComponent<UIActivate>().AddAction(ActivateKnife);
        knifeAnim = proteinKnife.GetComponent<Animator>();
        knifeAnim.GetComponent<UIActivate>().AddAction(ActivateProtein);
    }

    private void OnMouseUpAsButton(){
        if (!player.handFree) return;
        tentacleAnim.SetTrigger("ShowProtein");
        //when player clicks on UI, show tentacle for cutting
        //tentacle has 3 parts -> iamges need to be manually sliced
            //colour of sprite will be changed upon hover
        //when tentacle moves closer, hand should shake (more vigorously)
        //animate tentacle when clicked -> showing knife/allow interaction after anim
    }

    //called by tentacle anim uiactivate at end of animation
    public void ActivateKnife(){
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
        player.PickUpItem(newProtein);
        newProtein.SetActive(true);
        newProtein = null;
    }

    public void CloseSelector(){
        tentacleAnim.Rebind();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProteinSelector : MonoBehaviour
{
    private Vector3 initialPos;
    private Player player;
    GameObject tentacle;
    private Animator tentacleAnim; //anims: ShowProtein - brings the tentacle up -> should be first child under this object
    [SerializeField] private GameObject proteinKnife; //will be a tool type
    private Animator knifeAnim; ///anims: CutProtein - show protein being cut
    private GameObject newProtein;
    private GameObject planeUpdate;

    private GameManager gameManager;
    public GameManager gm {get{return gameManager;}}
    HealthManager hm;

    private void Awake(){
        player = FindObjectOfType<Player>();
        hm = FindObjectOfType<HealthManager>();
        gameManager = FindObjectOfType<GameManager>();
        
        planeUpdate = transform.GetChild(1).gameObject;
        tentacle = transform.GetChild(0).gameObject;
        tentacleAnim = tentacle.GetComponent<Animator>();
        tentacleAnim.GetComponent<UIActivate>().AddAction(ActivateKnife);
        knifeAnim = proteinKnife.GetComponent<Animator>();
        knifeAnim.GetComponent<UIActivate>().AddAction(ActivateProtein);

        initialPos = tentacle.transform.localPosition;
        proteinKnife.SetActive(false);
        tentacle.SetActive(false);
        planeUpdate.SetActive(false);
    }
    private void Update(){
        if (!tentacle.activeSelf) return;
        if (Input.GetKeyDown(KeyCode.Escape)){
            CloseSelector();
        }
    }

    private void OnMouseUpAsButton(){
        if (!player.handFree) return;
        planeUpdate.SetActive(true);
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
        CloseSelector();
        player.PickUpItem(newProtein);
        newProtein.SetActive(true);
        newProtein = null;
    }

    private void CloseSelector(){
        proteinKnife.gameObject.SetActive(false);
        planeUpdate.SetActive(false);
        tentacle.SetActive(false);
        tentacleAnim.Rebind();
        knifeAnim.Rebind();
        tentacle.transform.localPosition = initialPos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProteinSelector : MonoBehaviour
{
    private Player player;

    private void Awake(){
        player = FindObjectOfType<Player>();
    }
    private void OnMouseUp(){
        if (!player.handFree) return;

        //when player clicks on UI, show tentacle for cutting
        //tentacle has 3 parts -> iamges need to be manually sliced
            //colour of sprite will be changed upon hover
        //when tentacle moves closer, hand should shake (more vigorously)
        //animate tentacle when clicked -> showing knife/allow interaction after anim
    }

    public void ActivateKnife(){

    }
}

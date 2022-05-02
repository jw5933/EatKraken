using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSAnimation : MonoBehaviour
{
    //since the cutscene is small, we will just do it manually..
    [SerializeField] private List<int> frames = new List<int>();
    private List<string> triggers = new List<string>{"F1", "F2", "F3", "F4", "F5"};
    Animator myAnimator;
    [SerializeField] int endFrame;

    private void Awake(){
        if (triggers.Count != 0) FindObjectOfType<EventManager>().OnDialogueChange += CheckAdjust;
        myAnimator = GetComponent<Animator>();
    }

    private void CheckAdjust(int frame){
        if (endFrame == frame){
            this.gameObject.SetActive(false);
            return;
        }
        if (frames[0] != frame) return;
        AdjustImage();
    }

    private void AdjustImage(){
        myAnimator.Rebind();
        myAnimator.SetTrigger(triggers[0]);
        frames.RemoveAt(0);
        triggers.RemoveAt(0);
    }
}

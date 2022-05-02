using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    //beginning animation
    [SerializeField] Animator cutSceneAnim;
    DialogueManager dm;

    private void Awake(){
        //maxLen = (int)Mathf.Ceil(text.preferredWidth);
        cutSceneAnim.GetComponent<UIDeactivate>().AddAction(ActivateInput);
        dm = GetComponent<DialogueManager>();
    }

    //change dialogue if the player presses enter key
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.Space)){
            if (cutSceneAnim.gameObject.activeSelf)
                SkipCutScene();
            else 
                ActivateInput();
        }
    }

    public void SkipCutScene(){
        cutSceneAnim.GetComponent<UIDeactivate>().StopAnim();
    }

    public void ActivateInput(){
        //Debug.Log(dialogue.Count);
        if(!dm.GoNextDialogue()){ // if the dialogue is still being typed, finish typing
            SceneManager.LoadScene("New Main", LoadSceneMode.Single);
        }
    }
}
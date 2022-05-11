using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InstructionBook : MonoBehaviour
{
    private bool start = true;
    private UnityAction myAction;
    public UnityAction action {set{myAction = value;}}
    
    int index;
    [SerializeField] private Sprite[] pages;
    Image myImage;
    GameObject left, right, x;
    GameManager gm;

    private void Awake(){
        myImage = GetComponent<Image>();
        gm = FindObjectOfType<GameManager>();

        foreach(Transform c in transform){
            switch(c.name){
                case "left":
                    left = c.gameObject;
                    left.GetComponent<ColliderButton>().AddAction(GoPreviousPage);
                break;
                case "right":
                    right = c.gameObject;
                    right.GetComponent<ColliderButton>().AddAction(GoNextPage);
                break;
                case "x":
                    x = c.gameObject;
                    x.GetComponent<ColliderButton>().AddAction(CloseBook);
                break;
            }
        }

        left.SetActive(false);
    }
    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            CloseBook();
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
            GoPreviousPage();
        }
        else if(Input.GetKeyDown(KeyCode.D)|| Input.GetKeyDown(KeyCode.LeftArrow)){
            GoNextPage();
        }
    }

    private void GoNextPage(){
        if (index+1 >= pages.Length)return;
        index++;
        myImage.sprite = pages[index];
        if (!left.activeSelf) left.SetActive(true);
        if (index == pages.Length-1) right.SetActive(false);
    }

    private void GoPreviousPage(){
        if (index-1 < 0) return;
        index--;
        myImage.sprite = pages[index];
        if (!right.activeSelf) right.SetActive(true);
        if (index == 0) left.SetActive(false);
    }

    private void CloseBook(){
        this.transform.parent.gameObject.SetActive(false);
        if (start){
            start = false;
            myAction();
            action = null;
            Time.timeScale = 1;
        }
        else if (!gm.paused){
            AudioListener.pause = false;
            Time.timeScale = 1;
        }
        gm.ResetActiveScreen();
    }

    public void OpenBook(){
        gm.otherScreenOpen = true;
        if (!gm.paused){
            AudioListener.pause = true;
            Time.timeScale = 0;
        }
        this.transform.parent.gameObject.SetActive(true);
    }
}

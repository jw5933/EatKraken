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

    private void Awake(){
        myImage = GetComponent<Image>();

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
    }
    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            CloseBook();
        }
        else if (Input.GetKeyDown(KeyCode.A)){
            GoPreviousPage();
        }
        else if(Input.GetKeyDown(KeyCode.D)){
            GoNextPage();
        }
    }

    private void GoNextPage(){
        if (index+1 >= pages.Length) return;
        index++;
        myImage.sprite = pages[index];
    }

    private void GoPreviousPage(){
        if (index-1 < 0) return;
        index--;
        myImage.sprite = pages[index];
    }

    private void CloseBook(){
        this.transform.parent.gameObject.SetActive(false);
        if (start){
            myAction();
            action = null;
            start = false;
        }
    }
}

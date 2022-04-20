using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InstructionBook : MonoBehaviour
{
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
                    left.GetComponent<Button>().AddAction(GoPreviousPage);
                break;
                case "right":
                    right = c.gameObject;
                    right.GetComponent<Button>().AddAction(GoNextPage);
                break;
                case "x":
                    x = c.gameObject;
                    x.GetComponent<Button>().AddAction(CloseBook);
                break;
            }
        }
    }
    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            CloseBook();
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
        this.gameObject.SetActive(false);
    }
}

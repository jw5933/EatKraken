using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIAlpha : MonoBehaviour

{
    private Image i;
    private void Awake(){
        i = GetComponent<Image>();
    }
    
    public void setAlpha(float n){
        Color colour = i.color;
        colour.a = n;
        i.color = colour;
    }

    public void stopAnim(){
        gameObject.SetActive(false);
    }
}

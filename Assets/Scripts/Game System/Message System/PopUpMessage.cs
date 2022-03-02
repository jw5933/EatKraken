using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpMessage : MonoBehaviour
{
    /* a new pop up message is instantiated whenever we want a message */
    // ==============   variables   ==============
    public delegate void MyDelegate(bool b);
    MyDelegate myDelegate;
    private Text myText;
    private Text choiceText;
    // private bool madeChoice;
    // public bool choiceMade{set{madeChoice = value;}} //for childed buttons to mark whether or not a choice was made
    private bool myChoice;
    public bool choice{set{myChoice = value;}} //for childed buttons to set the choice
    private IEnumerator myCoroutine;
    // ==============   methods   ==============
    //constructor
    private void Awake(){
        myText = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        choiceText = myText.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>();
    }
    public void Init(string newText, string newChoiceText, MyDelegate newAction){
        myText.text = newText;
        choiceText.text = newChoiceText;
        myDelegate = newAction;
    }

    public void ReturnChoice(){
        myDelegate(myChoice);
        Destroy(this.gameObject);
    }
}

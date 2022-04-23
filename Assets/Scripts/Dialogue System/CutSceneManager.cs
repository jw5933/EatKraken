using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    public Action<int> OnDialogueChange;
    IEnumerator myCoroutine;

    //beginning animation
    [SerializeField] Animator cutSceneAnim;
    //text
    [SerializeField] Text text;
    int letterPerSec = 60; //how fast the typing is

    bool openText;

    [TextArea]
    [SerializeField] List <string> dialogue = new List<string>(); //the dialogue that will be typed
    [SerializeField] int index = 0; //which line is being typed
    bool isTyping; //condition for whether text is curerntly being typed

    [SerializeField] int maxLen; //760?

    private void Awake(){
        //maxLen = (int)Mathf.Ceil(text.preferredWidth);
        cutSceneAnim.GetComponent<UIDeactivate>().AddAction(BeginDialogue);
        for (int i = 0; i < dialogue.Count; i++){
            dialogue[i] = dialogue[i].ToUpper();
        }
    }

    //change dialogue if the player presses enter key
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Return)) CheckInput();
    }

    private void ChangeDialogue(){
        if (OnDialogueChange != null) OnDialogueChange(index);
    }

    private void BeginDialogue(){
        myCoroutine = DisplayDialogue();
        StartCoroutine(myCoroutine);
        ChangeDialogue();
    }

    public void CheckInput(){
        //Debug.Log(dialogue.Count);
        if(isTyping){ // if the dialogue is still being typed, finish typing
            StopCoroutine(myCoroutine);
            text.text = dialogue[index];
            isTyping = false;
        }
        else if(index < dialogue.Count-1){
            index++;
            myCoroutine = DisplayDialogue();
            StartCoroutine(myCoroutine);
            ChangeDialogue();
        }
        else{
            SceneManager.LoadScene("New Main", LoadSceneMode.Single);
        }
    }
        

    // ====================================== DIALOGUE (NON CHOICE) METHODS =============================
		// displays dialogue
	IEnumerator DisplayDialogue(){
        yield return new WaitForEndOfFrame();
        openText = true; //condition: dialogue is now open
        if (!isTyping){
            myCoroutine = TypeDialogue(dialogue[index]);
            StartCoroutine(myCoroutine); //begin typing the text
        }
    }

	// method to type dialogue
	IEnumerator TypeDialogue(string str){
        // -------------- typing effect ---------------------
        isTyping = true;
        text.text="";

        //local variables; this resets each time the method is called
        int wordIndex = 0;
        string [] words = str.Split(' ');
        int len = 0;
        bool firstWord = true;

        Font myFont = text.font;
        CharacterInfo spaceInfo = new CharacterInfo();
        myFont.GetCharacterInfo(' ', out spaceInfo, text.fontSize);
        //string currLine = "";

        //add a single character to the text at a time, thus creating a typing effect
        foreach(char l in str.ToCharArray()){
            char letter = l;
            if (!isTyping) yield return null;
            if (letter == '\n'){
                text.text += '\n';
                //currLine = "";
                len = 0;
                continue;
            }
            //if there is a new word do the following
            if((letter == ' '||firstWord) && wordIndex < words.Length){
                //string line = currLine + words[wordIndex]; //add the new word to the current line
                string word = words[wordIndex];
                int wordLen = 0;

                CharacterInfo charInfo = new CharacterInfo();
                
                if (!firstWord){
                    wordLen += spaceInfo.advance;
                }

                foreach(char c in word){
                    myFont.GetCharacterInfo(c, out charInfo, text.fontSize);
                    wordLen += charInfo.advance; //go on to the next character
                }
                len += wordLen;
                //Debug.Log("word: "+ words[wordIndex] + ", length: " + wordLen + ", total length: " + len);

                /* if (len > maxLen){
                    Debug.Log("over max: " + len + ", max: " + maxLen);
                    text.text += '\n';
                    // wordIndex++;
                    //currLine = "";
                    len = 0;
                    if (!firstWord) len -= spaceInfo.advance;
                    continue;
                } */
                //len = 0;
                //find the font/text info
                /* foreach(char c in line){
                    myFont.GetCharacterInfo(c, out charInfo, text.fontSize);
                    len += charInfo.advance; //go on to the next character
                } */
                firstWord = false;
                wordIndex++;
            }
            if(isTyping){
                text.text += letter; //otherwise just add the letter
                //currLine = currLine += letter;
                yield return new WaitForSeconds(1f/letterPerSec);
            }
        }
        isTyping = false;

        if(index >= dialogue.Count-1){
            openText = false;
        }
    }
}
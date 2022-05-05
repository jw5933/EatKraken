using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    int letterPerSec = 60;
    bool isTyping;

    [TextArea(3, 8)]
    [SerializeField] List <string> dialogue = new List<string>(); //the dialogue that will be typed
    IEnumerator myCoroutine;
    int index = -1; //which line is being typed

    [SerializeField] private TextMeshProUGUI tmp;
    public GameObject textmp {get{return tmp.transform.parent.gameObject;}}
    private EventManager em;

    private void Awake(){
        em = FindObjectOfType<EventManager>();
        /* for (int i = 0; i < dialogue.Count; i++){
            dialogue[i] = dialogue[i].ToUpper();
        } */
    }

    public void AddDialogue(string s){
        dialogue.Add(s);
    }

    private IEnumerator TypeDialogue(string s){
        isTyping = true;
        tmp.text = s;
        int totalVisibleCharacters = s.Length;
        Debug.Log("tvc: " + totalVisibleCharacters);
        
        int counter = 0;

        while (true){
            int visibleCount = counter % (totalVisibleCharacters+1);
            //Debug.Log("visible count: " + visibleCount);
            tmp.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalVisibleCharacters){
                isTyping = false;
                yield break;
            }

            counter++;
            yield return new WaitForSecondsRealtime(1f/letterPerSec);
        }
    }

    //if there is more text return true, else false
    public bool GoNextDialogue(){
        if(isTyping){ // if the dialogue is still being typed, finish typing
            StopCoroutine(myCoroutine);
            tmp.maxVisibleCharacters = tmp.text.Length;
            isTyping = false;
            return true;
        }
        else if(index < dialogue.Count-1){
            index++;
            tmp.maxVisibleCharacters = 0;
            myCoroutine = TypeDialogue(dialogue[index]);
            StartCoroutine(myCoroutine);
            em.ChangeDialogue(index);
            return true;
        }
        return false;
    }

    public bool GoLastDialogue(){
        if (isTyping){
            StopCoroutine(myCoroutine);
        }
        index = dialogue.Count-1;
        tmp.maxVisibleCharacters = 0;
        myCoroutine = TypeDialogue(dialogue[index]);
        StartCoroutine(myCoroutine);
        em.ChangeDialogue(index);
        return true;
    }

}

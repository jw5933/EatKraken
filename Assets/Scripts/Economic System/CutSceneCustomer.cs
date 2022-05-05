using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneCustomer : Customer
{
    [SerializeField] private int wantedPos = 2;
    DialogueManager dialogueManager;
    DayManager dm;
    [TextArea(3, 8)]
    [SerializeField] string finalWords = "";

    public override void Awake(){
        dm = FindObjectOfType<DayManager>();
        dialogueManager = GetComponent<DialogueManager>();
        dialogueManager.textmp.SetActive(false);
        base.Awake();
    }

    //change dialogue if the player presses enter key
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.Space)){
            ActivateInput();
        }
    }

    protected override void Activate(){
        dialogueManager.textmp.SetActive(true);
        base.Activate();
        dm.ShrinkMeter();
        dialogueManager.GoNextDialogue();
    }

    public void ActivateInput(){
        if(!dialogueManager.GoNextDialogue()){ // if the dialogue is still being typed, finish typing
            Debug.Log("Finished customer dialogue");
        }
    }

    public override void Init(int posInLine){
        if (positionInLine != 2){
            if (cm.TrySwapPosition(posInLine, wantedPos)){
                posInLine = wantedPos;
            }
        }
        em.OnTimeChange += Leave;
        base.Init(posInLine);
    }

    //this customer will always leave on the next phase, so myTimePhase + 1 will not pass generator's test
    // 1 -> 2 <= 2
    //overloading parent method so that we can change the sequence that the leave happens in
    //initial leave will do nothing
    private void Leave(float f, int i){ 
        em.OnTimeChange -= (Leave);
        if (finalWords != "") dialogueManager.AddDialogue(finalWords);
        else Debug.Log("no final words");

        if(dialogueManager.GoLastDialogue())
            Debug.Log("saying final words");//do nothing
        StartCoroutine(LeaveInTwo());
    }

    IEnumerator LeaveInTwo(){
        yield return new WaitForSeconds(1.7f);
        Destroy(myCustomerAnim.gameObject);
        em.FreeCustomer(this, positionInLine, myMood, myTimePhase+1); //em OnCustomerLeave
    }

    public override void CreateAppearance(){
        base.CreateAppearance();
        myCustomerAnim.gameObject.GetComponent<UIDeactivate>().AddAction(Wait);
    }

    private void Wait(){
        myCustomerAnim.gameObject.SetActive(true);
        dm.ResizeMeter();
    }
}

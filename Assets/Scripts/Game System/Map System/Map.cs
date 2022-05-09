using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class Map : MonoBehaviour, IPointerClickHandler
{
    // ==============   variables   ==============
    private GameManager gm;
    private EventManager em;

    [SerializeField] private GameObject map;
    [SerializeField] private Location currLocation; //debug
    public Location location {get{return currLocation;}}
    [SerializeField] private Location nextLocation; //debug
    public Location selectedLocation{set{nextLocation = value;}}
    private PopUpMessage myMessage;
    [SerializeField] private bool validRelocation; //debug
    private Economy econ;

    [SerializeField] private GameObject locationAnim;
    private InstructionBook book;


    // ==============   methods   ==============
    private void Awake(){
        gm = FindObjectOfType<GameManager>();
        econ = FindObjectOfType<Economy>();
        em = FindObjectOfType<EventManager>();
        book = FindObjectOfType<InstructionBook>();
        book.action = GoNextLocation;
        if (locationAnim !=null) locationAnim.GetComponent<UIDeactivate>().AddAction(OpenBook);
    }

    private void Start(){
        if (nextLocation !=null) PlayGoNextAnimation();
    }

    private void PlayGoNextAnimation(){ //end of animation will call go next location
        //Debug.Log("anim");
        //play animation
        if (locationAnim !=null){
            locationAnim.SetActive(true);
            locationAnim.GetComponent<Animator>().SetTrigger("MoveLocation");
        }
        else{
            OpenBook();
        }
    }

    private void OpenBook(){
        gm.otherScreenOpen = true;
        book.gameObject.SetActive(true);
    }

    //go to the location the player wants
    public void GoNextLocation(){
        Debug.Log("next");
        OpenOrCloseMap();

        //update location by unsetting current and setting next
        if (currLocation != null) currLocation.UnsetLocation();
        if (nextLocation != null){
            //Debug.Log("setting next location");
            nextLocation.SetLocation();
            //let subscribers know location has changed
            em.ChangeLocation(nextLocation); 
        }

        UpdateMapUI();
        currLocation = nextLocation;
        nextLocation = null;
    }

    //update the map's UI to show change in location
    private void UpdateMapUI(){
        //change scale
        if (currLocation != null) currLocation.ResetScale();
        if (nextLocation != null) nextLocation.ChangeScale(new Vector3(0.5f, 0.5f, 0.5f));
    }

    //check if the player can relocate, and notify the player of the validation results
    public void ValidateRelocation(){
        if (nextLocation == null) return;

        //check if the player can move, economically
        if (econ.playerAccount >= nextLocation.relocationPrice){
            validRelocation = true;
        }
        else validRelocation = false;

        //ask the player what they want to do, based on whether they can move or not
        myMessage = IndicateRelocation(validRelocation, nextLocation.gameObject.name, nextLocation.relocationPrice);
    }

    //create a pop up message to indicate whether or not player can relocate
    private PopUpMessage IndicateRelocation(bool v, string l, float p){
        //if the player can move, 
        string popUpText = "You " + (v? "have enough ": "don't have enough ") + "coins to move to " + l + ". " 
                            + (v? "Are you sure you want to use ": "Come back when you have ") + p 
                            + (v? (" coins to relocate? You will have " + (econ.playerAccount - p) + " coins left."): " coins.");
        string popUpChoiceText = v? "Confirm" : "Got it!";
        if (gm.popUpMessagePrefab != null){
            myMessage = Instantiate(gm.popUpMessagePrefab).GetComponent<PopUpMessage>(); //instantiate popup message
            myMessage.transform.SetParent(gm.gameCanvas.transform, false);
            if (myMessage != null){
                myMessage.Init(popUpText, popUpChoiceText, HandlePopUpResult);
            }
        }
        return null;
    }

    public void HandlePopUpResult(bool result){
        Debug.Log("delegate entered");
        if (result){ //if player can relocate, go to the next location
            if (validRelocation) 
                GoNextLocation();
        }
    }

    private void OpenOrCloseMap(){
        if (map.activeSelf) map.SetActive(false);
        else map.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData){
            OpenOrCloseMap();
    }
}

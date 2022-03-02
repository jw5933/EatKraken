using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    // ==============   variables   ==============
    [SerializeField] private GameObject customerView;
    [SerializeField] private GameObject orderView;

    private GameObject cam;
    [SerializeField] private List <CinemachineVirtualCamera> virtualCams;
    [SerializeField] private List <CinemachineVirtualCamera> virtualUpCams;

    public int camIndex {get; private set;}
    public int maxCamIndex {get{return virtualCams.Count;}}

    [SerializeField] private CameraButton leftButton;
    [SerializeField] private CameraButton rightButton;
    [SerializeField] private CameraButton upButton;
    [SerializeField] private CameraButton downButton;

    private CameraButton[] buttons;

    private Player p;
    private GameObject health;

    // ==============   methods   ==============
    void Start(){
        health = FindObjectOfType<HealthManager>().gameObject;
        p = FindObjectOfType<Player>();
        cam = FindObjectOfType<Camera>().gameObject;

        buttons = new CameraButton[] {leftButton, rightButton, upButton, downButton};
        camIndex = 1;
        //SwapToCam(camIndex);
        if (virtualUpCams.Count >0) SwapUpDownCam();
        
    }
    
    void Update()
    {
        CheckMoveInput();
    }

    private void CheckMoveInput(){
        if (Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow)){
            SwapToCam(camIndex-1 >=0 ? camIndex-1: camIndex);
        }
        else if (Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow)){
            SwapToCam(camIndex+1 < maxCamIndex ? camIndex+1: camIndex);
        }
        else if (Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow)){
            if (virtualUpCams.Count >0) SwapUpDownCam();
        }
        else if (Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.DownArrow)){
            if (virtualUpCams.Count >0) SwapUpDownCam();
        }
    }

    public void SwapToCam(int n){ //current, new
        if (camIndex == n || virtualCams[camIndex].Priority == 10) return;

        //swap the camera
        int c = camIndex;
        virtualCams[n].Priority = 11;
        virtualCams[c].Priority = 10;
        camIndex = n;

        ShowUI();

        //move the customer view to be above the new cam;
        Vector3 newCustomerViewPos = new Vector3 (virtualCams[camIndex].transform.position.x, customerView.transform.position.y, 0);
        customerView.transform.position = newCustomerViewPos;

        //move order view
        Vector3 newOrderViewPos = new Vector3 (virtualCams[camIndex].transform.position.x, orderView.transform.position.y, 0);
        orderView.transform.position = newOrderViewPos;
    }

    public void SwapUpDownCam(){
        if (virtualCams[camIndex].Priority == 11){ //swap up to customer cam
            HideUI();
            virtualUpCams[camIndex].Priority = 11;
            virtualCams[camIndex].Priority = 10;
            ShowUpDownButtons();
        }
        else{//swap down to game cam
            virtualCams[camIndex].Priority = 11;
            virtualUpCams[camIndex].Priority = 10;
            ShowUpDownButtons();
            ShowUI();
        }
    }

    private void ShowUpDownButtons(){
        if (virtualCams[camIndex].Priority == 11){ //show up button
            downButton.gameObject.SetActive(false);
            upButton.gameObject.SetActive(true);
        }
        else{ //show down button
            downButton.gameObject.SetActive(true);
            upButton.gameObject.SetActive(false);
        }
    }

    private void HideButtons(){
        foreach(CameraButton c in buttons){
            if (c != null){
                c.gameObject.SetActive(false);
                c.ResetScale();
            }
        }
    }

    private void ShowButtons(){
        switch (camIndex){
            case 0: //left -> don't show left arrow
            leftButton.ResetScale();
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(true);
            break;

            case 1: //mid -> show both arrows
            //Debug.Log("cam index 1");
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(true);
            break;

            case 2: //right -> don't show right arrow
            rightButton.ResetScale();
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(false);
            break;
        }
        if (upButton !=null) ShowUpDownButtons();
    }

    private void ShowHealth(){
        health.SetActive(true);
    }
    private void HideHealth(){
        health.SetActive(false);
    }

    public void ShowUI(){
        ShowButtons();
        ShowHealth();
    }

    public void HideUI(){
        HideButtons();
        HideHealth();
    }
}

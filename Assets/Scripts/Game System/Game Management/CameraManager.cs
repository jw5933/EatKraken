using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    // ==============   variables   ==============
    [SerializeField] private GameObject customerView;

    private GameObject cam;
    [SerializeField] private List <CinemachineVirtualCamera> virtualCams;
    [SerializeField] private List <CinemachineVirtualCamera> virtualUpCams;

    public int camIndex {get; private set;}
    public int maxCamIndex {get{return virtualCams.Count;}}

    [SerializeField] private CameraHover leftButton;
    [SerializeField] private CameraHover rightButton;

    private CameraHover[] buttons;

    private Player p;
    private GameObject health;
    private Tentacle t;
    private GameObject book;

    // ==============   methods   ==============
    void Start(){
        health = FindObjectOfType<HealthManager>().gameObject;
        t = FindObjectOfType<Tentacle>();
        p = FindObjectOfType<Player>();
        cam = FindObjectOfType<Camera>().gameObject;
        book = FindObjectOfType<InstructionBook>().transform.parent.gameObject;

        buttons = new CameraHover[] {leftButton, rightButton};
        camIndex = 1;
    }
    
    void Update()
    {
        if (!book.activeSelf && Time.timeScale != 0) CheckMoveInput();
    }

    private void CheckMoveInput(){
        if (Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow)){
            SwapToCam(camIndex-1 >=0 ? camIndex-1: camIndex);
            t.UpdateEndPos(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow)){
            SwapToCam(camIndex+1 < maxCamIndex ? camIndex+1: camIndex);
            t.UpdateEndPos(1);
        }
    }

    public void SwapToCam(int n){ //current, new
        if (camIndex == n || virtualCams[camIndex].Priority == 10) return;

        //swap the camera
        int c = camIndex;
        virtualCams[n].Priority = 11;
        virtualCams[c].Priority = 10;
        camIndex = n;
        ShowButtons();

        /* //move the customer view to be above the new cam;
        Vector3 newCustomerViewPos = new Vector3 (virtualCams[camIndex].transform.position.x, customerView.transform.position.y, 0);
        customerView.transform.position = newCustomerViewPos; */

        /* //move order view
        Vector3 newOrderViewPos = new Vector3 (virtualCams[camIndex].transform.position.x, orderView.transform.position.y, 0);
        orderView.transform.position = newOrderViewPos; */
    }

    private void ShowButtons(){
        switch (camIndex){
            case 0: //left -> don't show left arrow
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(true);
            break;

            case 1: //mid -> show both arrows
            //Debug.Log("cam index 1");
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(true);
            break;

            case 2: //right -> don't show right arrow
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(false);
            break;
        }
    }
}

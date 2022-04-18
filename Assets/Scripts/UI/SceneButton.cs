using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    private Sprite initialImg;
    [SerializeField] private Sprite hoverImg;

    Image myImage;
    IEnumerator myCoroutine;

    private void Awake(){
        myImage = GetComponent<Image>();
        initialImg = myImage.sprite;
    }

    private void Click(){
        myCoroutine = LoadYourAsyncScene();
        StartCoroutine(myCoroutine);
    }

    private void OnMouseUpAsButton(){
        this.enabled = false;
        transform.parent.GetComponent<Animator>().SetTrigger("ClickedButton");
        transform.parent.GetComponent<UIActivate>().AddListener(Click);
    }
    /* public void OnMouseEnter(){
        myImage.sprite = hoverImg;
    }
    public void OnMouseExit(){
        if (myCoroutine == null) myImage.sprite = initialImg;
    } */

    //via Unity Documentation
    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("New Main", LoadSceneMode.Single);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}

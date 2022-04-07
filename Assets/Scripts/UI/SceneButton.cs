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

    public void OnMouseUp(){
        myCoroutine = LoadYourAsyncScene();
        StartCoroutine(myCoroutine);
    }
    public void OnMouseEnter(){
        myImage.sprite = hoverImg;
    }
    public void OnMouseExit(){
        if (myCoroutine == null) myImage.sprite = initialImg;
    }

    //via Unity Documentation
    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("bgStuffAdded", LoadSceneMode.Single);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}

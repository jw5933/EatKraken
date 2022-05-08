using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSProfile : MonoBehaviour
{
    [SerializeField] private List<int> frames = new List<int>();
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    Image myImage;
    [SerializeField] int endFrame;

    private void Awake(){
        if (sprites.Count != 0) FindObjectOfType<EventManager>().OnDialogueChange += CheckAdjust;
        myImage = GetComponent<Image>();
    }

    private void CheckAdjust(int frame){
        if (endFrame == frame){
            this.gameObject.SetActive(false);
            return;
        }
        if (frames.Count == 0) return;
        if (frames[0] != frame) return;
        AdjustImage();
    }

    private void AdjustImage(){
        myImage.sprite = sprites[0];
        frames.RemoveAt(0);
        sprites.RemoveAt(0);
    }
}

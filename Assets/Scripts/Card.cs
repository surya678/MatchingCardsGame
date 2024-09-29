using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Card : MonoBehaviour
{
    public Sprite frontSprite; // Assign the front sprite in the inspector
    public Sprite backSprite; // Assign the back sprite in the inspector
    public Image inside;
    private bool isFaceUp = false;
    public int cardID;


    private void Start()
    {
        // Start with the back side facing up
       // inside.sprite = backSprite;
        inside.sprite = frontSprite;
        Invoke("FlipBack", 2);
    }

    private void FlipBack()
    {        
        this.GetComponent<Animator>().Play("cardFlip");
        Invoke("AssignCard", 0.2f);
    }
    private void AssignCard()
    {
        inside.sprite = backSprite;
    }

    public void Flip()
    {
        isFaceUp = !isFaceUp;        
        this.GetComponent<Animator>().Play("cardFlip");
        Invoke("FlipCardassigned", 0.8f);
    }
    private void FlipCardassigned()
    {
        inside.sprite = isFaceUp ? frontSprite : backSprite;
    }
    public bool IsFaceUp()
    {
        return isFaceUp;
    }

    public void Click()
    {
        FindObjectOfType<GameManager>().OnCardSelect(this);
    }
}

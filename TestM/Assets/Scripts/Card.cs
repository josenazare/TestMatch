using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour,IPointerDownHandler
{
    public CardSO CardSOData; // ScriptableObject reference
    public bool isFlipped = false; // Whether the card is flipped
    public Sprite CardImage; // Front side image of the card
    public bool isMatched = false;
    Quaternion initialQuart = new Quaternion();
    Quaternion FinalQuart = new Quaternion();


    // Reference to the UI Image to display the card

    private void Start()
    {
        if (CardSOData != null)
        {
            // Assign the sprite from the ScriptableObject to the card
            CardImage = CardSOData.Image;

        }
        else
        {
            if (isMatched)
            {
                Debug.Log("12 " + gameObject.name);
                gameObject.GetComponent<Image>().enabled = false;
            }
        }
    }

 public   IEnumerator RotateCard(GameObject Card,bool FlipBack = true)
    {
        GameManager.instance.eventSystem.gameObject.SetActive(false);
        initialQuart = Card.transform.rotation;
        FinalQuart = Card.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        float elapsedTime = 0f;

        // Smoothly interpolate over time
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime;


              Card.transform.rotation = Quaternion.Slerp(initialQuart,FinalQuart,t);
            if (elapsedTime > 0.5f)
            {
                if (FlipBack)
                {
                    Card.GetComponent<Image>().sprite = CardSOData.Image;
                }
                else
                {
                    Card.GetComponent<Image>().sprite = UIManager.instance.CardBackImage;
                }
            }

            yield return null; // Wait for the next frame
        }
        GameManager.instance.eventSystem.gameObject.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(GameManager.instance.CardInBuffer != null && GameManager.instance.CurrentlySelected != null)
        {
            return;
        }

        AudioHandler.Instance.audioSource.PlayOneShot(AudioHandler.Instance.FlipAudio);
       GameManager.instance.eventSystem.gameObject.SetActive(false);
        //  GameManager.instance.eventSystem.enabled = false;
        isFlipped = true;
        if (GameManager.instance.CardInBuffer == null)
        {
            GameManager.instance.CardInBuffer = this;

            GameManager.instance.CardInBuffer.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(RotateCard(GameManager.instance.CardInBuffer.gameObject));
        }
        else
        {
            GameManager.instance.CurrentlySelected = this;
            GameManager.instance.CurrentlySelected.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(RotateCard(GameManager.instance.CurrentlySelected.gameObject));
            GameManager.instance.CheckPair();
        }
    }
}


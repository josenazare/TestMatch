using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<CardSO> AllCardsData;
    public int NumberOfTries = 0, NumberOfMatches = 0;
    public Card CardInBuffer, CurrentlySelected;
    public int NumberOfPossiblePairs;
    public EventSystem eventSystem;
    [SerializeField] List<GameObject> GeneratedCards = new List<GameObject>();
    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        eventSystem.gameObject.SetActive(false);
        UIManager.instance.GenerateCards();
    }

    public IEnumerator InitialFlipCards()
    {
        yield return new WaitForSeconds(1f);
        AddAllCardsToFlip();
       
    }

    public  void CheckPair()
    {
        eventSystem.gameObject.SetActive(false);
        if (CardInBuffer.CardSOData.ID == CurrentlySelected.CardSOData.ID)
        {
            Debug.Log("correct");
            CardInBuffer.isMatched = true;
            CurrentlySelected.isMatched = true;
            StartCoroutine(SlightDelayForVisualCorrect());

            NumberOfMatches++;
            UIManager.instance.matchesTxt.text = NumberOfMatches.ToString();

            if(NumberOfMatches == NumberOfPossiblePairs)
            {
                Debug.Log("END");
            }

        }
        else
        {
            Debug.Log("wrong");
            StartCoroutine(SlightDelayForVisualWrong());

        }

        NumberOfTries++;
        UIManager.instance.triesTxt.text = NumberOfTries.ToString();
      
    }

    private void AddAllCardsToFlip()
    {
        int children = UIManager.instance.gridLayout.transform.childCount;

        for (int i = 0; i < children; i++)
        {
            GeneratedCards.Add(UIManager.instance.gridLayout.transform.GetChild(i).gameObject);
        }


        StartCoroutine(RotateObjectsSmoothly());
    }


    IEnumerator RotateObjectsSmoothly()
    {
        float elapsedTime = 0f;

        // Store the initial and target rotations for each GameObject
        List<Quaternion> initialRotations = new List<Quaternion>();
        List<Quaternion> targetRotations = new List<Quaternion>();

        foreach (var obj in GeneratedCards)
        {
                initialRotations.Add(obj.transform.rotation);
                targetRotations.Add(obj.transform.rotation * Quaternion.Euler(0f, 180f, 0f));
        }

        // Smoothly interpolate over time
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 1f;

            if(elapsedTime > 0.5f)
            {
                for (int i = 0; i < GeneratedCards.Count; i++)
                {
                    
                        GeneratedCards[i].GetComponent<Image>().sprite = UIManager.instance.CardBackImage;
                    
                }
            }


            for (int i = 0; i < GeneratedCards.Count; i++)
            {
                if (GeneratedCards[i] != null)
                {
                    GeneratedCards[i].transform.rotation = Quaternion.Slerp(initialRotations[i], targetRotations[i], t);
                }
            }

            yield return null; // Wait for the next frame
        }

        // Ensure final rotations are applied
        for (int i = 0; i < GeneratedCards.Count; i++)
        {
            if (GeneratedCards[i] != null)
            {
                GeneratedCards[i].transform.rotation = initialRotations[i];
            }
        }
        eventSystem.gameObject.SetActive(true);
    }

    IEnumerator SlightDelayForVisualWrong()
    {
        CardInBuffer.isFlipped = false;
        CurrentlySelected.isFlipped = false;
        yield return new WaitForSeconds(1f);
        StartCoroutine(CardInBuffer.RotateCard(CardInBuffer.gameObject, false));
        StartCoroutine(CurrentlySelected.RotateCard(CurrentlySelected.gameObject, false));
        CardInBuffer.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        CurrentlySelected.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        CardInBuffer = CurrentlySelected = null;
        eventSystem.gameObject.SetActive(true);
    }

    IEnumerator SlightDelayForVisualCorrect()
    {
        yield return new WaitForSeconds(1.5f);
        CardInBuffer.GetComponent<Image>().enabled = false;
        CurrentlySelected.GetComponent<Image>().enabled = false;
        CardInBuffer = CurrentlySelected = null;
        eventSystem.gameObject.SetActive(true);
    }
}
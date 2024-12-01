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
        StartCoroutine(RotateObjectsSmoothly());

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
               AudioHandler.Instance.audioSource.PlayOneShot(AudioHandler.Instance.OverAudio);
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
        yield return new WaitForSeconds(1.5f);
        AudioHandler.Instance.audioSource.PlayOneShot(AudioHandler.Instance.WrongAudio);
        StartCoroutine(CardInBuffer.RotateCard(CardInBuffer.gameObject, false));
        StartCoroutine(CurrentlySelected.RotateCard(CurrentlySelected.gameObject, false));
        CardInBuffer.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        CurrentlySelected.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        CardInBuffer = CurrentlySelected = null;
        eventSystem.gameObject.SetActive(true);
    }

    IEnumerator SlightDelayForVisualCorrect()
    {
        yield return new WaitForSeconds(1f);
        AudioHandler.Instance.audioSource.PlayOneShot(AudioHandler.Instance.CorrectAudio);
        CardInBuffer.GetComponent<Image>().enabled = false;
        CurrentlySelected.GetComponent<Image>().enabled = false;
        CardInBuffer = CurrentlySelected = null;
        eventSystem.gameObject.SetActive(true);
    }


    public void Save()
    {
        SaveCards(GeneratedCards);

        PlayerPrefs.SetInt("Tries", NumberOfTries);
        PlayerPrefs.SetInt("Matches", NumberOfMatches);
       
    }

    public void Load()
    {
        AudioHandler.Instance.audioSource.PlayOneShot(AudioHandler.Instance.FlipAudio);
        foreach (Transform child in UIManager.instance.gridLayout.transform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
        LoadCardData();

       UIManager.instance.triesTxt.text = PlayerPrefs.GetInt("Tries").ToString();
        UIManager.instance.matchesTxt.text = PlayerPrefs.GetInt("Matches").ToString();

    }

    private void SaveCards(List<GameObject> cardObjects)
    {
        CardSaveData saveData = new CardSaveData();

        foreach (GameObject cardObject in cardObjects)
        {
            if (cardObject != null)
            {
                Card card = cardObject.GetComponent<Card>();


                if (card != null && card.isMatched == false)
                {
                    CardData cardData = new CardData
                    {
                        CardSO_ID = card.CardSOData.ID,
                        isFlipped = card.isFlipped,
                        isMatched = card.isMatched,
                        //  CardImageData = card.CardSOData.Image

                    };
                    saveData.Cards.Add(cardData);
                }
                else
                {
                    CardData cardData = new CardData
                    {
                        CardSO_ID = -1,
                        isFlipped = true,
                        isMatched = true,

                    };
                    saveData.Cards.Add(cardData);
                }
            }
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("CardSaveData", json);
        PlayerPrefs.Save();
        Debug.Log(json);
    }

      
    private void LoadCardData()
    {
        Debug.Log("LOADING...1");

        // Get the saved data from PlayerPrefs
        string data = PlayerPrefs.GetString("CardSaveData");

        if (!string.IsNullOrEmpty(data))
        {
            Debug.Log("LOADING...2");

            // Deserialize the JSON data into the CardSaveData object
            CardSaveData saveData = JsonUtility.FromJson<CardSaveData>(data);

            if (saveData != null && saveData.Cards.Count > 0)
            {

                for (int i = 0; i < saveData.Cards.Count; i++)
                {
                    // Instantiate the card
                    GameObject clone = Instantiate(UIManager.instance.card, UIManager.instance.gridLayout.transform);

                    // Get the Card component
                    Card cardComponent = clone.GetComponent<Card>();
                    if (cardComponent == null)
                    {
                        Debug.LogError($"Card component missing on instantiated object at index {i}");
                        continue;
                    }

                    //// Set card properties
                    cardComponent.isFlipped = saveData.Cards[i].isFlipped;
                    cardComponent.isMatched = saveData.Cards[i].isMatched;

                    //// Assign the matching CardSOData using the ID
                    CardSO matchingCardSO = AllCardsData.Find(data => data.ID == saveData.Cards[i].CardSO_ID);
                    if (matchingCardSO != null)
                    {
                        cardComponent.CardSOData = matchingCardSO;

                        if (cardComponent.isFlipped)
                        {
                            cardComponent.GetComponent<Image>().sprite = cardComponent.CardSOData.Image;
                        }
                        else
                        {
                            cardComponent.GetComponent<Image>().sprite = UIManager.instance.CardBackImage;

                        }

                        if (cardComponent.isFlipped && cardComponent.isMatched == false)
                        {
                            CardInBuffer = cardComponent;
                        }

                        //if (cardComponent.isMatched)
                        //{
                           
                        //    cardComponent.GetComponent<Image>().enabled = false;
                            
                        //}
                        //   cardComponent.CardSOData.Image = matchingCardSO.Image; // Assuming frontImage is stored in CardSO


                    }
                    else
                    {
                        Debug.LogWarning($"No matching CardSO found for ID: {saveData.Cards[i].CardSO_ID}");
                    }


                    Debug.Log("LOADING...");
                }

                GeneratedCards.Clear();
                AddAllCardsToFlip();

               // SetImages();
            }
            else
            {
                Debug.LogWarning("No cards found in saved data.");
            }
        }
        else
        {
            Debug.LogWarning("No saved data found in PlayerPrefs.");
        }
    }


}
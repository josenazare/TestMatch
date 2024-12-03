using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public GridLayoutGroup gridLayout;
    public RectTransform gridLayoutTransform;
    public GameObject card, EmptyGameObject,GameScreen,GameOverScreen;
    public int rowSize, columnSize;
    bool isEven;
    public Sprite CardBackImage;
    public List<Sprite> CardImages;
 public   TextMeshProUGUI triesTxt, matchesTxt;
    public static UIManager instance;
    [SerializeField] Button Save;
    public Button Load;
   [SerializeField] int midRow;
   [SerializeField] int midColumn;
  [SerializeField]  bool FirstLaunch = false; 
    private void Awake()
    {
        instance = this;
    }

private void Start()
    {
        CheckForSave();
       
    }


    private void CheckForSave()
    {
        string data = PlayerPrefs.GetString("CardSaveData");
        if (string.IsNullOrEmpty(data))
        {
            Load.enabled = false;
        }
        else
        {
            Load.enabled = true;
        }
    }
    public void initRowColumnData()
    {
        if (FirstLaunch == true)
        {
            rowSize = Random.Range(2, 7);
            columnSize = Random.Range(2, 7);
        }
        else
        {
            FirstLaunch = true;
            rowSize = columnSize = 2; // added for first use so user gets an easy game 
        }

        if (columnSize < rowSize)
        {
            columnSize = rowSize;
        }

   
        if (rowSize * columnSize % 2 == 0)
        {
            isEven = true;
        }
        else
        {
            isEven = false;
        }

        if (!isEven)
        {
            midRow = rowSize / 2;
            midColumn = columnSize / 2;

        }
        GameManager.instance.NumberOfPossiblePairs = rowSize * columnSize / 2;
    }

    public List<CardSO> list1 = new List<CardSO>();
    public List<CardSO> list2 = new List<CardSO>();
    public void GetDimensions()
    {
        //Get total container dimensions
        float totalAvailableWidth = gridLayoutTransform.rect.width;
        float totalAvailableHeight = gridLayoutTransform.rect.height;

        //  dynamic spacing
        float spacingX = totalAvailableWidth * 0.02f; // 2% of container width
        float spacingY = totalAvailableHeight * 0.02f; // 2% of container height

        // Calculate card size by subtracting total spacing
        float cardWidth = (totalAvailableWidth - (spacingX * (columnSize - 1))) / columnSize;
        float cardHeight = (totalAvailableHeight - (spacingY * (rowSize - 1))) / rowSize;

        // Update GridLayout settings
        gridLayout.cellSize = new Vector2(cardWidth, cardHeight);
        gridLayout.spacing = new Vector2(spacingX, spacingY);
    }
     
    public List<Card> cardsGeneratedData = new List<Card>();
    public void GenerateCards()
    {

        GetDimensions();


        // Instantiate cards dynamically
        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < columnSize; j++)
            {
                // Instantiate the card prefab


                if (!isEven)
                {
                    if (i == midRow && j == midColumn)
                    {
                      GameObject emptyGO =  Instantiate(EmptyGameObject, gridLayout.transform);
                        Card EmptyC = emptyGO.GetComponent<Card>();
                        EmptyC.CardSOData = GameManager.instance.EmptyData;
                        cardsGeneratedData.Add(EmptyC);
                        continue;
                    }
                }
                GameObject cardClone = Instantiate(card, gridLayout.transform);
                Card cardComponent = cardClone.GetComponent<Card>();
                cardsGeneratedData.Add(cardComponent);
            }
        }                 

        
         

          for (int k = 0; k < GameManager.instance.NumberOfPossiblePairs; k++)
          {
              list1.Add(GameManager.instance.AllCardsData[k]);
              list2.Add(GameManager.instance.AllCardsData[k]);
          }


          ShuffleList(list1);
          ShuffleList(list2);

        if (!isEven)
        {
            list1.Add(GameManager.instance.EmptyData);
        }
          list1.AddRange(list2);

        
        for (int i = 0; i < cardsGeneratedData.Count; i++)
        {
            if (!isEven)
            {
                if (i == cardsGeneratedData.Count / 2)
                {
                    //cardsGeneratedData[i].CardSOData = list1[i];
                    cardsGeneratedData[i].GetComponent<Image>().sprite = null;
                    continue;
                }
            }
            cardsGeneratedData[i].CardSOData = list1[i];
            cardsGeneratedData[i].GetComponent<Image>().sprite = list1[i].Image;
        }

        StartCoroutine(GameManager.instance.InitialFlipCards());
    }

    private void ShuffleList(List<CardSO> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardSO temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}


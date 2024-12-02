using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static UnityEditor.Progress;

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
    int midRow;
    int midColumn;
   
    private void Awake()
    {
        instance = this;
    }

private void Start()
    {
        initRowColumnData();
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
        rowSize =  Random.Range(2, 7);
        columnSize =  Random.Range(2, 7);

        if (columnSize < rowSize)
        {
            columnSize = rowSize;
        }

   
        if (rowSize * columnSize % 2 == 0)
        {
            isEven = true;
        }

        if (!isEven)
        {
            midRow = rowSize / 2;
            midColumn = columnSize / 2;

        }
        GameManager.instance.NumberOfPossiblePairs = rowSize * columnSize / 2;
    }


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
    public void GenerateCards()
    {
        List<Card> cardsGeneratedData = new List<Card>();

        GetDimensions();


        int Imgcount = 0;
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
                        emptyGO.GetComponent<Card>().CardSOData = GameManager.instance.EmptyData;
                        continue;
                    }
                }

                GameObject cardClone = Instantiate(card, gridLayout.transform);
                Card cardComponent = cardClone.GetComponent<Card>();
                cardsGeneratedData.Add(cardComponent);
            }
        }                 

                
                    List<CardSO>list1 = new List<CardSO>();
                    List<CardSO>list2 = new List<CardSO>();

                    for (int k = 0; k < GameManager.instance.NumberOfPossiblePairs; k++)
                    {
                        list1.Add(GameManager.instance.AllCardsData[k]);
                        list2.Add(GameManager.instance.AllCardsData[k]);
                    }

                    foreach (var item in list1)
                    {
                        Debug.Log(item.ID);
                    }

                    ShuffleList(list1);
                    ShuffleList(list2);

                    list1.AddRange(list2);


        for (int i = 0; i < list1.Count; i++)
        {
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


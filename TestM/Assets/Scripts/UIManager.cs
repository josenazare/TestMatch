using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GridLayoutGroup gridLayout;
    public RectTransform gridLayoutTransform;
    public GameObject card, EmptyGameObject;
    public int rowSize, columnSize;
    bool isEven;
    public Sprite CardBackImage;
    public List<Sprite> CardImages;
 public   TextMeshProUGUI triesTxt, matchesTxt;
    public static UIManager instance;
    int midRow;
    int midColumn;
   
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        rowSize = Mathf.Clamp(rowSize, 2, 6);
        columnSize = Mathf.Clamp(columnSize, 2, 6);

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

    public void GenerateCards()
    {
        
        
        // Get total container dimensions
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


        int Imgcount = 0;
        // Instantiate cards dynamically
        for (int i = 0; i < rowSize ; i++)
        {
            for (int j = 0; j < columnSize ; j++)
            {
                // Instantiate the card prefab
                

                if (!isEven)
                {
                    if (i == midRow && j == midColumn)
                    {
                        Instantiate(EmptyGameObject, gridLayout.transform);
                        continue;
                    }
                }

                GameObject cardClone = Instantiate(card, gridLayout.transform);

                // Get the Card component and assign the ScriptableObject
                Card cardComponent = cardClone.GetComponent<Card>();

                if (cardComponent != null)
                {
                    if (Imgcount < GameManager.instance.NumberOfPossiblePairs)
                    {
                        cardComponent.CardSOData = GameManager.instance.AllCardsData[Imgcount];
                       
                    }
                    else
                    {
                        Imgcount = 0;
                        cardComponent.CardSOData = GameManager.instance.AllCardsData[Imgcount];
                    }
                    Imgcount++;
                }
            }

        }
        StartCoroutine(GameManager.instance.InitialFlipCards());
    }
}


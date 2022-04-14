using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_Graph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform labelCircle;
    
    private static List<int> value_x = new List<int>();
    private static List<int> value_y = new List<int>();

    public void GraphCall()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        labelCircle = graphContainer.Find("labelCircle").GetComponent<RectTransform>();

        ShowGraph(value_x, value_y);
    }

    public void GetValues(List<int> data_x, List<int> data_y)
    {
        for(int i = 0; i < data_x.Count; i++)
        {
            value_x.Add(data_x[i]);
            value_y.Add(data_y[i]);
        }
    }

    private void CreateCircle(Vector2 anchoredPosition, int x, int y)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        
        RectTransform labelC = Instantiate(labelCircle);
        labelC.SetParent(graphContainer);
        labelC.gameObject.SetActive(true);
        labelC.anchoredPosition = anchoredPosition + new Vector2(0, 20f);
        labelC.GetComponent<Text>().text = "("+ x + ", " + y + ")";
    }

    private void ShowGraph(List<int> value_x, List<int> value_y)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMaximum = RegresiMain.max_y;
        float xMaximum = RegresiMain.max_x;
        float xSize = 10f; // 10f
        for (int i = 0; i < value_x.Count; i++)
        {
            float xPosition = xSize + value_x[i] * xSize;
            float yPosition = (value_y[i] / yMaximum) * graphHeight;
            CreateCircle(new Vector2(xPosition, yPosition), value_x[i], value_y[i]);
        }

        int separatorY = 10;
        int separatorX = 10;
        for (int i = 1; i < separatorY; i++)
        {
            float normalizedValueY = i * 1f / separatorY;
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            labelY.anchoredPosition = new Vector2(-20f, normalizedValueY * graphHeight);
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValueY * yMaximum).ToString();
        }

        for (int i = 1; i < separatorX; i++)
        {
            float normalizedValueX = i * 1f / separatorX;
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(normalizedValueX * graphWidth, -20f);
            labelX.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValueX * xMaximum).ToString();
        }
    }
}

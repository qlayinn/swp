using UnityEngine;
using UnityEngine.UI;

public class SetBackgroundImage : MonoBehaviour
{
    public Sprite backgroundImage;

    void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject bgImageObject = new GameObject("BackgroundImage");
        bgImageObject.transform.SetParent(canvas.transform, false);

        Image image = bgImageObject.AddComponent<Image>();
        image.sprite = backgroundImage;

        RectTransform rectTransform = bgImageObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);
    }
}

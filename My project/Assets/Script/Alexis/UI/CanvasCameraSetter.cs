using UnityEngine;
using UnityEngine.UI;

public class CanvasCameraSetter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.sortingLayerName = "UI";
        //Canvas score = this.transform.Find("ScoreCanvas").GetComponent<Canvas>();
        //score.sortingLayerName = "UI";
    }
}

using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] GameObject scorePanel;
    [SerializeField] TMP_Text numberOfDeathsTxt;
    [SerializeField] TMP_Text numberOfTimeTxt;
    [SerializeField] TMP_Text objectCollectedTxt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowScorePanel()
    {
        scorePanel.SetActive(true);
    }
}

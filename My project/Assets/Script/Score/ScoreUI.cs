using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    // This class manage the UI for the Score
    [SerializeField] Canvas canvasScore;
    [SerializeField] GameObject scorePanel;
    [SerializeField] GameObject timerGroup;
    [SerializeField] GameObject deathsGroup;
    [SerializeField] GameObject collectedItemGroup;
    [SerializeField] TMP_Text numberOfDeathsTxt;
    [SerializeField] TMP_Text numberOfTimeTxt;
    [SerializeField] TMP_Text collectedItemTxt;
    [SerializeField] GameObject[] starsImageTimer;
    [SerializeField] GameObject[] starsImageDeaths;
    [SerializeField] GameObject[] starsImageItemCollected;


    void Start()
    {
        ScoreManager.Instance.OnShowScoreBoard += ShowScoreBoard;
    }

    // Display Score Panel
    public void ShowScorePanel()
    {
        canvasScore.enabled = true;
        ///scorePanel.SetActive(true);
    }

    // Display ScoreBoard
    private void ShowScoreBoard(TimeSpan time, int numberStarsTime, int numberDeaths, int numberStarsDeaths, int collectedItems, int numberStarsItems)
    {
        StartCoroutine(ShowScoreBoardCoroutine(time, numberStarsTime, numberDeaths, numberStarsDeaths, collectedItems, numberStarsItems));
    }

    // Mange ScoreBoard Animation and display
    private IEnumerator ShowScoreBoardCoroutine(TimeSpan time, int numberStarsTime, int numberDeaths, int numberStarsDeaths, int collectedItems, int numberStarsItems)
    {
        yield return new WaitForSecondsRealtime(1.5f);


        numberOfTimeTxt.text = time.ToString(@"mm\:ss");
        yield return StartCoroutine(ShowText(timerGroup));
        yield return StartCoroutine(ShowStars(numberStarsTime, starsImageTimer));

        numberOfDeathsTxt.text = numberDeaths.ToString();
        yield return StartCoroutine(ShowText(deathsGroup));
        yield return (StartCoroutine(ShowStars(numberStarsDeaths, starsImageDeaths)));

        collectedItemTxt.text = collectedItems.ToString();
        yield return StartCoroutine(ShowText(collectedItemGroup));
        yield return (StartCoroutine(ShowStars(numberStarsItems, starsImageItemCollected)));
    }

    // Show the right amount of stars
    private IEnumerator ShowStars(int numberStars, GameObject[] starsUI)
    {
        if(numberStars > 3)
        {
            numberStars = 3;
        }

        for(int i = 0; i < numberStars; i++)
        {
            //starsUI[i].gameObject.SetActive(true);
            starsUI[i].GetComponent<Animator>().SetBool("ShowStar", true);
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    // Manage animation text
    private IEnumerator ShowText(GameObject text)
    {
        text.GetComponent<Animator>().SetBool("ShowText", true);
        yield return new WaitForSecondsRealtime(0.2f);
    }

    private void OnDisable()
    {
        ScoreManager.Instance.OnShowScoreBoard -= ShowScoreBoard;
    }
}

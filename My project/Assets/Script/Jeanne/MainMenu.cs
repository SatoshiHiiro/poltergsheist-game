using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    int buttonNb;
    Image[] buttons;
    TextMeshProUGUI[] texts;
    Vector4 transButtonColor;
    Vector4 iniTextColor;
    Vector4 transTextColor;
    Coroutine sequence;

    private void Awake()
    {
        buttons = this.GetComponentsInChildren<Image>();
        texts = this.GetComponentsInChildren<TextMeshProUGUI>();
        transButtonColor = Vector4.one;
        transButtonColor.w = 0;
        iniTextColor = buttons[0].GetComponentInChildren<TextMeshProUGUI>().color;
        transTextColor = iniTextColor;
        transTextColor.w = 0;
    }

    private void OnEnable()
    {
        buttonNb = 0;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].color = transButtonColor;
            texts[i].color = transTextColor;
        }
        sequence = null;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        for (int i = 0; i < buttons.Length; i++)
        {
            texts[i].color = iniTextColor;
        }
    }

    IEnumerator Sequence()
    {
        float iniDelay = .5f;
        yield return new WaitForSecondsRealtime(iniDelay);

        float delay = .5f;
        for (int i = 0; i < buttons.Length; i++)
        {
            yield return new WaitForSecondsRealtime(delay);
            StartCoroutine(ButtonApparition(buttonNb));
            buttonNb++;
        }

        yield return null;
    }

    IEnumerator ButtonApparition(int index)
    {
        float animTime = 1f;
        float startTime = Time.time;
        float endTime = startTime + animTime;
        float step = Vector4.Distance(transButtonColor, Vector4.one) / animTime;

        while (Time.time <= endTime)
        {
            float stepTime = step * Time.deltaTime;
            buttons[index].color = Vector4.MoveTowards(buttons[index].color, Vector4.one, stepTime);
            texts[index].color = Vector4.MoveTowards(texts[index].color, iniTextColor, stepTime);
            yield return null;
        }
    }

    private void Update()
    {
        if (sequence == null) { sequence = StartCoroutine(Sequence()); }
    }

    public void Jouer()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void Controls()
    {
        SceneManager.LoadScene("Controls");
    }

    public void Histoire()
    {
        SceneManager.LoadScene("Histoire");
    }

    public void AudioSettings()
    {
        SceneManager.LoadScene("AudioSettings");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Retour()
    {
        SceneManager.LoadScene("UI_Accueil");
    }

    //Load le niveau quon clique
    public void OuvreNiveaux(int niveauId)
    {
        string nomNiveau = "Niveau" + niveauId;
        SceneManager.LoadScene(nomNiveau);
    }
}

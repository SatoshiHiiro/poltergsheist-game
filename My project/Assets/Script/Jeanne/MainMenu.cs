using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button[] boutons;

    //Gere les niveaux cliquables
    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < boutons.Length; i++)
        {
            boutons[i].interactable = false;
        }
        for (int i = 0; i < unlockedLevel; i++)
        {
            boutons[i].interactable = true;
        }

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

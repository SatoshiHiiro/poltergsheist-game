using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button[] boutons;

    //Gere les niveaux cliquables
    private void Awake()
    {
        //Niveau 1 debloquer quand on commence le jeu
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

    public void ProchainNiveau()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); // Get the last unlocked level
        int nextSceneIndex = Mathf.Min(unlockedLevel, SceneManager.sceneCountInBuildSettings - 1); // Ensure it's in range

        Debug.Log("Current Unlocked Level: " + unlockedLevel);
        Debug.Log("Loading next available scene: " + nextSceneIndex);

        if (nextSceneIndex > SceneManager.GetActiveScene().buildIndex) // Ensure it's ahead of the current level
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("!!!");
        }
    }
}

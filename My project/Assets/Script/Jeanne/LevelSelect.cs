using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : BaseSceneManager
{
    public Button[] boutons;
    public GameObject levelButtons;

    private void Awake()
    {
        ButtonsToArray();

        //Gere les niveaux cliquables
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

    //Load le niveau quon clique
    public void OuvreNiveaux(int niveauId)
    {
        string nomNiveau = "Niveau" + niveauId;
        SceneManager.LoadScene(nomNiveau);
    }

    void ButtonsToArray()
    {
        int childCount = levelButtons.transform.childCount;
        boutons = new Button[childCount];
        for (int i = 0; i < childCount; i++)
        {
            boutons[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }
}

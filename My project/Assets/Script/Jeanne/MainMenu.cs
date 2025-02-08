using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void Jouer()
    {
        // SceneManager.LoadScene("");
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
}

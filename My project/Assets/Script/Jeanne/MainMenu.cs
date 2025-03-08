using UnityEngine.SceneManagement;

public class MainMenu : BaseSceneManager
{
    public void Controls()
    {
        SceneManager.LoadScene("Controls");
    }

    public void Histoire()
    {
        SceneManager.LoadScene("Histoire");
    }
}

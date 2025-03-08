using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : BaseSceneManager
{
    public void Restart()
    {
        int sceneID = PlayerPrefs.GetInt("LastScene", 1);
        SceneManager.LoadScene("Niveau" + sceneID);
    }
}

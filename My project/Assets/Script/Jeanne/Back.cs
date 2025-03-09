using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Back : MonoBehaviour
{
    public void Retour()
    {
        SceneManager.LoadScene("UI_Accueil");
    }
}

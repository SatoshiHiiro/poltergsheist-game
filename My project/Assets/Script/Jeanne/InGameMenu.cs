using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject optionsMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // active ou desactive le overlay quand on clique sur escape
        if (Input.GetKeyDown(KeyCode.Escape)) 
        { 
            optionsMenu.gameObject.SetActive(!optionsMenu.gameObject.activeSelf);

            // met en pause le jeu quand le overlay est actif 
            Time.timeScale = optionsMenu.activeSelf ? 0f : 1f;
            if (optionsMenu.activeSelf)
            {
                AkUnitySoundEngine.Suspend();
            }
            else
            {
                AkUnitySoundEngine.WakeupFromSuspend();
            }
        }
    }
}

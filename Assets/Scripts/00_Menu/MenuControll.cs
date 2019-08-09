using UnityEngine;
using UnityEngine.UI;

public class MenuControll : MonoBehaviour
{
    public static bool isPaused = false;
    public static bool isDebugShow = false;

    public GameObject debug_info_menu;
    public GameObject pause_menu;
    // public GameObject popup_menu;

    void Start()
    {
        pause_menu.SetActive(isPaused);
        debug_info_menu.SetActive(isDebugShow);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseMenu();

        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                QuitToMainMenu();
        }
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pause_menu.SetActive(isPaused);

        // Show Debug info menu with the pause menu
        if (isPaused)
            debug_info_menu.SetActive(true);
        else
            debug_info_menu.SetActive(isDebugShow);
    }

    public void QuitToMainMenu()
    {
        StartCoroutine(LoadingManager.AsyncLoad("00_Menu"));
        TogglePauseMenu();
    }

    public void ToggleDebugInfoMenu()
    {
        isDebugShow = !isDebugShow;
    }

    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

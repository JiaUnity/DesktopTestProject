using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SingletonControl : MonoBehaviour
{
    public static SingletonControl singleton;

    public EventSystem m_eventSystem;

    void Awake()
    {
        // Make sure the menu is singleton
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Make sure each scene has only one event system
        EventSystem[] allES = FindObjectsOfType(typeof(EventSystem)) as EventSystem[];
        foreach (EventSystem es in allES)
        {
            if (es != m_eventSystem)
                Destroy(es.gameObject);
        }
    }
}

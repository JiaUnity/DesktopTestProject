using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSelect : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform contentContainer;

    private SceneList m_sceneList;

    void Start()
    {
        m_sceneList = Resources.Load<SceneList>("SceneList");

// In Editor, make sure the menu list is generated before trying to access it.
#if UNITY_EDITOR
        StartCoroutine("AsyncGenerateMenuItems");
#else
        GenerateMenuItems();
#endif
    }

#if UNITY_EDITOR
    IEnumerator AsyncGenerateMenuItems()
    {
        // Make sure the SceneList.asset exists
        while (m_sceneList == null)
        {
            m_sceneList = Resources.Load<SceneList>("SceneList");
            yield return new WaitForEndOfFrame();
        }

        // Make sure the list is up to date
        while (!m_sceneList.isUpdated)
            yield return new WaitForEndOfFrame();

        GenerateMenuItems();
    }

#endif

    private void GenerateMenuItems()
    {
        int i = 1;
        foreach (string name in m_sceneList.sceneNames)
        {
            // The Preload & Menu scene should not be listed
            if (name != SceneManager.GetActiveScene().name)
            {
                GameObject newButtonObject = Instantiate(buttonPrefab, new Vector3(0f, -i, 10), Quaternion.identity, contentContainer) as GameObject;
                newButtonObject.GetComponentInChildren<Text>().text = name.Substring(3);

                Button newButton = newButtonObject.GetComponent<Button>();
                newButton.onClick.AddListener(() => { ChangeScene(name); });

                i++;
            }
        }
        Resources.UnloadAsset(m_sceneList);
    }

    public void ChangeScene(string targetScene)
    {
        if (LoadingManager.asyncLoading != null && (!LoadingManager.asyncLoading.isDone || MenuControll.isPaused))
            return;

        StartCoroutine(LoadingManager.AsyncLoad(targetScene));
    }
}

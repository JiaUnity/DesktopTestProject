using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static AsyncOperation asyncLoading;

    public GameObject m_loadingObject;

    private static Text m_loadingPercentText;
    private static Material m_loadingBarMaterial;

    void Start()
    {
        m_loadingPercentText = m_loadingObject.transform.Find("Loading Text").GetComponent<Text>();
        m_loadingBarMaterial = m_loadingObject.transform.Find("Loading Bar").GetComponent<Image>().material;
    }

    void Update()
    {
        if (asyncLoading != null && m_loadingObject.activeSelf == asyncLoading.isDone)
            m_loadingObject.SetActive(!asyncLoading.isDone);
    }

    public static IEnumerator AsyncLoad(string levelName)
    {
        asyncLoading = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);

        while (!asyncLoading.isDone)
        {
            UpdateProgress();
            yield return null;
        }
        yield return new WaitForSeconds(1f);
    }

    private static void UpdateProgress()
    {
        m_loadingBarMaterial.SetFloat("_Cutoff", (1f - asyncLoading.progress) * 0.95f);
        m_loadingPercentText.text = (asyncLoading.progress * 100f).ToString("F0") + "%";
    }
}

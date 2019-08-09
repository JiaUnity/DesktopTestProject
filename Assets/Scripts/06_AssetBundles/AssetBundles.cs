using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class AssetBundles : MonoBehaviour
{
    [Header("Changing Objects")]
    public TextMesh m_textObject;
    public Renderer m_textureObject;
    public AudioSource m_audioObject;

    [Header("UI Objects")]
    public GameObject m_mainMenu;
    public GameObject m_resetMenu;
    public GameObject m_progressBar;

    private Transform m_loadingBar;
    private TextMesh m_loadingText;

    private string m_defaultText;
    private Texture m_defaultTexture;

    private AssetBundle m_assetBundle;
    private bool is_loaded = false;
    private bool is_loading = false;

    [Header("Load via Local Path")]
    private string m_bundleName;
    private string m_localPath;

    [Header("Load via URL")]
    private string m_url;

#if UNITY_2018_3_OR_NEWER
    private UnityWebRequest m_webRequest;
#else
    private WWW m_www;
#endif


    void Start()
    {
        m_defaultText = m_textObject.text;
        m_defaultTexture = m_textureObject.material.mainTexture;

        m_bundleName = "bundle4test";

#if UNITY_EDITOR || UNITY_STANDALONE
        m_localPath = Application.streamingAssetsPath + "/AssetBundles/Standalone/";
        m_url = "https://oc.unity3d.com/index.php/s/zQetrIunhA6sAoz/download";
#elif UNITY_WSA
        m_localPath = Application.streamingAssetsPath + "/AssetBundles/UWP/";
        m_url = "https://oc.unity3d.com/index.php/s/1HYxDqjFewONBnq/download";
#endif
        m_localPath = Path.Combine(m_localPath, m_bundleName);

        m_loadingBar = m_progressBar.transform.Find("Loading Bar");
        m_loadingText = m_progressBar.GetComponentInChildren<TextMesh>();

        m_loadingBar.localScale = Vector3.zero;
        m_progressBar.SetActive(false);
        m_resetMenu.SetActive(false);
        m_mainMenu.SetActive(true);
    }

    void Update()
    {
        if (is_loading)
        {
#if UNITY_2018_3_OR_NEWER
            m_loadingText.text = "Loading.. " + (m_webRequest.downloadProgress * 100f).ToString("F0") + "%";
            Vector3 progress = new Vector3(m_webRequest.downloadProgress, 1, 1);
#else
            m_loadingText.text = "Loading.. " + (m_www.progress * 100f).ToString("F0") + "%";
            Vector3 progress = new Vector3(m_www.progress, 1, 1);
#endif
            m_loadingBar.localScale = Vector3.Lerp(m_loadingBar.localScale, progress, Time.deltaTime * 5f);
        }
        // To disable input during loading.
        else
        {
            if (is_loaded)
            {
                if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
                    Reset();
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.Alpha1))
                    StartCoroutine(GetLocalBundle());
                else if (Input.GetKeyUp(KeyCode.Alpha2))
                    StartCoroutine(GetOnlineBundle());
                else if (Input.GetKeyUp(KeyCode.Alpha3))
                    StartCoroutine(GetOnlineBundleInBytes());
            }
        }
    }

    public IEnumerator GetLocalBundle()
    {
        is_loaded = true;
        m_mainMenu.SetActive(false);

        m_assetBundle = AssetBundle.LoadFromFile(m_localPath);
        yield return m_assetBundle;

        StartCoroutine("RetrieveAssets");
    }

    public IEnumerator GetOnlineBundle()
    {
        is_loaded = true;
        is_loading = true;
        m_mainMenu.SetActive(false);
        m_progressBar.SetActive(true);

#if UNITY_2018_3_OR_NEWER
        using (m_webRequest = UnityWebRequestAssetBundle.GetAssetBundle(m_url))
        {
            yield return m_webRequest.SendWebRequest();

            is_loading = false;
            if (m_webRequest.isNetworkError || m_webRequest.isHttpError)
            {
                Debug.LogError(m_webRequest.error);
                Reset();
                yield break;
            }
            else
                m_assetBundle = DownloadHandlerAssetBundle.GetContent(m_webRequest);
        }

#else
        using (m_www = new WWW(m_url))
        {
            yield return m_www;

            is_loading = false;
            if (!string.IsNullOrEmpty(m_www.error))
            {
                Debug.LogError(m_www.error);
                Reset();
                yield break;
            }
            else
                m_assetBundle = m_www.assetBundle;
        }

#endif
        StartCoroutine("RetrieveAssets");
    }

    public IEnumerator GetOnlineBundleInBytes()
    {
        is_loaded = true;
        is_loading = true;

        m_mainMenu.SetActive(false);
        m_progressBar.SetActive(true);

        byte[] bundleData = null;

#if UNITY_2018_3_OR_NEWER
        using (m_webRequest = UnityWebRequest.Get(m_url))
        {
            yield return m_webRequest.SendWebRequest();

            is_loading = false;
            if (m_webRequest.isNetworkError || m_webRequest.isHttpError)
            {
                Debug.LogError(m_webRequest.error);
                Reset();
                yield break;
            }
            else
                bundleData = m_webRequest.downloadHandler.data;
        }

#else
        using (m_www = new WWW(m_url))
        {
            yield return m_www;

            is_loading = false;
            if (!string.IsNullOrEmpty(m_www.error))
            {
                Debug.LogError(m_www.error);
                Reset();
                yield break;
            }
            else
                bundleData = m_www.bytes;
        }
#endif
        AssetBundleCreateRequest acr = AssetBundle.LoadFromMemoryAsync(bundleData);
        yield return acr;

        m_assetBundle = acr.assetBundle;
        StartCoroutine("RetrieveAssets");
    }

    IEnumerator RetrieveAssets()
    {
        m_progressBar.SetActive(false);

        TextAsset retrievedText = m_assetBundle.LoadAsset("TextDoc", typeof(TextAsset)) as TextAsset;
        yield return retrievedText;
        m_textObject.text = retrievedText.text;


        Texture2D retrievedTexture = m_assetBundle.LoadAsset("Texture", typeof(Texture2D)) as Texture2D;
        yield return retrievedTexture;
        m_textureObject.material.mainTexture = retrievedTexture;


        AudioClip retrievedAudio = m_assetBundle.LoadAsset("AudioSample", typeof(AudioClip)) as AudioClip;
        yield return retrievedAudio;
        m_audioObject.clip = retrievedAudio;
        m_audioObject.Play();

        m_resetMenu.SetActive(true);
        m_assetBundle.Unload(false);
    }

    private void Reset()
    {
        m_textObject.text = m_defaultText;
        m_textureObject.material.mainTexture = m_defaultTexture;
        m_audioObject.Stop();

        m_loadingText.text = string.Empty;
        m_loadingBar.localScale = Vector3.zero;

        m_resetMenu.SetActive(false);
        m_mainMenu.SetActive(true);

        is_loaded = false;
    }
}

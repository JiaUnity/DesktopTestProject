using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class AssetBundles : MonoBehaviour
{
    [Header("Changing Objects")]
    public TextMesh m_textObject;
    public Renderer m_textureObject;
    public AudioSource m_audioObject;
    public GameObject m_modelObject;

    [Header("UI Objects")]
    public GameObject m_mainMenu;
    public GameObject m_resetMenu;
    public Slider m_progressBar;

    private Text m_loadingText;

    private AssetBundle m_assetBundle;
    private bool is_loaded = false;
    private bool is_loading = false;

    [Header("Load via Local Path")]
    public string m_localPath = "bundle4test";

    [Header("Load via URL")]
    private string m_url;
#if UNITY_2018_3_OR_NEWER
    private UnityWebRequest m_webRequest;
#else
    private WWW m_www;
#endif


    void Start()
    {
        //m_defaultText = m_textObject.text;
        //m_defaultTexture = m_textureObject.material.mainTexture;

#if UNITY_EDITOR || UNITY_STANDALONE
        m_localPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/Standalone/", m_localPath);
        m_url = "https://oc.unity3d.com/index.php/s/p7MR0czK5ScIIlh/download";
#elif UNITY_WSA
        m_localPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/UWP/", m_localPath);
        m_url = "https://oc.unity3d.com/index.php/s/TN7FUbL4kjqbnmQ/download";
#endif
        //m_localPath = Path.Combine(Application.streamingAssetsPath, m_localPath);

        m_loadingText = m_progressBar.GetComponentInChildren<Text>();

        m_progressBar.gameObject.SetActive(false);
        m_resetMenu.SetActive(false);
        m_mainMenu.SetActive(true);
    }

    void Update()
    {
        if (is_loading)
        {
#if UNITY_2018_3_OR_NEWER
            m_loadingText.text = "Loading.. " + (m_webRequest.downloadProgress * 100f).ToString("F0") + "%";
            m_progressBar.value = m_webRequest.downloadProgress;
#else
            m_loadingText.text = "Loading.. " + (m_www.progress * 100f).ToString("F0") + "%";
            m_progressBar.value = m_www.progress;
#endif
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

    private IEnumerator GetLocalBundle()
    {
        is_loaded = true;
        m_mainMenu.SetActive(false);

        m_assetBundle = AssetBundle.LoadFromFile(m_localPath);
        yield return m_assetBundle;

        StartCoroutine("RetrieveAssets");
    }

    private IEnumerator GetOnlineBundle()
    {
        is_loaded = true;
        is_loading = true;
        m_mainMenu.SetActive(false);
        m_progressBar.gameObject.SetActive(true);

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

    private IEnumerator GetOnlineBundleInBytes()
    {
        is_loaded = true;
        is_loading = true;

        m_mainMenu.SetActive(false);
        m_progressBar.gameObject.SetActive(true);

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
        m_resetMenu.SetActive(true);
        m_progressBar.gameObject.SetActive(false);

        TextAsset retrievedText = m_assetBundle.LoadAsset("TextDoc", typeof(TextAsset)) as TextAsset;
        Font retrievedFont = m_assetBundle.LoadAsset("coolvetica", typeof(Font)) as Font;
        yield return retrievedText;
        yield return retrievedFont;
        m_textObject.text = retrievedText.text;
        m_textObject.GetComponent<MeshRenderer>().material = retrievedFont.material;
        m_textObject.font = retrievedFont;

        Texture2D retrievedTexture = m_assetBundle.LoadAsset("Texture", typeof(Texture2D)) as Texture2D;
        yield return retrievedTexture;
        m_textureObject.material.mainTexture = retrievedTexture;

        AudioClip retrievedAudio = m_assetBundle.LoadAsset("AudioSample", typeof(AudioClip)) as AudioClip;
        yield return retrievedAudio;
        m_audioObject.clip = retrievedAudio;
        m_audioObject.Play();

        Mesh retrievedModel = m_assetBundle.LoadAsset("ModelSample", typeof(Mesh)) as Mesh;
        Material retrievedMaterial = m_assetBundle.LoadAsset("MaterialSample", typeof(Material)) as Material;
        Shader retrievedShader = m_assetBundle.LoadAsset("Additive", typeof(Shader)) as Shader;
        yield return retrievedModel;
        yield return retrievedMaterial;
        yield return retrievedShader;
        retrievedMaterial.shader = retrievedShader;
        m_modelObject.GetComponent<MeshRenderer>().material = retrievedMaterial;
        m_modelObject.GetComponent<MeshFilter>().mesh = retrievedModel;

        GameObject retrievedPrefab = m_assetBundle.LoadAsset("Spinning Cube", typeof(GameObject)) as GameObject;
        yield return retrievedPrefab;
        Instantiate(retrievedPrefab);

        m_assetBundle.Unload(false);
    }

    public void Reset()
    {
        //m_textObject.text = m_defaultText;
        //m_textureObject.material.mainTexture = m_defaultTexture;
        //m_audioObject.Stop();

        //m_loadingText.text = string.Empty;
        //m_loadingBar.localScale = Vector3.zero;

        //m_resetMenu.SetActive(false);
        //m_mainMenu.SetActive(true);

        //is_loaded = false;

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}

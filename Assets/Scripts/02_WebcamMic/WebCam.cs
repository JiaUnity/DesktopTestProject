using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WebCam : MonoBehaviour
{
    private WebCamDevice[] webcam_devices;
    private bool is_cam_available;
    private Texture default_background;

    private RawImage webcam_canvas;
    private AspectRatioFitter fitter;
    public GameObject no_camera_warning;

    // Use this for initialization
    void Start()
    {
        webcam_canvas = GetComponent<RawImage>();
        fitter = GetComponent<AspectRatioFitter>();

        default_background = webcam_canvas.texture;

        if (CheckAvailability())
            StartCoroutine("InitializeCamera", 0);
        else
            NoWebCamAvailable();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle camera connect/disconnect
        bool availability = CheckAvailability();
        if (availability != is_cam_available)
        {
            if (availability)
                StartCoroutine("InitializeCamera", 0);
            else
                NoWebCamAvailable();
        }
    }

    private bool CheckAvailability()
    {
        webcam_devices = WebCamTexture.devices;
        if (webcam_devices.Length == 0)
            return false;
        else
            return true;
    }

    // initialize the ith device in the webcam_devices
    IEnumerator InitializeCamera(int i)
    {
        is_cam_available = true;
        webcam_canvas.color = Color.white;
        no_camera_warning.SetActive(false);

        WebCamTexture webcam_texture = new WebCamTexture(webcam_devices[i].name, Screen.width, Screen.height, 30);
        webcam_texture.Play();
        webcam_canvas.texture = webcam_texture;

        // Make sure the webcam is done intializing before change aspect ratio
        yield return new WaitUntil(() => webcam_texture.didUpdateThisFrame);

        // Adjuct webcam canvas to fit the webcam's aspect ratio.
        fitter.aspectRatio = (float)webcam_texture.width / webcam_texture.height;
        float scaleY = webcam_texture.videoVerticallyMirrored ? -1f : 1f;
        webcam_canvas.rectTransform.localScale = new Vector3(-1f, scaleY, 1f); // x is -1 because of front cameara
    }

    private void NoWebCamAvailable()
    {
        is_cam_available = false;
        no_camera_warning.SetActive(true);
        webcam_canvas.color = Color.grey;
        webcam_canvas.texture = default_background;
    }
}

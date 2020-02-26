using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class WebCam : MonoBehaviour
{
    public Text m_warning;
    public Button m_takePhotoButton;
    public Button m_recordVideoButton;

    private bool m_IsCamAvailable = false;
    private Texture m_defaultBG;
    private AspectRatioFitter fitter;

    private WebCamTexture m_webCamTexture = null;
    private RawImage webcam_canvas;

    private PhotoCapture m_photoCaptureObject = null;
    private VideoCapture m_videoCaptureObject = null;
    private float m_recordingTimer = float.MaxValue;
    static readonly float MaxRecordingTime = 5f;

    // Use this for initialization
    void Start()
    {
        webcam_canvas = GetComponent<RawImage>();
        fitter = GetComponent<AspectRatioFitter>();

        m_defaultBG = webcam_canvas.texture;
        UpdateWebCam();

#if !UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN && !UNITY_WSA
        m_takePhotoButton.gameObject.SetActive(false);
        m_recordVideoButton.gameObject.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        // Handle camera connect/disconnect
        UpdateWebCam();

        // Handle Video Capture timer
        if (m_videoCaptureObject != null && m_videoCaptureObject.IsRecording && Time.time > m_recordingTimer)
            StopVideoCaptureRecording();
    }

    private void OnDisable()
    {
        StopWebCamTexture();
    }

    private bool CheckWebCamAvailability()
    {
        if (WebCamTexture.devices.Length == 0)
            return false;
        else
            return true;
    }

    private void UpdateWebCam()
    {
        bool availability = CheckWebCamAvailability();
        if (availability != m_IsCamAvailable)
        {
            if (availability)
                StartCoroutine(StartWebCamTexture(0));
            else
            {
                m_IsCamAvailable = false;
                ShowWarning("No Webcam is available.");
            }
        }
    }

    // initialize the ith device in the webcam_devices
    IEnumerator StartWebCamTexture(int i)
    {
        m_IsCamAvailable = true;
        webcam_canvas.color = Color.white;
        m_warning.gameObject.SetActive(false);

        if (m_webCamTexture == null)
            m_webCamTexture = new WebCamTexture(WebCamTexture.devices[i].name, Screen.width, Screen.height, 30);
        m_webCamTexture.Play();
        webcam_canvas.texture = m_webCamTexture;

        // Make sure the webcam is done intializing before change aspect ratio
        yield return new WaitUntil(() => m_webCamTexture.didUpdateThisFrame);

        // Adjuct webcam canvas to fit the webcam's aspect ratio.
        fitter.aspectRatio = (float)m_webCamTexture.width / m_webCamTexture.height;
        float scaleY = m_webCamTexture.videoVerticallyMirrored ? -1f : 1f;
        webcam_canvas.rectTransform.localScale = new Vector3(-1f, scaleY, 1f); // x is -1 because of front cameara
    }

    private void StopWebCamTexture()
    {
        if (m_IsCamAvailable && m_webCamTexture.isPlaying)
            m_webCamTexture.Stop();
    }

    // From example: https://docs.unity3d.com/ScriptReference/Windows.WebCam.PhotoCapture.TakePhotoAsync.html
    public void OnTakePhoto()
    {
        if (!m_IsCamAvailable || m_videoCaptureObject != null) return;

        Resolution cameraRes = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        PhotoCapture.CreateAsync(false, delegate(PhotoCapture captureObject) {
            m_photoCaptureObject = captureObject;
            CameraParameters c = SetupCameraParameters(cameraRes);

            m_photoCaptureObject.StartPhotoModeAsync(c, delegate(PhotoCapture.PhotoCaptureResult r0) {
                StopWebCamTexture();
                ShowWarning("Taking photo with webcam...");

                // Take two photos. One in JPG format, the other in PNG.
                string fileName = "CapturedImage";
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName + ".jpg");

                m_photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, delegate(PhotoCapture.PhotoCaptureResult r1) {
                    filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName + ".png");

                    m_photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.PNG, delegate(PhotoCapture.PhotoCaptureResult r2) {
                        m_photoCaptureObject.StopPhotoModeAsync(delegate(PhotoCapture.PhotoCaptureResult r3) {
                            m_photoCaptureObject.Dispose();
                            m_photoCaptureObject = null;

                            StartCoroutine(StartWebCamTexture(0));
                            System.Diagnostics.Process.Start("explorer.exe", Application.persistentDataPath.Replace("/", "\\"));
                        });
                    });
                });
            });
        });
    }

    // From example: https://docs.unity3d.com/ScriptReference/Windows.WebCam.VideoCapture.html
    public void ToggleVideoRecord()
    {
        if (!m_IsCamAvailable || m_photoCaptureObject != null) return;

        // Start recording, or stop if is currently doing so
        if (m_videoCaptureObject == null)
        {
            Resolution camRes = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            float camFramerate = VideoCapture.GetSupportedFrameRatesForResolution(camRes).OrderByDescending((fps) => fps).First();

            VideoCapture.CreateAsync(false, delegate(VideoCapture videoCapture)
            {
                if (videoCapture == null)
                    Debug.LogWarning("Failed to create VideoCapture instance.");
                else
                {
                    m_videoCaptureObject = videoCapture;
                    CameraParameters c = SetupCameraParameters(camRes, camFramerate);

                    m_videoCaptureObject.StartVideoModeAsync(c, VideoCapture.AudioState.MicAudio, delegate(VideoCapture.VideoCaptureResult r0)
                    {
                        StopWebCamTexture();
                        ShowWarning("Recording video with webcam...");

                        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "RecordedVideo.mp4");
                        m_videoCaptureObject.StartRecordingAsync(filePath, delegate(VideoCapture.VideoCaptureResult r1)
                        {
                            m_recordingTimer = Time.time + MaxRecordingTime;
                        });
                    });
                }
            });
        }
        else if (m_videoCaptureObject.IsRecording)
            StopVideoCaptureRecording();
    }

    private void StopVideoCaptureRecording()
    {
        m_recordingTimer = float.MaxValue;
        m_videoCaptureObject.StopRecordingAsync(delegate(VideoCapture.VideoCaptureResult r0) {
            m_videoCaptureObject.StopVideoModeAsync(delegate(VideoCapture.VideoCaptureResult r1) {
                m_videoCaptureObject.Dispose();
                m_videoCaptureObject = null;

                StartCoroutine(StartWebCamTexture(0));
                System.Diagnostics.Process.Start("explorer.exe", Application.persistentDataPath.Replace("/", "\\"));
            });
        });
    }

    private CameraParameters SetupCameraParameters(Resolution res, float frameRate = 0f)
    {
        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0f;
        c.cameraResolutionWidth = res.width;
        c.cameraResolutionHeight = res.height;
        c.frameRate = frameRate;
        c.pixelFormat = CapturePixelFormat.BGRA32;
        return c;
    }

    private void ShowWarning(string msg)
    {
        m_warning.text = msg;
        m_warning.gameObject.SetActive(true);
        webcam_canvas.color = Color.grey;
        webcam_canvas.texture = m_defaultBG;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AspectRatioFitter))]
public class VideoControl : MonoBehaviour
{
    private RawImage m_videoCanvas;
    private VideoPlayer m_videoPlayer;
    private Slider m_timelineSlider;

    private Text m_debugText;
    private Text m_errorText;

    private bool isDraggingTimeline = false;
    private bool isPreviousPlaying = false;

    [Header("Video Controls")]
    public Image m_playPauseControl;
    public Dropdown m_formatPicker;
    public Text m_currentTimeText;
    public Text m_totalTimeText;

    [Header("Other Assets")]
    public Sprite m_playIcon;
    public Sprite m_pauseIcon;
    public VideoClip m_mp4Video;
    public VideoClip m_webmVideo;
    public VideoClip m_ogv;
    public VideoClip m_vfr;

    void Start()
    {
        m_videoCanvas = GetComponent<RawImage>();
        m_videoPlayer = GetComponent<VideoPlayer>();

        m_debugText = transform.Find("Debug Message").GetComponent<Text>();
        m_errorText = transform.Find("Error Message").GetComponent<Text>();
        m_timelineSlider = transform.Find("Timeline").GetComponent<Slider>();

        m_videoPlayer.errorReceived += OnErrorMessage;
        SwitchVideoFormat();
    }

    private void Update()
    {
        if (m_videoPlayer.isPrepared)
            m_currentTimeText.text = ConvertSecondsToTime(m_videoPlayer.time);

        if (m_debugText != null)
            m_debugText.text = m_videoPlayer.clip.name + "\n"
                + "Resolution: " + m_videoPlayer.clip.width + " x " + m_videoPlayer.clip.height + "\n"
                + "Frame rate: " + m_videoPlayer.frameRate + "\n"
                + "Is Playing: " + m_videoPlayer.isPlaying.ToString() + "\n"
                + "Is Prepared: " + m_videoPlayer.isPrepared.ToString() + "\n";

        if (m_timelineSlider != null)
        {
            if (isDraggingTimeline)
                GoToTime(m_timelineSlider.normalizedValue * m_videoPlayer.clip.length);
            else
                m_timelineSlider.normalizedValue = (float)(m_videoPlayer.time / m_videoPlayer.clip.length);
        }
    }

    public void SwitchPlayPause()
    {
        if (m_videoPlayer.isPlaying)
            Pause();
        else
            StartCoroutine("Play");
    }

    public void Stop()
    {
        m_videoPlayer.Stop();
        m_playPauseControl.sprite = m_playIcon;
        m_videoCanvas.color = Color.grey;

        StartCoroutine("InitializeVideo");
    }

    public void SwitchVideoFormat()
    {
        switch (m_formatPicker.captionText.text)
        {
            case "mp4":
                LoadVideoClip(m_mp4Video);
                break;
            case "vfr":
                LoadVideoClip(m_vfr);
                break;
            case "ogv":
                LoadVideoClip(m_ogv);
                break;
            case "webm":
            default:
                LoadVideoClip(m_webmVideo);
                break;
        }
    }

    public void OnPointerDownTimeline()
    {
        isPreviousPlaying = m_videoPlayer.isPlaying;
        Pause();
        isDraggingTimeline = true;
    }

    public void OnPointerUpTimeline()
    {
        if (isDraggingTimeline)
        {
            isDraggingTimeline = false;
            if (isPreviousPlaying)
                StartCoroutine("Play");
        }
    }

    private void LoadVideoClip(VideoClip video)
    {
        m_videoPlayer.clip = video;

        AspectRatioFitter fitter = GetComponent<AspectRatioFitter>();
        fitter.aspectRatio = (float)m_videoPlayer.clip.width / m_videoPlayer.clip.height;
        m_videoPlayer.SetTargetAudioSource(0, GetComponent<AudioSource>());
        m_totalTimeText.text = "/ " + ConvertSecondsToTime(m_videoPlayer.clip.length);

        StartCoroutine("InitializeVideo");
        StartCoroutine("Play");
    }

    IEnumerator InitializeVideo()
    {
        //Debug.Log("Start Prepare");

        m_videoPlayer.Prepare();
        yield return new WaitUntil(() => m_videoPlayer.isPrepared);

        //Debug.Log("End Prepare");

        m_videoCanvas.texture = m_videoPlayer.texture;
    }

    IEnumerator Play()
    {
        if (!m_videoPlayer.isPlaying)
        {
            yield return new WaitUntil(() => m_videoPlayer.isPrepared);

            m_videoPlayer.Play();
            m_playPauseControl.sprite = m_pauseIcon;
            m_videoCanvas.color = Color.white;
        }
    }

    private void Pause()
    {
        if (m_videoPlayer.isPlaying)
        {
            m_videoPlayer.Pause();
            m_playPauseControl.sprite = m_playIcon;
            //m_videoCanvas.color = Color.grey;
        }
    }

    private void GoToTime(double seconds)
    {
        m_videoPlayer.time = seconds;
    }

    private void OnErrorMessage(VideoPlayer source, string message)
    {
        if (m_errorText != null)
            m_errorText.text += message;
    }

    private string ConvertSecondsToTime(double seconds)
    {
        float minute = Mathf.Floor((float)seconds / 60);
        int sec = Mathf.RoundToInt((float)seconds % 60);

        string result = string.Empty;
        if (minute < 10)
            result += "0";
        result += minute.ToString() + ":";

        if (sec < 10)
            result += "0";
        result += sec.ToString();
        return result;
    }
}

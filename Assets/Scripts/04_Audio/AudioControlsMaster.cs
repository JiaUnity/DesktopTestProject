using UnityEngine;
using System.Collections;

public class AudioControlsMaster : MonoBehaviour
{
    public AudioSource audioSource;

    public Renderer rewindButton;
    public Renderer fastforwardButton;
    public Renderer playButton;
    public Renderer pauseButton;
    public Renderer stopButton;
    public Renderer volumeUpButton;
    public Renderer volumeDownButton;
    public Transform volumeScale;

    public TextMesh timerText;

    private float startFlashTime;
    private string timerFormatted;
    private string totalTimeFormatted;

    void Start()
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(audioSource.clip.length);
        totalTimeFormatted = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
    }

    void Update()
    {
        if (!audioSource.isPlaying)
            playButton.material.color = Color.white;
        else
            playButton.material.color = Color.green;

        volumeScale.localScale = new Vector3(audioSource.volume, 1f, 1f);

        System.TimeSpan t = System.TimeSpan.FromSeconds(audioSource.time);
        timerFormatted = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

        timerText.text = timerFormatted + " / " + totalTimeFormatted;
    }

    public void RecieveInput(AudioControlsChild.ControlType controlType)
    {
        if (controlType == AudioControlsChild.ControlType.Play)
        {
            pauseButton.material.color = Color.white;
            audioSource.Play();
        }
        else if (controlType == AudioControlsChild.ControlType.Pause)
        {
            if (audioSource.isPlaying)
            {
                pauseButton.material.color = Color.green;
                audioSource.Pause();
            }
            else if (!audioSource.isPlaying && audioSource.time > 0f)
            {
                pauseButton.material.color = Color.white;
                audioSource.Play();
            }
        }
        else if (controlType == AudioControlsChild.ControlType.Stop)
        {
            StartCoroutine(FlashColour(0.25f, stopButton));
            pauseButton.material.color = Color.white;
            audioSource.Stop();
            audioSource.time = 0f;
        }
        else if (controlType == AudioControlsChild.ControlType.Rewind)
        {
            if (audioSource.time > 0f)
            {
                StartCoroutine(FlashColour(0.25f, rewindButton));
                audioSource.time -= 2f;
            }
        }
        else if (controlType == AudioControlsChild.ControlType.FastForward)
        {
            StartCoroutine(FlashColour(0.25f, fastforwardButton));
            audioSource.time += 2f;
        }
        else if (controlType == AudioControlsChild.ControlType.VolumeUp)
        {
            StartCoroutine(FlashColour(0.25f, volumeUpButton));
            audioSource.volume += 0.1f;
        }
        else if (controlType == AudioControlsChild.ControlType.VolumeDown)
        {
            StartCoroutine(FlashColour(0.25f, volumeDownButton));
            audioSource.volume -= 0.1f;
        }
    }

    IEnumerator FlashColour(float duration, Renderer targetObject)
    {
        startFlashTime = Time.time;

        while (Time.time < startFlashTime + duration)
        {
            targetObject.material.color = Color.green;
            yield return null;
        }

        targetObject.material.color = Color.white;
    }
}

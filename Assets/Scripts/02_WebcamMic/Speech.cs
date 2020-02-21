using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class Speech : MonoBehaviour
{
    public Image speech_icon;
    public Text speech_phrase;

    private DictationRecognizer speech_recognizer;

    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
        StartCoroutine(InitSpeechRecognizer());
#else
        OnWrongPlatform();
#endif
    }

    private void OnDisable()
    {
        if (speech_recognizer != null)
        {
            speech_recognizer.Stop();
            speech_recognizer.DictationResult -= OnSpeechResult;
            speech_recognizer.DictationError -= OnSpeechError;
            speech_recognizer.Dispose();
        }
    }

    private IEnumerator InitSpeechRecognizer()
    {
        while (!Mic.Is_Mic_Initialized)
            yield return new WaitForEndOfFrame();

        if (Mic.Is_Mic_Available)
        {
            speech_recognizer = new DictationRecognizer();
            speech_recognizer.DictationResult += (text, confidence) => OnSpeechResult(text, confidence);
            speech_recognizer.DictationError += (error, hresult) => OnSpeechError(error, hresult);
            speech_recognizer.Start();
        }
        else
        {
            speech_icon.color = Color.red;
            speech_phrase.text = "<color=red>No microphone available.</color>";
        }            
    }

    private void OnSpeechResult(string text, ConfidenceLevel confidence)
    {
        speech_icon.color = Color.green;

        switch (confidence)
        {
            case ConfidenceLevel.High:
                speech_phrase.text = "<color=#06390CFF>" + text + "</color>";
                break;
            case ConfidenceLevel.Medium:
                speech_phrase.text = "<color=blue>" + text + "</color>";
                break;
            case ConfidenceLevel.Low:
                speech_phrase.text = "<color=orange>" + text + "</color>";
                break;
            default:
                speech_phrase.text = "<color=brown>(Inaudible)</color>";
                break;
        }
    }

    private void OnSpeechError(string error, int hresult)
    {
        Debug.LogError(hresult + ": " + error);

        speech_icon.color = Color.red;
        speech_phrase.text = "<color=red>" + error + "</color>";
    }

    private void OnWrongPlatform()
    {
        speech_icon.color = Color.red;
        speech_phrase.text = "<color=brown>Not Available</color>";
    }
}

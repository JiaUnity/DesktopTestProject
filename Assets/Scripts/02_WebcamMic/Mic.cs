using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

[RequireComponent(typeof(AudioSource))]
public class Mic : MonoBehaviour
{
    public Image mic_icon;
    public Dropdown mic_select_menu;
    public GameObject no_available_warning;

    public Sprite mic_available;
    public Sprite mic_unavailable;

    private string current_mic;
    private int num_of_devices;

    private AudioSource audio_source;
    private int min_freq;
    private int max_freq;

    // Use this for initialization
    void Start()
    {
        // initialize mic
        audio_source = GetComponent<AudioSource>();
        mic_select_menu.onValueChanged.AddListener(delegate {
            SwitchToMic(mic_select_menu.captionText.text);
        });

        RefreshMicSetup();
    }

    // Update is called once per frame
    void Update()
    {
        int currentNum = Microphone.devices.Length;
        if (currentNum != num_of_devices)
            RefreshMicSetup();
    }

    private void RefreshMicSetup()
    {
        mic_select_menu.ClearOptions();
        num_of_devices = Microphone.devices.Length;

        // if no mic is available
        if (num_of_devices == 0)
        {
            mic_icon.color = Color.red;
            mic_icon.sprite = mic_unavailable;
            no_available_warning.SetActive(true);
            mic_select_menu.gameObject.SetActive(false);
        }
        else
        {
            mic_icon.color = Color.green;
            mic_icon.sprite = mic_available;
            no_available_warning.SetActive(false);
            mic_select_menu.gameObject.SetActive(true);

            mic_select_menu.AddOptions(Microphone.devices.Cast<string>().ToList());
            SwitchToMic(Microphone.devices[0]);
        }
    }

    private void SwitchToMic(string newMic)
    {
        if (current_mic == newMic)
            return;

        Microphone.End(current_mic);
        audio_source.Stop();
        current_mic = newMic;

        Microphone.GetDeviceCaps(current_mic, out min_freq, out max_freq);
        if (min_freq == 0 && max_freq == 0)
            max_freq = 44100;

        audio_source.clip = Microphone.Start(current_mic, true, 10, max_freq);
        audio_source.loop = true;

        if (Microphone.IsRecording(current_mic))
        {
            while (Microphone.GetPosition(current_mic) <= 0) ;
            Debug.Log("Recording started with " + current_mic);
            audio_source.Play();
        }
    }
}

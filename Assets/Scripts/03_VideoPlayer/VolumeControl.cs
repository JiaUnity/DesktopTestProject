using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioSource m_audioSource;

    [Header("Volumn Controls")]
    public Image m_muteControl;
    public Slider m_volumnSliderControl;

    [Header("Sprites for icon change")]
    public Sprite m_unmuteIcon;
    public Sprite m_muteIcon;

    // Use this for initialization
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        SetVolume();
    }

    public void ToggleMuteUnmute()
    {
        m_muteControl.sprite = m_audioSource.mute ? m_unmuteIcon : m_muteIcon;
        m_audioSource.mute = !m_audioSource.mute;
    }

    public void SetVolume()
    {
        if (m_volumnSliderControl != null)
            m_audioSource.volume = m_volumnSliderControl.value;
    }
}

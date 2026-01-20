using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] Slider _bgmSlider;
    void Update()
    {
        float bgmFloat = _bgmSlider.value;
        _audioMixer.SetFloat("VolumeMaster", Mathf.Log10(bgmFloat) * 20);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

//Settings Controller
public class PostProcessingController : MonoBehaviour
{
    [SerializeField] PostProcessVolume postProcessVolume;
    [SerializeField] AudioMixer Music;
    [SerializeField] AudioMixer Sound;
    [SerializeField] Toggle[] toggles;
    [SerializeField] Slider[] sliders;
    Bloom bloom;
    Grain grain;
    LensDistortion lensDistortion;
    Vignette vignette;
    ChromaticAberration chromaticAbberation;


    private void Start()
    {
        postProcessVolume.profile.TryGetSettings(out bloom);
        postProcessVolume.profile.TryGetSettings(out grain);
        postProcessVolume.profile.TryGetSettings(out lensDistortion);
        postProcessVolume.profile.TryGetSettings(out vignette);
        postProcessVolume.profile.TryGetSettings(out chromaticAbberation);

        bloomToggle(Convert.ToBoolean(PlayerPrefs.GetInt("Bloom", 1)));
        grainToggle(Convert.ToBoolean(PlayerPrefs.GetInt("Grain", 1)));
        lensToggle(Convert.ToBoolean(PlayerPrefs.GetInt("LensDistortion", 1)));
        vignetteToggle(Convert.ToBoolean(PlayerPrefs.GetInt("Vignette", 1)));
        chromaticToggle(Convert.ToBoolean(PlayerPrefs.GetInt("ChromaticAberration", 1)));

        SetMusicLevel(PlayerPrefs.GetFloat("MusicVol", 1));
        SetSoundLevel(PlayerPrefs.GetFloat("SoundVol", 1));

        sliders[0].value = PlayerPrefs.GetFloat("MusicVol", 1);
        sliders[1].value = PlayerPrefs.GetFloat("SoundVol", 1);
    }

    public void SetMusicLevel(float sliderValue)
    {
        Music.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVol", sliderValue);
    }

    public void SetSoundLevel(float sliderValue)
    {
        Sound.SetFloat("SoundVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SoundVol", sliderValue);
    }

    public void bloomToggle(bool value)
    {
        bloom.active = value;
        toggles[0].isOn = value;
        PlayerPrefs.SetInt("Bloom", Convert.ToInt32(value));
    }
    public void grainToggle(bool value)
    {
        grain.active = value;
        toggles[1].isOn = value;
        PlayerPrefs.SetInt("Grain", Convert.ToInt32(value));
    }
    public void lensToggle(bool value)
    {
        lensDistortion.active = value;
        toggles[2].isOn = value;
        PlayerPrefs.SetInt("LensDistortion", Convert.ToInt32(value));
    }
    public void vignetteToggle(bool value)
    {
        vignette.active = value;
        toggles[3].isOn = value;
        PlayerPrefs.SetInt("Vignette", Convert.ToInt32(value));
    }
    public void chromaticToggle(bool value)
    {
        chromaticAbberation.active = value;
        toggles[4].isOn = value;
        PlayerPrefs.SetInt("ChromaticAberration", Convert.ToInt32(value));
    }
}

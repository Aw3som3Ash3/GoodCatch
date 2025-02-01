using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SettingsUI : VisualElement
{
    Slider masterSound, sfx, music;
    DropdownField resolutions;
    Toggle vsync;
    Slider brightness;
    const float soundMin=-40, soundMax=20;
    public new class UxmlFactory : UxmlFactory<SettingsUI, CombatUI.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public SettingsUI()
    {
        Initial();
    }

    void Initial()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/ModSettings");

        visualTreeAsset.CloneTree(root);

        masterSound = this.Q<Slider>("MasterSoundSlider");
       
        if (GameManager.Instance != null)
        {
            float volume;
            masterSound.lowValue = soundMin;
            masterSound.highValue = soundMax;
            GameManager.Instance.audioMixer.GetFloat("Master", out volume);
            masterSound.value = volume;
            
            masterSound.RegisterValueChangedCallback((evt) => GameManager.Instance.audioMixer.SetFloat("Master", evt.newValue));

            sfx = this.Q<Slider>("SFXSlider");
            sfx.lowValue = soundMin*2;
            sfx.highValue = soundMax;
            GameManager.Instance.audioMixer.GetFloat("Effects", out volume);
            sfx.value = volume;
            sfx.RegisterValueChangedCallback((evt) => GameManager.Instance.audioMixer.SetFloat("Effects", evt.newValue));

            music = this.Q<Slider>("MusicSlider");
            music.lowValue = soundMin * 2;
            music.highValue = soundMax;
            GameManager.Instance.audioMixer.GetFloat("Music", out volume);
            music.value = volume;
            music.RegisterValueChangedCallback((evt) => GameManager.Instance.audioMixer.SetFloat("Music", evt.newValue));
        }
        

        resolutions = this.Q<DropdownField>("ScreenDropDown");
        vsync = this.Q<Toggle>("VSYNCToggle");
        brightness = this.Q<Slider>("BrightnessSlider");
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SettingsUI : VisualElement
{
    AudioMixer audioMixer;
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
        if (GameManager.Instance != null)
        {
            audioMixer=GameManager.Instance.audioMixer;
        }
        else 
        {
            MainMenu menu= GameObject.FindObjectOfType<MainMenu>();
            if (menu != null)
            {
                audioMixer = menu.mixer;
            }

        }
        masterSound = this.Q<Slider>("MasterSoundSlider");
       
        if (audioMixer != null)
        {
            float volume;
            masterSound.lowValue = soundMin;
            masterSound.highValue = soundMax;
            audioMixer.GetFloat("Master", out volume);
            masterSound.value = volume;
            
            masterSound.RegisterValueChangedCallback((evt) => audioMixer.SetFloat("Master", evt.newValue));

            sfx = this.Q<Slider>("SFXSlider");
            sfx.lowValue = soundMin*2;
            sfx.highValue = soundMax;
            audioMixer.GetFloat("Effects", out volume);
            sfx.value = volume;
            sfx.RegisterValueChangedCallback((evt) => audioMixer.SetFloat("Effects", evt.newValue));

            music = this.Q<Slider>("MusicSlider");
            music.lowValue = soundMin * 2;
            music.highValue = soundMax;
            audioMixer.GetFloat("Music", out volume);
            music.value = volume;
            music.RegisterValueChangedCallback((evt) => audioMixer.SetFloat("Music", evt.newValue));
        }
        
      
        resolutions = this.Q<DropdownField>("ScreenDropDown");
        resolutions.choices = Screen.resolutions.Select((x) => x.width.ToString() + "x" + x.height.ToString()).ToList();
        resolutions.value = Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString();
        resolutions.RegisterValueChangedCallback((evt) =>
        {

            var resolution = Screen.resolutions[resolutions.index];
            int width=resolution.width;
            int height = resolution.height;
            var full = Screen.fullScreenMode;
            Screen.SetResolution(width, height, full);
            Debug.Log(resolution);
            Debug.Log(Screen.currentResolution);
            

        });
        //Screen.SetResolution(1920, 1080, true);



        vsync = this.Q<Toggle>("VSYNCToggle");
        vsync.value=QualitySettings.vSyncCount>0?true:false;
        vsync.RegisterValueChangedCallback(evt => QualitySettings.vSyncCount = evt.newValue ? 1 : 0);
        brightness = this.Q<Slider>("BrightnessSlider");
    }
}

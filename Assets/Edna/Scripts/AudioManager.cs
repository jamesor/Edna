using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace JamesOR.Edna
{
    [System.Serializable]
    public class AudioSetting
    {
        public string ExposedParam;
        public Slider Slider;
        public float DefaultVolume;

        public void Initialize()
        {
            float savedValue = PlayerPrefs.GetFloat(ExposedParam, DefaultVolume);
            Slider.value = savedValue;
            SetMixerVolume(savedValue);
        }

        public void SetExposedParam(float value)
        {
            SetMixerVolume(value);
            PlayerPrefs.SetFloat(ExposedParam, value);
        }

        private void SetMixerVolume(float value)
        {
            float newValue = Mathf.Log10(value) * 20;
            AudioManager.Instance.AudioMixer.SetFloat(ExposedParam, newValue);
        }
    }

    public class AudioManager : Singleton<AudioManager>
    {
        public AudioMixer AudioMixer;
        public AudioSetting[] AudioSettings;
        
        private enum AudioGroups { Master, Music, Effects, Speech };

        void Start()
        {
            for (int i = 0; i < AudioSettings.Length; i++)
            {
                AudioSettings[i].Initialize();
            }
        }

        public void SetMasterVolume(float value)
        {
            AudioSettings[(int)AudioGroups.Master].SetExposedParam(value);
        }

        public void SetMusicVolume(float value)
        {
            AudioSettings[(int)AudioGroups.Music].SetExposedParam(value);
        }

        public void SetEffectsVolume(float value)
        {
            AudioSettings[(int)AudioGroups.Effects].SetExposedParam(value);
        }

        public void SetSpeechVolume(float value)
        {
            AudioSettings[(int)AudioGroups.Speech].SetExposedParam(value);
        }
    }
}

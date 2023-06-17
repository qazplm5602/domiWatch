using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LobbyAudioAdmin : MonoBehaviour
{
    enum Channels {
        Master,
        Effect
    }
    [System.Serializable]
    class DomiSlider {
        public UnityEngine.UI.Slider slider;
        public TMPro.TextMeshProUGUI text;
    }
    [SerializeField] AudioMixer mixer;
    [SerializeField] DomiSlider[] SliderList;

    private void Start() {
        for (int i = 0; i < SliderList.Length; i++)
        {
            DomiSlider domi = SliderList[i];
            Channels ch = (Channels)i;

            string valueStr = PlayerPrefs.GetString("AudioChannel."+ch.ToString(), "100");
            float Percent = float.Parse(valueStr);

            domi.text.text = valueStr;
            domi.slider.value = Percent / 100;
            mixer.SetFloat("volume."+ ch.ToString(), -(80 - (Percent / 100) * 80));
        }
    }
    
    public void ChangeVolume(int channel) {
        Channels ch = (Channels)channel;
        float Value = SliderList[channel].slider.value;

        float Percent = ((Value * 80) / 80) * 100;

        SliderList[channel].text.text = string.Format("{0:0.#}", Percent)+"%";
        PlayerPrefs.SetString("AudioChannel."+ch.ToString(), string.Format("{0:0.#}", Percent));
        mixer.SetFloat("volume."+ ch.ToString(), -(80 - (Value * 80)));
    }
}

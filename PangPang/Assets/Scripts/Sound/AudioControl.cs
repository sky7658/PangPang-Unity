using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PangPang.Sound
{
    public class AudioControl : MonoBehaviour
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        private void Awake()
        {
            bgmSlider = GameObject.Find("SoundManager").transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).GetComponent<Slider>();
            sfxSlider = GameObject.Find("SoundManager").transform.GetChild(2).transform.GetChild(0).transform.GetChild(2).GetComponent<Slider>();

            bgmSlider.value = SoundManager.Instance.masterVolumeBGM;
            sfxSlider.value = SoundManager.Instance.masterVolumeSFX;
        }

        public void SFXVolumeControl()
        {
            float sound = sfxSlider.value;
            SoundManager.Instance.SFXVolumeUpdate(sound);
        }

        public void BGMVolumeControl()
        {
            float sound = bgmSlider.value;
            SoundManager.Instance.BGMVolumeUpdate(sound);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Sound
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager instance;
        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(SoundManager)) as SoundManager;

                    if (instance == null)
                    {
                        instance = new GameObject("@" + typeof(SoundManager).ToString(),
                            typeof(SoundManager)).GetComponent<SoundManager>();
                    }
                }
                return instance;
            }
        }

        private AudioSource bgmPlayer;
        private AudioSource sfxPlayer;

        public float masterVolumeSFX = 1f;
        public float masterVolumeBGM = 1f;

        [SerializeField] private AudioClip[] sfxAudioClips;
        private Dictionary<string, AudioClip> m_Sounds = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);

            sfxPlayer = transform.GetChild(0).GetComponent<AudioSource>();
            bgmPlayer = transform.GetChild(1).GetComponent<AudioSource>();

            foreach (var audioClip in sfxAudioClips)
            {
                m_Sounds.Add(audioClip.name, audioClip);
            }
        }

        public void PlayBGM()
        {
            bgmPlayer.Play();
        }

        public void PlaySFX(string name)
        {
            if (m_Sounds.ContainsKey(name) == false)
            {
                Debug.Log("±×µý°Å ¾øÀ½");
                return;
            }
            sfxPlayer.PlayOneShot(m_Sounds[name]);
        }

        public void BGMVolumeUpdate(float volume)
        {
            bgmPlayer.volume = volume;
        }

        public void SFXVolumeUpdate(float volume)
        {
            sfxPlayer.volume = volume;
        }
    }
}

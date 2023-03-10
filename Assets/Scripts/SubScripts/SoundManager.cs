using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Mentum
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Inst { get; private set; }

        [Title("+[ 사운드 ]")]
        [SerializeField] private AudioClip[] audioClip; // 오디오 소스들 지정.
        private Dictionary<string, AudioClip> audioClipsDict;
        [SerializeField] AudioClip backGroundMusic;

        private float masterVolumeSFX = 1f;
        private float masterVolumeBGM = 0.5f;

        private AudioSource sfxPlayer;
        private AudioSource bgmPlayer;

        private readonly HashSet<string> lastPlayedSound = new();

        private void Awake()
        {
            Inst = this;

            sfxPlayer = transform.GetChild(0).GetComponent<AudioSource>();
            bgmPlayer = transform.GetChild(1).GetComponent<AudioSource>();

            audioClipsDict = new Dictionary<string, AudioClip>();

            foreach (AudioClip a in audioClip)
                EnrollSound(a);

            bgmPlayer.clip = backGroundMusic;
            bgmPlayer.volume = masterVolumeBGM;
            bgmPlayer.Play();
        }

        public void EnrollSound(AudioClip a)
        {
            if (a == null)
                return;

            if (audioClipsDict.ContainsKey(a.name))
            {
                Debug.Log(a.name + "사운드가 중복");
                return;
            }
            audioClipsDict.Add(a.name, a);
        }

        private void Start()
        {
            if (bgmPlayer.clip != null && bgmPlayer.isPlaying == false)
                bgmPlayer.Play();
        }

        public void SetBGMAndPlay(AudioClip bgm)
        {
            bgmPlayer.clip = bgm;
            bgmPlayer.Play();
        }

        private void LateUpdate()
        {
            if (lastPlayedSound.Count > 0)
                lastPlayedSound.Clear();
        }

        public void PlayButtonSound()
        {
            PlaySound("click");
        }

        public void PlaySound(string soundName, float volume = 1f)
        {
            if (!audioClipsDict.TryGetValue(soundName, out AudioClip value))
            {
                Debug.Log($"{soundName} 은 audioClipsDict에 포함되지 않음");
                return;
            }

            if (lastPlayedSound.Contains(soundName))
            {
                if (soundName == "click")
                    Debug.Log($"{soundName} 이 한프레임에 두번 호출됨");

                return;
            }

            lastPlayedSound.Add(soundName);
            sfxPlayer.PlayOneShot(value, volume * masterVolumeSFX);
        }
    }
}
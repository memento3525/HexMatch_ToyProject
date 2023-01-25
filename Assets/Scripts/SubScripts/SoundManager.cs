using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Inst { get; private set; }

    [Title("+[ ���� ]")]
    [SerializeField] private AudioClip[] audioClip; // ����� �ҽ��� ����.
    private Dictionary<string, AudioClip> audioClipsDic;
    public AudioClip backGroundMusic;

    private float masterVolumeSFX = 1f;
    private float masterVolumeBGM = 0.5f;

    private AudioSource sfxPlayer;
    private AudioSource bgmPlayer;

    private readonly List<string> lastPlayedSound = new List<string>();

    private void Awake()
    {
        Inst = this;

        sfxPlayer = transform.GetChild(0).GetComponent<AudioSource>();
        bgmPlayer = transform.GetChild(1).GetComponent<AudioSource>();

        audioClipsDic = new Dictionary<string, AudioClip>();

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

        if (audioClipsDic.ContainsKey(a.name))
        {
            Debug.Log(a.name + "���尡 �ߺ�");
            return;
        }
        audioClipsDic.Add(a.name, a);
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
        if (audioClipsDic.ContainsKey(soundName) == false)
        {
            Debug.Log(soundName + " is not Contained audioClipsDic");
            return;
        }

        if (lastPlayedSound.Contains(soundName))
        {
            if (soundName == "click")
                Debug.Log(soundName + "�� �������ӿ� �ι� ȣ���");

            return;
        }

        lastPlayedSound.Add(soundName);
        sfxPlayer.PlayOneShot(audioClipsDic[soundName], volume * masterVolumeSFX);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource _backgroundAudio;
    //AudioSource _effectAudio;

    protected override void Awake()
    {
        base.Awake();
        _backgroundAudio.loop = true;
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        _backgroundAudio.clip = clip;
        _backgroundAudio.Play();
    }
    public void PlayEffectSound(AudioSource source ,AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
}

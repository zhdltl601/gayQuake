using System;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioClip[] bmgs;
    public AudioSource bgmPlayer;
    public AudioSource sfxPlayer;
    
    public SFX[] sfxes;
    
    private int _bgmIndex = 0;
    
    public void PlaySound(string str)
    {
        SFX s = Array.Find(sfxes,x => x.name == str);
        
        if (s.name != null)
        {
            sfxPlayer.clip = s.AudioClip;
            sfxPlayer.Play();
        }
        
    }
    
    private void Update()
    {
        PlayBgm();
    }
    private void PlayBgm()
    {
        if (bgmPlayer.isPlaying == false)
        {
            bgmPlayer.clip = bmgs[_bgmIndex++ % bmgs.Length];
            bgmPlayer.Play();
        }
    }
}

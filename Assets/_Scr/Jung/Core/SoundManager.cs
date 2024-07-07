using System;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioClip[] bmgs;
    public AudioSource bgmPlayer;
    public AudioSource playerSfxPlayer;
    public AudioSource enemySfxPlayer;
    
    public SFX[] sfxes;
    
    private int _bgmIndex = 0;
    private void Start()
    {
        //SetMusicVolume();
        SetSfxVolume();
    }
    public void PlayPlayerSOund(string str)
    {
        SFX s = Array.Find(sfxes,x => x.name == str);
        
        if (s.name != null)
        {
            playerSfxPlayer.PlayOneShot(s.AudioClip);
        }
        
    }
    
    public void PlayEnemyrSound(string str)
    {
        SFX s = Array.Find(sfxes,x => x.name == str);
        
        if (s.name != null)
        {
            enemySfxPlayer.PlayOneShot(s.AudioClip);
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
    
    public void SetSfxVolume()
    {
        playerSfxPlayer.volume = UIManager.Instance.sfxSlider.value;
        enemySfxPlayer.volume =  UIManager.Instance.sfxSlider.value;
    }
    
    public void SetMusicVolume()
    {
        bgmPlayer.volume = UIManager.Instance.musicSlider.value;
    }
}

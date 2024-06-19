using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using VHierarchy.Libs;

public class UIManager : MonoSingleton<UIManager>
{
    public TextMeshProUGUI _ammoText;
    public WeaponController Player;
    public Transform minimapCam;
    
    public Volume Volume;
    private bool screenEffectting;

    public DiePanel DiePanel;

    public Image hitCrossHair;
    public Image crossHair;
    private bool isCrossChange;
    
    private void Start()
    {
        SetAmmoText();
    }

    public void SetAmmoText()
    {
        _ammoText.SetText($"{Player.GetCurrentGun().gunMagazine.ammoInMagazine}/{Player.GetCurrentGun().gunMagazine.maxAmmoInMagazine} <color=#bdc3c7>{Player.GetCurrentGun().gunMagazine.totalAmmo}</color>");
    }

    private void LateUpdate()
    {
        minimapCam.position = new Vector3(Player.transform.position.x , minimapCam.position.y , Player.transform.position.z);
    }

    public void BloodScreen(Color color ,float duration = 0.2f , float targetIntensity = 0.5f, float screenTime = 0f)
    {
        if(screenEffectting == true)return;
        
        if (Volume.profile.TryGet(out Vignette vignette))
        {
            
            vignette.color.value = color;
            vignette.color.overrideState = true;
            StartCoroutine(BloodScreenCoroutine(vignette , duration , targetIntensity ,screenTime));
        }
    }

    public void OnDiePanel()
    {
        DiePanel.OnPanel();
    }
    
    IEnumerator BloodScreenCoroutine(Vignette vignette , float _duration , float _targetIntensity , float screenTime)
    {
        screenEffectting = true;
        
        float duration = _duration;
        float targetIntensity = _targetIntensity;
        float initialIntensity = vignette.intensity.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(initialIntensity, targetIntensity, elapsed / duration);
            yield return null;
        }
        
        yield return new WaitForSeconds(screenTime);
        
        elapsed = 0f;
        initialIntensity = vignette.intensity.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(initialIntensity, 0f, elapsed / duration);
            yield return null;
        }
        
        vignette.intensity.value = 0f; 
        
        screenEffectting = false;
    }

    public void ChangeCrosshair()
    {
        StartCoroutine(ChangeCrosshairCoroutine());
    }
    
    private IEnumerator ChangeCrosshairCoroutine()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(hitCrossHair.DOFade(1, 0f));
        sequence.Append(hitCrossHair.DOFade(0, 0.2f));
        sequence.OnComplete(() => Debug.Log("Crosshair animation complete"));
        yield return null;
    }
    
    public void SetCrosshair(Sprite sprite)
    {
        crossHair.sprite = sprite;
    }
}

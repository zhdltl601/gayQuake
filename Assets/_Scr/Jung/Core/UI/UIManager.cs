using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VHierarchy.Libs;

public class UIManager : MonoSingleton<UIManager>
{
    public TextMeshProUGUI _ammoText;

    public WeaponController Player;
    public Transform minimapCam;

    public Volume Volume;
    private bool screenEffectting;
    private void Start()
    {
        SetAmmoText();
    }

    public void SetAmmoText()
    {
        _ammoText.SetText($"{Player.GetCurrentGun().gunData.ammoInMagazine}/{Player.GetCurrentGun().gunData.maxAmmoInMagazine} <color=#bdc3c7>{Player.GetCurrentGun().gunData.totalAmmo}</color>");
    }

    private void LateUpdate()
    {
        minimapCam.position = new Vector3(Player.transform.position.x , minimapCam.position.y , Player.transform.position.z);
    }

    public void BloodScreen(float duration , float targetIntensity , Color color , float screenTime)
    {
        if(screenEffectting == true)return;
        
        if (Volume.profile.TryGet(out Vignette vignette))
        {
            
            vignette.color.value = color;
            vignette.color.overrideState = true;
            StartCoroutine(BloodScreenCoroutine(vignette , duration , targetIntensity ,screenTime));
        }
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
}

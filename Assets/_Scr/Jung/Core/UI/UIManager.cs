using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    public TextMeshProUGUI _ammoText;

    public WeaponController Player;
    public Transform minimapCam;
    private void Start()
    {
        _ammoText.SetText($"{Player.GetCurrentGun().gunData.ammoInMagazine} / {Player.GetCurrentGun().gunData.maxAmmoInMagazine}");
    }

    public void SetAmmoText()
    {
        _ammoText.SetText($"{Player.GetCurrentGun().gunData.ammoInMagazine} / {Player.GetCurrentGun().gunData.maxAmmoInMagazine}");
    }

    private void LateUpdate()
    {
        minimapCam.position = new Vector3(Player.transform.position.x , minimapCam.position.y , Player.transform.position.z);
    }
}

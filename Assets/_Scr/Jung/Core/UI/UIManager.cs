using TMPro;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    public TextMeshProUGUI _ammoText;

    public WeaponController Player;

    private void Start()
    {
        _ammoText.SetText($"{Player.GetCurrentGun().gunData.ammoInMagazine} / {Player.GetCurrentGun().gunData.maxAmmoInMagazine}");
    }

    public void SetAmmoText()
    {
        _ammoText.SetText($"{Player.GetCurrentGun().gunData.ammoInMagazine} / {Player.GetCurrentGun().gunData.maxAmmoInMagazine}");
    }
}



public class AmmoBottle : Bottle
{
    public int GetAmmo()
    {
        int Ammo = PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Ammo].GetValue();
        PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Ammo].RemoveValue(Ammo);
        return Ammo;
    }
    
   
}

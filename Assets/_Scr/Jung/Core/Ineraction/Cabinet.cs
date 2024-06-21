using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cabinet : MonoBehaviour
{
    [SerializeField] private int price;
    [Space]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private GoodsList lists;
    public GameObject goods;
    
    [SerializeField] private TextMeshPro priceText;
    
    private Transform _camera;
    public Interaction interaction;
    private Vector3 direction;

    private bool isSell;

    private void Start()
    {
        _camera = Camera.main.transform;
        priceText.SetText(price.ToString());
        
        int random = Random.Range(0, lists.goods.Length - 1);
        goods = Instantiate(lists.goods[random], transform.position + Vector3.up * 1.5f,Quaternion.identity);
        goods.transform.parent = transform;
        goods.name = lists.goods[random].name;
    }

    private void Update()
    {
        if(isSell)return;
        
        goods.transform.Rotate(new Vector3(0,15,0) * (Time.deltaTime * rotateSpeed));

        if (priceText.gameObject.activeSelf)
        {
            LookPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Interaction>() && isSell == false)
        {
            interaction = other.GetComponent<Interaction>();
            
            priceText.gameObject.SetActive(true);
            interaction.onInteraction += OnBuy;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Interaction>() && isSell == false)
        {
            priceText.gameObject.SetActive(false);
            interaction.onInteraction -= OnBuy;
            interaction = null;
        }
    }

    private void OnBuy()
    {
        if (PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Money].GetValue() - price >= 0)
        {
            if (goods != null && goods.GetComponent<Gun>() && interaction.GetComponent<Gun>() != goods.GetComponent<Gun>())
            {
                goods.GetComponent<Gun>().ThrowGun();
                goods.GetComponent<Rigidbody>().AddForce(Vector3.up * 250);
                
                BuySetting();
            }
            else if (goods.TryGetComponent(out Bottle bottle) && interaction.TryGetComponent(out WeaponController weaponController))
            {
                foreach (var item in weaponController.bottleList)
                {
                    if (bottle._bottleDataSo.statType == item._bottleDataSo.statType)
                    {
                        UIManager.Instance.PopupText($"{"이미 존재하는 아이템 입니다."}");
                        return;
                    }
                }
                
                goods.SetActive(false);
                goods.GetComponent<Bottle>().SetBottleParent(weaponController.bottleTrm);
                weaponController.bottleList.Add(goods.GetComponent<Bottle>());
                
                BuySetting();
            }
        }
    }

    private void BuySetting()
    {
        interaction.onInteraction -= OnBuy;
        
        UIManager.Instance.CoinText();
        PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Money].RemoveValue(price);
        SoundManager.Instance.PlayPlayerSOund("Buy");
        priceText.SetText("");
        Destroy(this);
    }

    private void LookPlayer()
    {
        direction = _camera.position - priceText.transform.position;
        direction.y = 0;
                
        priceText.transform.rotation = Quaternion.LookRotation(direction);
        
        Vector3 currentRotation = priceText.transform.eulerAngles;
        currentRotation.y -= 180;
        priceText.transform.eulerAngles = currentRotation;
    }

}

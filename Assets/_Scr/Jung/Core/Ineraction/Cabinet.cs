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

    private void Awake()
    {
        int random = Random.Range(0, lists.goods.Length - 1);
        goods = Instantiate(lists.goods[random], transform.position + Vector3.up * 1.5f,Quaternion.identity);
        goods.transform.parent = transform;
        goods.name = lists.goods[random].name;
        
    }

    private void Start()
    {
        _camera = Camera.main.transform;
        priceText.SetText(price.ToString());
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
        }
    }

    private void OnBuy()
    {
        if(isSell)return;
        
        if (PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Money].GetValue() - price >= 0)
        {
            PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Money].RemoveValue(price);

            if (goods != null && goods.GetComponent<Gun>())
            {
                goods.GetComponent<Gun>().ThrowGun();
                goods.GetComponent<Rigidbody>().AddForce(Vector3.up * 250);
                Destroy(this);
            }
            else if (goods != null && goods.TryGetComponent(out Bottle bottle) && interaction.TryGetComponent(out WeaponController weaponController))
            {
                goods.SetActive(false);
                goods.GetComponent<Bottle>().SetBottleParent(weaponController.bottleTrm);
                weaponController.bottleList.Add(goods.GetComponent<Bottle>());
                Destroy(this); Debug.Log("지랄하네 진짜");
            }
        }
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

using TMPro;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    [SerializeField] private int price;
    [Space]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private GameObject goods;
    [SerializeField] private TextMeshPro priceText;
    
    private Transform _camera;
    public Interaction interaction;
    private Vector3 direction;

    private bool isSell;
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
        if (PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Money].GetValue() - price >= 0)
        {
            PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Money].RemoveValue(price);
            
            goods.GetComponent<Rigidbody>().AddForce(Vector3.up * 250);
            priceText.gameObject.SetActive(false);
            goods = null;
            isSell = true;
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

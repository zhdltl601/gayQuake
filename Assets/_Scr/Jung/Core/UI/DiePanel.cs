using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DiePanel : MonoBehaviour
{
    [SerializeField] private Image diePanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button mainButton;
    
    public void OnPanel()
    {
        diePanel.DOFade(1f, 3f).OnComplete(() =>
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < 5; i++)
            {
                sequence.Append(gameOverText.DOFade(0.4f, 0.1f));
                sequence.Append(gameOverText.DOFade(1f, 0.1f));
            }
            sequence.Append(gameOverText.DOFade(1f, 0.2f));
            sequence.Append(gameOverText.rectTransform.DOMoveY(gameOverText.rectTransform.position.y + 175, 0.3f)).OnComplete(
                () =>
                {
                    mainButton.gameObject.SetActive(true);
                    mainButton.image.DOFade(1f, 0.3f);
                });
            
            sequence.Play();
        });
        //diePanel.DOFade(1f, 0.3f);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}

using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DiePanel : MonoBehaviour
{
    [SerializeField] private Image diePanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button mainButton;
    
    public void OnPanel()
    {
        Time.timeScale = 0;

        diePanel.DOFade(1f, 3f).SetUpdate(true).OnComplete(() =>
        {
            Sequence sequence = DOTween.Sequence().SetUpdate(true);
            for (int i = 0; i < 5; i++)
            {
                sequence.Append(gameOverText.DOFade(0.4f, 0.1f).SetUpdate(true));
                sequence.Append(gameOverText.DOFade(1f, 0.1f).SetUpdate(true));
            }
            sequence.Append(gameOverText.DOFade(1f, 0.2f).SetUpdate(true));
            sequence.Append(gameOverText.rectTransform.DOMoveY(gameOverText.rectTransform.position.y + 175, 0.3f).SetUpdate(true)).OnComplete(
                () =>
                {
                    mainButton.gameObject.SetActive(true);
                    mainButton.image.DOFade(1f, 0.3f).SetUpdate(true);
                });

            sequence.Play();
        });

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}

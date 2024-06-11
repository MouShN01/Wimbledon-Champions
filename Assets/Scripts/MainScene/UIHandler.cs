using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class UIHandler : NetworkBehaviour
{
    public Slider powerSlide;
    public Slider toHitSlider;
    public GameObject winBox;
    public TextMeshProUGUI winText;
    
    public PlayerControl Player;
    public GameController gameCon;
    
    public void UpdateHUD(float upperBorder, float loverBorder)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(Player.transform.position);
        
        RectTransform canvasRect = this.GetComponent<RectTransform>();
        Vector2 viewportPosition = Camera.main.ScreenToViewportPoint(screenPosition);
        Vector2 worldObjectScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        
        RectTransform powerSlideRect = powerSlide.GetComponent<RectTransform>();
        RectTransform toHitSliderRect = toHitSlider.GetComponent<RectTransform>();
        
        powerSlideRect.anchoredPosition = new Vector2(worldObjectScreenPosition.x, powerSlideRect.anchoredPosition.y);
        toHitSliderRect.anchoredPosition = new Vector2(worldObjectScreenPosition.x, toHitSliderRect.anchoredPosition.y);
        
        if (Ball.B != null)
        {
            powerSlide.value = Ball.B.hitPower;
        }
        toHitSlider.value = (upperBorder + loverBorder) / 2;

        if (gameCon.isGameEnded)
        {
            winBox.SetActive(true);
            winText.text = gameCon.winnersMsg;
        }
    }
}

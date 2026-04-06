using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;
using UnityEngine;

public class FloatingJoyStick : OnScreenStick, IPointerDownHandler, IPointerUpHandler
{
    public Image stickBaseImg;
    public Image stickKnobImg;

    private void Awake()
    {
        stickBaseImg.enabled = false;
        stickKnobImg.enabled = false;
    }

    public new void OnPointerDown(PointerEventData eventData)
    {
        stickBaseImg.rectTransform.position = eventData.position;
        stickKnobImg.rectTransform.position = eventData.position;

        stickBaseImg.enabled = true;
        stickKnobImg.enabled = true;

        base.OnPointerDown(eventData);
    }

    public new void OnPointerUp(PointerEventData eventData)
    {
        stickBaseImg.enabled = false;
        stickKnobImg.enabled = false;

        base.OnPointerUp(eventData);
    }
}
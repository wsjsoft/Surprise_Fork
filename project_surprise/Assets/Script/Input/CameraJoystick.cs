using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraJoystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform lever;
    private RectTransform rectTransform; // ¡∂¿ÃΩ∫∆Ω¿« RectTransform

    [HideInInspector]
    public Vector3 Dir { get; private set; }
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        lever.position = eventData.position;
        lever.localPosition = Vector3.ClampMagnitude(eventData.position - (Vector2)transform.position, rectTransform.rect.width * 0.5f);

        Dir = new Vector3(lever.localPosition.x, 0, lever.localPosition.y).normalized;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lever.localPosition = Vector3.zero;
        Dir = Vector3.zero;
    }
}

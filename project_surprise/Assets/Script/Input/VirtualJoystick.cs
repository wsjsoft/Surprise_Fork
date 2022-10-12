using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // 키보드, 마우스, 터치를 이벤트로 오브젝트에 보낼 수 있는 기능 지원

public class VirtualJoystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform lever;
    private RectTransform rectTransform; // 조이스틱의 RectTransform

    [HideInInspector]
    public Vector3 Dir { get; private set; }

    [SerializeField, Range(10, 150)] // Range()이 범위 안에서만 값의 수정이 가능하게!
    private float leverRange;

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
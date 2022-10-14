using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class CameraJoystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform lever;
    private RectTransform rectTransform; // ¡∂¿ÃΩ∫∆Ω¿« RectTransform

    [HideInInspector]
    public Vector3 Dir { get; private set; }

    CinemachineFreeLook camFreeLook;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        camFreeLook = FindObjectOfType<CinemachineFreeLook>();
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

    void MoveCamera()
    {
        camFreeLook.m_XAxis.m_InputAxisValue = Dir.x;
        camFreeLook.m_YAxis.m_InputAxisValue = Dir.z;
    }

    void FixedUpdate()
    {
        MoveCamera();
    }
}

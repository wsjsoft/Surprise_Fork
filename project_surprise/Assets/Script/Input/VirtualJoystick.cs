using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // 키보드, 마우스, 터치를 이벤트로 오브젝트에 보낼 수 있는 기능 지원

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform lever;
    private RectTransform rectTransform; // 조이스틱의 RectTransform

    [SerializeField, Range(10, 150)] // Range()이 범위 안에서만 값의 수정이 가능하게!
    private float leverRange;

    private Vector2 inputDirection;
    private bool isInput;

    private PlayerInput input;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        input = FindObjectOfType<PlayerInput>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        ControlJoystickLever(eventData);
        isInput = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        ControlJoystickLever(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lever.anchoredPosition = Vector2.zero; //손을 떼면 조이스틱의 중심으로 돌아가게
        isInput = false;
        input.Movement(Vector2.zero);
    }

    private void ControlJoystickLever(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransform.anchoredPosition;
        var inputVector = inputPos.magnitude < leverRange ? inputPos : inputPos.normalized * leverRange;
        lever.anchoredPosition = inputVector; // inputVector : 해상도를 기반으로 만들어진 값이라 캐릭터움직임에 쓰면 너무 큰 값이라 너무 빠르게 움직일 것임
        inputDirection = -1*(inputVector / leverRange); // 따라서, 0~1사이의 정규화된 값을 캐릭터 움직임으로 전달하기위해
        // 캐릭터의 움직임이 조이스틱의 레버 방향과 반대로되서 -1곱함
    }

    private void InputControlVector() // 캐릭터에게 입력벡터 전달
    {
        input.Movement(inputDirection);
    }

    void Update()
    {
        if (isInput)
        {
            InputControlVector();
        }
    }
}
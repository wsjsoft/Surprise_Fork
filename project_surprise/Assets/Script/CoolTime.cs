using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolTime : MonoBehaviour
{
    float coolTime = 3;
    Slider slider;

    Button button;

    //time
    float current;
    float percent;

    private void Awake()
    {
        button = transform.parent.GetComponent<Button>();//부모오브젝트의 버튼

        slider = GetComponentInChildren<Slider>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        slider.maxValue = 1;
        button.interactable = false;//쿨타임이므로 버튼 비활성화
    }

    public void SetCoolTime(float time)
    {
        coolTime = time;
    }

    private void Update()
    {
        current += Time.deltaTime;
        if(percent < 1)
        {
            percent = current / coolTime;
            slider.value = percent;
            //슬라이더의 maxValue값이 1이므로
            //coolTime 값이 다양해도 일관되게 계산할 수 있음

        }
        else
        {
            current = 0;
            percent = 0;
            button.interactable = true;//쿨타임 이후로 버튼 활성화
            gameObject.SetActive(false);
        }
    }
}

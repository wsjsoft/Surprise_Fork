using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolTime : MonoBehaviour
{
    float coolTime = 3;
    Slider slider;//쿨타임을 나타낼 슬라이더

    Button button;//부모 오브젝트 버튼(달리기, 공격)을 쿨타임이 끝나면 활성화, 쿨타임 중일 때는 비활성화 하기 위해 필요

    //time
    float current;//경과시간
    float percent;//경과시간 / 최종 쿨타임

    private void Awake()
    {
        button = transform.parent.GetComponent<Button>(); // 부모오브젝트의 버튼

        slider = GetComponentInChildren<Slider>(); //쿨타임을 표시해줄 슬라이더
        slider.maxValue = 1; //cooltime을 분모에 넣고 시간 경과를 분자에다 넣고 계산할 것이므로 maxValue를 1로 설정한다.
    }

    private void OnEnable()
    {
        current = 0;//경과시간 초기화
        percent = 0;//경과시간/최종시간 값 초기화 -> 슬라이더에 실질적으로 들어갈 값으로 0부터 시작해야 함.
        button.interactable = false; // 쿨타임이므로 버튼 비활성화
    }

    private void Start()
    {
        button.interactable = true; //onEnable에서 비활성화하기 때문에 처음에 부모 버튼이 비활성화된 상태로 되어 달리기와 공격을 누를 수 없어 추가한 코드
        gameObject.SetActive(false); //모든 초기 세팅을 맞춘뒤 비활성화
    }

    public void SetCoolTime(float time)
    {
        coolTime = time;//Playercontroller에서 달리기와 공격 cooltime을 지정하여 넘겨준다.
    }

    private void Update()
    {
        current += Time.deltaTime;//쿨타임의 시작부터 경과시간을 나타냄
        if(percent < 1)//쿨타임 진행중
        {
            percent = current / coolTime;//경과시간/최종시간 의 값. 최종시간이 몇이든 관계없이 0~1이므로 슬라이더의 maxValue를 1로 두고 변경할 필요가 없음
            slider.value = percent; //지속적으로 경과시간을 반영하여 슬라이더 값으로 보여준다.
            //슬라이더의 maxValue값을 1로만 정해놓으면
            //coolTime 값이 다양해도 일관되게 계산할 수 있음

        }
        else//쿨타임 종료
        {
            button.interactable = true;//쿨타임 이후로 버튼 활성화
            gameObject.SetActive(false);//쿨타임 종료되면 쿨타임 패널 비활성화
        }
    }
}

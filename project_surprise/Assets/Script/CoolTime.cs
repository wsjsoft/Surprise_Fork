using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolTime : MonoBehaviour
{
    float coolTime = 3;
    Slider slider;//��Ÿ���� ��Ÿ�� �����̴�

    Button button;//�θ� ������Ʈ ��ư(�޸���, ����)�� ��Ÿ���� ������ Ȱ��ȭ, ��Ÿ�� ���� ���� ��Ȱ��ȭ �ϱ� ���� �ʿ�

    //time
    float current;//����ð�
    float percent;//����ð� / ���� ��Ÿ��

    private void Awake()
    {
        button = transform.parent.GetComponent<Button>(); // �θ������Ʈ�� ��ư

        slider = GetComponentInChildren<Slider>(); //��Ÿ���� ǥ������ �����̴�
        slider.maxValue = 1; //cooltime�� �и� �ְ� �ð� ����� ���ڿ��� �ְ� ����� ���̹Ƿ� maxValue�� 1�� �����Ѵ�.
    }

    private void OnEnable()
    {
        current = 0;//����ð� �ʱ�ȭ
        percent = 0;//����ð�/�����ð� �� �ʱ�ȭ -> �����̴��� ���������� �� ������ 0���� �����ؾ� ��.
        button.interactable = false; // ��Ÿ���̹Ƿ� ��ư ��Ȱ��ȭ
    }

    private void Start()
    {
        button.interactable = true; //onEnable���� ��Ȱ��ȭ�ϱ� ������ ó���� �θ� ��ư�� ��Ȱ��ȭ�� ���·� �Ǿ� �޸���� ������ ���� �� ���� �߰��� �ڵ�
        gameObject.SetActive(false); //��� �ʱ� ������ ����� ��Ȱ��ȭ
    }

    public void SetCoolTime(float time)
    {
        coolTime = time;//Playercontroller���� �޸���� ���� cooltime�� �����Ͽ� �Ѱ��ش�.
    }

    private void Update()
    {
        current += Time.deltaTime;//��Ÿ���� ���ۺ��� ����ð��� ��Ÿ��
        if(percent < 1)//��Ÿ�� ������
        {
            percent = current / coolTime;//����ð�/�����ð� �� ��. �����ð��� ���̵� ������� 0~1�̹Ƿ� �����̴��� maxValue�� 1�� �ΰ� ������ �ʿ䰡 ����
            slider.value = percent; //���������� ����ð��� �ݿ��Ͽ� �����̴� ������ �����ش�.
            //�����̴��� maxValue���� 1�θ� ���س�����
            //coolTime ���� �پ��ص� �ϰ��ǰ� ����� �� ����

        }
        else//��Ÿ�� ����
        {
            button.interactable = true;//��Ÿ�� ���ķ� ��ư Ȱ��ȭ
            gameObject.SetActive(false);//��Ÿ�� ����Ǹ� ��Ÿ�� �г� ��Ȱ��ȭ
        }
    }
}

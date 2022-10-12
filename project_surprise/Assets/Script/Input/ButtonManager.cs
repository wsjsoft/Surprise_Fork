using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    PlayerInput playerInput;
    public Text readyText;
    int readyButton = 0;

    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }
    public void RunButtonDown()
    {
        playerInput.run = true; // PlayerInput의 Update에서 매번 검사하지않아도되서 여기서 넣는게 나은거같기도,,
        Debug.Log("버튼Down 눌림");
    }
    public void RunButtonUp()
    {
        playerInput.run = false;
        Debug.Log("버튼Up 눌림");
    }
    public void AttackButtonDown()
    {
        playerInput.attack = true;
        Debug.Log("버튼Down 눌림");
    }
    public void AttackButtonUp()
    {
        playerInput.attack = false;
        Debug.Log("버튼Up 눌림");
    }

    public void ReadyButton()
    {
        readyButton++;

        if(readyButton % 2 == 1)
        {
            playerInput.ready = true;
            readyText.text = "준비";
            Debug.Log(playerInput.ready);
        }
        else
        {
            playerInput.ready = false;
            readyText.text = "";
            Debug.Log(playerInput.ready);
        }
    }
}
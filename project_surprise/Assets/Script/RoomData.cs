using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    public string roomName = "";
    public Text roomNameText;
    public int maxPlayer = 0;
    public int playerCount = 0;

    public void UpdateInfo()
    {
        roomNameText.text = string.Format(" {0} [{1}/{2}]", roomName, playerCount.ToString("00"), maxPlayer);
    }
}

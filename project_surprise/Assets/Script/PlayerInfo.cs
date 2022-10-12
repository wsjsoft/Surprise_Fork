using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Player
{
    public string name;
    public RenderTexture render;
}

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo instance;

    [SerializeField] Player[] playerList = null;
    public string playerName { get; private set; }
    [SerializeField] RawImage playerImg = null;

    AudioSource audioSource;
    [SerializeField] AudioClip clickClip;
    [SerializeField] int playerNum = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        Setting();
        audioSource = FindObjectOfType<AudioSource>();
    }

    public void NextButton()
    {
        audioSource.PlayOneShot(clickClip);
        if (++playerNum > playerList.Length - 1) playerNum = 0;
        Setting();
    }

    void Setting()
    {
        playerName = playerList[playerNum].name;
        playerImg.texture = playerList[playerNum].render;
    }

    public Player GetCurrentPlayer()
    {
        return playerList[playerNum];
    }
}

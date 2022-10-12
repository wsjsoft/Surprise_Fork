using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip clickClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }
    public void MainScene()
    {
        audioSource.PlayOneShot(clickClip);
        Invoke("GetMainScene", 0.5f);
    }

    void GetMainScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}

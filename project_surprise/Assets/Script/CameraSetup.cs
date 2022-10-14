using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class CameraSetup : MonoBehaviourPun
{
    void Start()
    {
        if(photonView.IsMine)
        {
            CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
            cam.Follow = transform;
            cam.LookAt = transform;
        }
    }
}

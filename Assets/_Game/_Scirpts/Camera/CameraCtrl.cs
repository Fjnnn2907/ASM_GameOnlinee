using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraCtrl : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        if(!virtualCamera)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

        SetupCamera();
    }
    private void SetupCamera()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                Transform followPoint = player.transform;
                virtualCamera.Follow = followPoint;
                break;
            }
        }
    }
}

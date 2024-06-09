using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerCollisionsHandler : MonoBehaviour
{
    private const string PLAYER = "Player";

    private Player player;
    private PhotonView photonView;

    private void Awake()
    {
        player = GetComponent<Player>();

        photonView = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;
        if(collision.gameObject.CompareTag(PLAYER))
        {
            AgoraManager.Instance.JoinChannel();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;
        if (collision.gameObject.CompareTag(PLAYER))
        {
            AgoraManager.Instance.LeaveChannel();
        }
    }
}

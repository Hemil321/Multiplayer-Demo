using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RoomManagerUI : MonoBehaviourPunCallbacks
{
    private const string GAME_SCENE = "GameScene";

    [SerializeField] private TMP_InputField createRoomField;
    [SerializeField] private TMP_InputField joinRoomField;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomField.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomField.text);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;

    private Rigidbody2D rigidbody;
    private PhotonView photonView;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        //To avoid moving all the players by a single player
        if (photonView.IsMine)
        {
            Vector2 movementDirection = InputManager.Instance.GetMovementDirectionNormalized();

            rigidbody.velocity = movementDirection * movementSpeed * Time.fixedDeltaTime;
        }
    }
}

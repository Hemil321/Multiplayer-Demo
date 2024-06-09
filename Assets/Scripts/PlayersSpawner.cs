using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [Header("Boundaries for Spawning the Player")]

    [SerializeField] private float minXBoundary;
    [SerializeField] private float maxXBoundary;
    [SerializeField] private float minYBoundary;
    [SerializeField] private float maxYBoundary;

    private void Start()
    {
        //Spawn every player at a randomly generated position
        Vector2 spawnPosition = new Vector2(Random.Range(minXBoundary, maxXBoundary), Random.Range(minYBoundary, maxYBoundary));

        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

    }
}

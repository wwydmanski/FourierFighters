using System;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    [Serializable]
    public class PlayerManager
    {
    [HideInInspector] public int playerNumber;
    [HideInInspector] public GameObject instance;
    public Transform spawnPoint;


    private PlayerController.PlayerController _playerController;

    public void Setup()
    {
        _playerController = instance.GetComponent<PlayerController.PlayerController>();
        _playerController.PlayerNumber = playerNumber;
    }
    }
}
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
        public int wins;
    
        private PlayerController.PlayerController _playerController;
    
        public void Setup()
        {
            _playerController = instance.GetComponent<PlayerController.PlayerController>();
            _playerController.PlayerNumber = playerNumber;
        }
    
        public void DisableControl()
        {
            _playerController.enabledController = false;
        }
    
        public void EnableControl()
        {
            _playerController.enabledController = true;
            _playerController.FreezeAllPosition(false);
        }

        public bool IsState()
        {
            return _playerController.IsState();
        }
        
        public void Reset()
        {
            _playerController.ResetPlayer();
            _playerController.FreezeAllPosition(true);
            instance.transform.position = spawnPoint.position;
            _playerController.SetState(true);
                        
        }
    }
}
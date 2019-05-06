using System.Collections;
using Assets.Scripts.Manager;
using UnityEngine;
namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public GameObject playerPrefab;

        public PlayerManager[] players;
    
        // Start is called before the first frame update
        void Start()
        {
            SpawnAllPlayers();
        }

        private void SpawnAllPlayers()
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].instance = Instantiate(playerPrefab, players[i].spawnPoint);
                players[i].playerNumber = i + 1;
                players[i].Setup();
            }       
        }

        private void Update()
        {
        
        }
    }
}


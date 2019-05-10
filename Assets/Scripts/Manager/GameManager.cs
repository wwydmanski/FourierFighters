using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public GameObject playerPrefab;

        public PlayerManager[] players;

        private Color[] playerColors;

        // Start is called before the first frame update
        void Start()
        {
            playerColors = new Color[4];

            playerColors[0] = Color.yellow;
            playerColors[1] = Color.green;
            playerColors[2] = Color.red;
            playerColors[3] = Color.blue;
            SpawnAllPlayers();
        }

        private void SpawnAllPlayers()
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].instance = Instantiate(playerPrefab, players[i].spawnPoint);
                players[i].playerNumber = i + 1;
                players[i].Setup();
                StartCoroutine(SetPlayerColor(players[i].instance, playerColors[i]));
            }
        }

        private IEnumerator SetPlayerColor(GameObject player, Color color)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            player.GetComponent<SignalCaster>().SetColor(color);
        }

        private void Update()
        {
        
        }
    }
}


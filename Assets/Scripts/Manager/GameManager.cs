using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public GameObject PlayerPrefab;

        public PlayerManager[] Players;

        private Color[] _playerColors;

        // Start is called before the first frame update
        void Start()
        {
            _playerColors = new Color[4];

            _playerColors[0] = Color.yellow;
            _playerColors[1] = Color.red;
            _playerColors[2] = Color.green;
            _playerColors[3] = Color.blue;
            SpawnAllPlayers();
        }

        private void SpawnAllPlayers()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].instance = Instantiate(PlayerPrefab, Players[i].spawnPoint);
                Players[i].playerNumber = i + 1;
                Players[i].Setup();
                Players[i].instance.name = $"Player {i + 1}";
                StartCoroutine(SetPlayerColor(Players[i].instance, _playerColors[i]));
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


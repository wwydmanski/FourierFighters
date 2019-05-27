using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public string sceneName;
        public PlayerManager[] Players;
        private PlayerManager _roundWinner;
        private PlayerManager _gameWinner;
        
        public int numRoundsToWin = 3;
        private Color[] _playerColors;

        private int _roundNumber;
        // Start is called before the first frame update
        void Start()
        {
            _playerColors = new Color[4];

            _playerColors[0] = Color.yellow;
            _playerColors[1] = Color.red;
            _playerColors[2] = Color.green;
            _playerColors[3] = Color.blue;
            SpawnAllPlayers();
            StartCoroutine(GameLoop());
        }

        private void SpawnAllPlayers()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].instance = Instantiate(PlayerPrefab, Players[i].spawnPoint);
                Players[i].playerNumber = i + 1;
                Players[i].Setup();
                Players[i].instance.name = $"Player {i + 1}";
                Players[i].instance.GetComponent<PlayerController.PlayerController>().AntennaIcon = GameObject.Find($"Antenna{i+1}");

                StartCoroutine(SetPlayerColor(Players[i].instance, _playerColors[i]));
            }
        }

        private IEnumerator SetPlayerColor(GameObject player, Color color)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            player.GetComponent<SignalCaster>().SetColor(color);
        }

        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(RoundStarting());
            yield return StartCoroutine(RoundPlaying());
            yield return StartCoroutine(RoundEnding());
            
            if (_gameWinner == null)
            {
                StartCoroutine(GameLoop());
            }
            else
            {
                print("Game Winner: " + _gameWinner.instance.name);
                SceneManager.LoadScene(sceneName);
            }
        }

        private IEnumerator RoundStarting()
        {
            ResetAllPlayers();
            DisabledPlayerControl();

            _roundNumber++;

            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator RoundPlaying()
        {
            EnablePlayerControl();

            while (!OnePlayerLeft())
            {
                yield return null;
            }
        }

        private IEnumerator RoundEnding()
        {
            //DisabledPlayerControl();

            _roundWinner = GetRoundWinner();

            if (_roundWinner != null)
            {
                _roundWinner.wins++;
                print("Round Winner: " + _roundWinner.instance.name);
            }

            _gameWinner = GetGameWinner();
            
            yield return new WaitForSeconds(3f);
        }

        private void ResetAllPlayers()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].Reset();
            }
        }

        private void EnablePlayerControl()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].EnableControl();
            }
        }
        
        private void DisabledPlayerControl()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].DisableControl();
            }
        }

        private bool OnePlayerLeft()
        {
            int numPlayerLeft = 0;
            
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].IsState())
                {
                    numPlayerLeft++;
                }
            }

            return numPlayerLeft <= 1;
        }

        private PlayerManager GetRoundWinner()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].IsState())
                {
                    return Players[i];
                }
            }

            return null;
        }
        
        private PlayerManager GetGameWinner()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].wins == numRoundsToWin)
                {
                    return Players[i];
                }
            }

            return null;
        }
        
        
        private void Update()
        {
        
        }
    }
}


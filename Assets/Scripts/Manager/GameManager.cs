using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform SpawnPoint1;
    public Transform SpawnPoint2;

    public GameObject playerPrefab;

    
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(playerPrefab, SpawnPoint1);
        Instantiate(playerPrefab, SpawnPoint2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

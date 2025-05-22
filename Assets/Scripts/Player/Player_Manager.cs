using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player_Manager: Singleton<Player_Manager>
{

    [Header("Player Prefabs")]
    [SerializeField]
    private GameObject[] PlayerPrefabs;

    void Start()
    {
        if (PlayerPrefabs.Length == 0)
        {
            Debug.LogError("No player prefabs assigned in Player_Manager.");
            return;
        }

        GameObject player = Instantiate(PlayerPrefabs[0]);
        player.transform.position = new Vector3(0, 0, 0);
        player.name = "Player";
    }

    void Update()
    {
        
    }
}

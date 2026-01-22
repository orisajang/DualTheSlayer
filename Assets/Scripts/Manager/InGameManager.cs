using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : Singleton<InGameManager>
{
    [SerializeField] GameObject _spawnPos;

    public GameObject SpawnPos => _spawnPos;
}

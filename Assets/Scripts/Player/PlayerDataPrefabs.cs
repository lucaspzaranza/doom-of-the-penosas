using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataPrefabs", menuName = "ScriptableObjects/PlayerDataPrefabs", order = 1)]
public class PlayerDataPrefabs : ScriptableObject
{
    [SerializeField] private Penosas _character;
    public Penosas Character => _character;

    [SerializeField] private GameObject _playerPrefab;
    public GameObject PlayerPrefab => _playerPrefab;

    [SerializeField] private List<GameObject> _listOf1stShots;
    public List<GameObject> ListOf1stShots => _listOf1stShots;

    [SerializeField] private List<GameObject> _listOf2ndShots;
    public List<GameObject> ListOf2ndShots => _listOf2ndShots;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Game Data",menuName = "Game Data",order = 53)]
public class GameData : ScriptableObject
{
    [SerializeField]
    private LevelInformationData[] _levels=new LevelInformationData[3];

    public LevelInformationData[] Levels => _levels;

    public GameData()
    {
    }
}

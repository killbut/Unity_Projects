using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]



[CreateAssetMenu(fileName = "New Level Information ", menuName = "Level Information", order = 52)]
public class LevelInformationData : ScriptableObject
{
    [SerializeField] private int _columns;
    [SerializeField] private int _rows;
    [SerializeField] private SkinBundleData _bundle;

    public int Columns => _columns;
    public int Rows => _rows;
    public SkinBundleData BundleData => _bundle;
    public CellGrid[,] GameField { get; set; }

}
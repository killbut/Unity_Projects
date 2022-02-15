using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Bundle", menuName = "Bundle Data", order = 52)]
public class SkinBundleData : ScriptableObject
{
  [SerializeField] private SkinSprite[] _skinData;
  public SkinSprite[] ValueData => _skinData;

}

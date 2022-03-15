using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Level/Picture",fileName = "New Picture")]
public class Picture : ScriptableObject
{
    [SerializeField] private Sprite _picture;

    public Sprite PictureRender => _picture;
}

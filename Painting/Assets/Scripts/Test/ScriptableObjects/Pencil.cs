using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName = "new Pencil",menuName = "Painting/Pencil")]
public class Pencil : ScriptableObject
{
    [SerializeField] private Color _color;
    
    [SerializeField] private Sprite _sprite;
    
    public Color Color => _color;
    public Sprite Sprite => _sprite;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;

public class CreateSetPencil : MonoBehaviour
{
    [SerializeField] private Pencil[] _pencils;
    [SerializeField] private GameObject _prefab;
    public Pencil[] Pencils => _pencils;
    void Start()
    {
        CreatePencil();
    }

    private void CreatePencil()
    {
        for (int i = 0; i < _pencils.GetLength(0); i++)
        {
            SetSprite(_pencils[i].Sprite,_prefab);
            var pencil=Instantiate(_prefab,gameObject.transform);
            SetPosition(pencil,i);
            pencil.name = _pencils[i].name;
        }
    }
    private void SetSprite(Sprite sprite,GameObject pencil)
    {
        pencil.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    private void SetPosition(GameObject pencil,int i)
    {
        pencil.transform.localPosition = new Vector3(300f * i, 0f, 0f);
        pencil.transform.localScale = new Vector3(150, 100, 0);
    }

}

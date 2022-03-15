using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickToPencil : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _ObjectToSetColor;
    
    private string _pencilString = "Pencil";
    private CreateSetPencil _pencilSet;
    private Color _color;
    protected void Start()
    {
        _pencilSet = gameObject.GetComponent<CreateSetPencil>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
       Debug.Log(eventData.pointerEnter);
       if (eventData.pointerEnter.name.Any(x => _pencilString.Contains(x)))
       {
           SearchColor(eventData.pointerEnter);
           SetColorToObject();
       }
    }
    private void SearchColor(GameObject pencilClicked)
    {
        for (int i = 0; i < _pencilSet.Pencils.GetLength(0); i++)
        {
            if (_pencilSet.Pencils[i].name == pencilClicked.name)
            {
                _color = _pencilSet.Pencils[i].Color;
            }
        }
    }
    private void SetColorToObject()
    {
        _ObjectToSetColor.GetComponent<Image>().color = _color;
    }
}

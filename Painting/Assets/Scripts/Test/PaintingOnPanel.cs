using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class PaintingOnPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _objectWithColor;
    [SerializeField] private LineRenderer _lineRenderer;

    private bool _onPanel = false;
    private Transform _parent;
    private int count = 0;
    protected void Start()
    {
        _parent = _lineRenderer.gameObject.transform;
        SetupLineRenderer();
    }

    protected void Update()
    {
        if (_onPanel)
        {
            if (Input.GetMouseButton(0))
            {
                var position = GetWorldPosition(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
                _lineRenderer.positionCount++;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, position);
            }

            if (Input.GetMouseButtonUp(0))
            {
                _lineRenderer = Instantiate(_lineRenderer, _parent);
                _lineRenderer.positionCount = 0;
                _lineRenderer.gameObject.name = $"Line_{count}";
                count++;
            }
        }

        if (!_onPanel)
        {
            if (Input.GetMouseButton(0) && _lineRenderer.positionCount>0)
            {
                _lineRenderer = Instantiate(_lineRenderer, _parent);
                _lineRenderer.positionCount = 0;
                _lineRenderer.gameObject.name = $"Line_{count}";
                count++;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _onPanel = true;
        SetupLineRenderer();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
        _onPanel = false;
    }

    private void SetupLineRenderer()
    {
        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
        _lineRenderer.positionCount = 0;
        _lineRenderer.numCornerVertices = 5;
        SetColorToLineRenderer();
    }

    private void SetColorToLineRenderer()
    {
        _lineRenderer.startColor = _objectWithColor.GetComponent<Image>().color;
        _lineRenderer.endColor = _objectWithColor.GetComponent<Image>().color;
    }

    private Vector3 GetWorldPosition(Vector3 localPosition)
    {
        return Camera.main.ScreenToWorldPoint(localPosition);
    }
}
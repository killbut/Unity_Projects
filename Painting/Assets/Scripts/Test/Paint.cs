using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;


public class Paint : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Texture2D _texture;
    [SerializeField] private Camera _camera;
    [Range(2,2048)]
    [SerializeField] private int _height;
    [Range(2,1024)]
    [SerializeField] private int _width;
    [Range(1,50)]
    [SerializeField] private float _brushSize = 10;

    [SerializeField] private Color _color;
    [SerializeField] private TextureWrapMode _wrapMode;
    [SerializeField] private FilterMode _filterMode;
    [SerializeField] private BoxCollider2D _collider;
    
    [SerializeField] private LineRenderer _line;
    [Range(0f,1f)]
    [SerializeField] private float _startWidth = 0.5f;
    [Range(0f,1f)]
    [SerializeField] private float _endWidth = 0.2f;
    private Random _random;
    void Start()
    {
        _width = Screen.width;
        _height = Screen.height;
        _texture = new Texture2D(_width, _height);
        _line.positionCount = 0;
        _line.startColor = _color;
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _texture.SetPixel(x,y,Color.white);
            }
        }
        _texture.Apply();
        _spriteRenderer.sprite=Sprite.Create(_texture,new Rect(0,0,_width,_height),new Vector2(0.5f,0.5f));
        gameObject.AddComponent<BoxCollider2D>();
    }

    protected void OnValidate()
    {
        if (_texture != null)
        {
            if (_texture.width != _width || _texture.height != _height)
            {
                _texture.Resize(_width, _height);
            }
        }

        if (_line.startWidth != _startWidth || _line.endWidth != _endWidth)
        {
            _line.startWidth = 0.2f;
            _line.endWidth = 0.5f;
        }

        if (_line.endColor != _color || _line.startColor != _color)
        {
            _line.startColor = _color;
            _line.endColor = _color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var mouseLocalPos = Input.mousePosition;
            var globalPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            
            var testPos = _camera.ScreenPointToRay(Input.mousePosition);
            _line.positionCount++;
            _line.SetPosition(_line.positionCount-1, new Vector3(globalPos.x,globalPos.y,0));
            Debug.DrawLine(Vector3.zero, globalPos,Color.green,5f);
            int xx = (int) (mouseLocalPos.x/Screen.width * _width);
            int yy = (int)(mouseLocalPos.y/Screen.height *_height);
            for (int x = 0; x < _brushSize; x++)
            {
                for (int y = 0; y < _brushSize; y++)
                {
                    _texture.SetPixel((int) (xx+x ),(int) (yy+y),_color);
                }
            }
            
            _texture.Apply();
            Debug.Log(globalPos.ToString());
        }

        if (Input.GetMouseButtonUp(0))
        {
            _line.positionCount = 0;
        }
    }
}

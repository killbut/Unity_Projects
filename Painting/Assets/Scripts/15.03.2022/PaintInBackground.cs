using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class PaintInBackground : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Color _color;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [Range(1,50)]
    [SerializeField] private int _brushSize;
    [SerializeField] private ColoringType _colorType;
    [SerializeField] private BrushType _brushType;
    private Texture2D _textureBackground;
    private int[,] _coordinates;
    private int _deltaY;
    private Queue<int> _coordinatesInQueue;
    private int _ScreenHeight;
    private enum ColoringType
    {
        Solid,
        MultiColor
    }

    private enum BrushType
    {
        Quad,
        Circle
    }
    // Start is called before the first frame update
    void Start()
    {
        _textureBackground = _spriteRenderer.sprite.texture;
        _deltaY = GetComponent<CreateBackground>().DeltaY;
        _ScreenHeight = Screen.height;
        _coordinates = new int[25, 25];
        _coordinatesInQueue = new Queue<int>();
        if(_textureBackground!=null)
            Debug.Log("YES");
    }

    protected void OnValidate()
    {
        
    }
    protected 
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector2 center = new Vector2(pos.x, (pos.y - (_ScreenHeight * 0.1f + _deltaY)));
            
            if (_brushType == BrushType.Quad)
            {
                DrawQuad(center);
            }
            else if (_brushType == BrushType.Circle)
            {
                DrawCircle(center);
            }
        }
    }

    private void DrawQuad(Vector2 center)
    {
        for (int x = 0; x < _brushSize; x++)
        {
            for (int y = 0; y < _brushSize; y++)
            {
                _textureBackground.SetPixel((int) (center.x + x), (int) (center.y + y), _color); //verx pravo
            }
        }
        _textureBackground.Apply();
    }

    private void DrawCircle(Vector2 center)
    {
        for (int x = 0; x < _brushSize; x++)
        {
            for (int y = 0; y < _brushSize; y++)
            {
                var x2 = Mathf.Pow(center.normalized.x, 2);
                var y2 = Mathf.Pow(center.normalized.y, 2);
                //todo 
                if (x2 + y2 < 1)
                {
                    _textureBackground.SetPixel((int) center.x+x,(int) center.y+y,_color);
                }
            }
        } 
        _textureBackground.Apply();
    }
}

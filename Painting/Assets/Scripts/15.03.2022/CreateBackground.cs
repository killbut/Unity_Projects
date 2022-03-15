using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CreateBackground : MonoBehaviour
{
    [SerializeField] private Sprite _picture;
    [SerializeField] private Camera _camera;
    [SerializeField] private SpriteRenderer _spriteRender;
    
    [Range(0,1024)]
    [SerializeField] private int _widthPixel;
    [Range(0,1024)]
    [SerializeField] private int _heightPixel;
    
    private RectTransform _rectTransform;
    private Texture2D _backgroundForPaint;

    public Texture2D Background => _backgroundForPaint;
    public int DeltaY { get; private set; }
    void Start()
    {
        _widthPixel = Screen.width;
        _heightPixel = Mathf.FloorToInt(Screen.height * 0.8f);
        DeltaY = Mathf.FloorToInt(Screen.height*0.05f);
        gameObject.transform.position = new Vector3(0, DeltaY, 0);
        CreateBackgroundTexture();
    }

    protected void OnValidate()
    {
        if (_backgroundForPaint != null)
        {
            if (_backgroundForPaint.width != _widthPixel || _backgroundForPaint.height != _heightPixel)
            {
                _backgroundForPaint.Resize(_widthPixel, _heightPixel);
            }
        }
       
    }

    private void CreateBackgroundTexture()
    {
        var pivotCenter = new Vector2(0.5f, 0.5f);
        byte white = 0xff;
        
        _backgroundForPaint = new Texture2D(_widthPixel, _heightPixel,TextureFormat.RGBA32,false);
        _backgroundForPaint.filterMode = FilterMode.Point;
        _backgroundForPaint.wrapMode = TextureWrapMode.Clamp;
        _spriteRender.sprite=Sprite.Create(_backgroundForPaint,
                                            new Rect(0,0,_widthPixel,_heightPixel),
                                            pivotCenter);
        
        var solidColor = CreateSolidBackground(_widthPixel, _heightPixel, white);
        _backgroundForPaint.LoadRawTextureData(solidColor);
        _backgroundForPaint.Apply();
    }

    private byte[] CreateSolidBackground(int width,int height,byte color)
    {
        byte[] background = new byte[width*height*4];
        for (int i = 0; i < background.GetLength(0); i++)
        {
            background[i] = color;
        }
        return background;
    }

    
}

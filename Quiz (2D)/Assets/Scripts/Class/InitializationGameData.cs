using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


public class InitializationGameData
{
    private const int FIRST_LAYER = 2;
    private const int SECOND_LAYER = 1;
    private const float SIZE_BACKGROUND = 0.9F;
    private const float SIZE_OBJECT = 0.5F;
    private const float RIGHT_ANGLE =-90f;
    
    private Sprite _background;
    private Sprite[] _spriteForRotate;
    private GameObject _Parent;
    public InitializationGameData(LevelInformationData level,Sprite background,Sprite[] spriteForRotate,GameObject ObjectWhoCall)
    {
        _background = background;
        _spriteForRotate = spriteForRotate;
        _Parent = ObjectWhoCall;
        InitEveryLevel(level);
        ChangeIsUsed(level.BundleData);
    }
    private void InitEveryLevel(LevelInformationData Level)
    {
        Level.GameField = new CellGrid[Level.Rows, Level.Columns];
        int id = 1;
        for (int y = 0; y < Level.GameField.GetLength(0); y++)
        {
            for (int x = 0; x < Level.GameField.GetLength(1); x++)
            {
                Level.GameField[y, x] = new CellGrid();
                SetValue(Level.GameField[y,x], x, y, id);
                InitializationGameObject(Level.GameField[y,x],Level.BundleData);
                id++;
            }
        }
    }
    private void SetValue(CellGrid cell,int x, int y, int id)
    {
        cell.Column = x;
        cell.Row = y;
        cell.Id = id;
    }
    private void InitializationGameObject(CellGrid cell,SkinBundleData bundle)
    {
        cell.Value = new GameObject();
        cell.Value.transform.SetParent(_Parent.transform);
        cell.Value.transform.localScale = new Vector3(SIZE_OBJECT, SIZE_OBJECT, 0);
        var number = RandomSelect(bundle);
        var spriteRenderer = cell.Value.AddComponent<SpriteRenderer>();
        SetSprite(spriteRenderer,number,bundle.ValueData[number]);
        cell.Name=cell.Value.name = bundle.ValueData[number].Identifier;
        SetBackground(cell.Value,cell.Background);
        cell.Value.AddComponent<BoxCollider2D>();
        cell.Value.AddComponent<AudioSource>();
    }
    private void SetSprite(SpriteRenderer value,int number,SkinSprite skin)
    {
        value.sprite = skin.Sprite;
        value.sortingOrder = FIRST_LAYER;
        for (int i = 0; i < _spriteForRotate.GetLength(0); i++)
        {
            if (skin.Sprite.name == _spriteForRotate[i].name)
               RotateSprite(value);
        }
    }
    private void SetBackground(GameObject parent,GameObject child)
    {
        child = new GameObject();
        child.name = "Background for " + parent.name;
        child.transform.localScale = new Vector3(SIZE_BACKGROUND, SIZE_BACKGROUND, 0);
        var spriteRender = child.AddComponent<SpriteRenderer>();
        var number=Random.Range(1, 5);
        spriteRender.sprite = _background;
        spriteRender.sortingOrder = SECOND_LAYER;
        SetColorBackground(spriteRender,number);
        child.transform.SetParent(parent.transform);
    }

    private void SetColorBackground(SpriteRenderer spriteRenderer,int number)
    {
        switch (number)
        {
            case 1:
                spriteRenderer.color=Color.magenta;
                return;
            case 2:
                spriteRenderer.color=Color.yellow;
                return;
            case 3:
                spriteRenderer.color=Color.cyan;
                return;
            case 4:
                spriteRenderer.color=Color.white;
                return;
            case 5:
                spriteRenderer.color=Color.blue;
                return;
        }
    }
    private int RandomSelect(SkinBundleData bundle)
    {
        System.Random random = new System.Random();
        while (true)
        {
            var number = random.Next(0,bundle.ValueData.GetLength(0));
            if (!bundle.ValueData[number].IsUsed)
            {
                bundle.ValueData[number].IsUsed = true;
                return number;
            }
        }
    }

    private void ChangeIsUsed(SkinBundleData bundle)
    {
        for (int i = 0; i < bundle.ValueData.GetLength(0); i++)
        {
            bundle.ValueData[i].IsUsed = false;
        }
    }

    private void RotateSprite(SpriteRenderer sprite)
    {
        sprite.gameObject.transform.Rotate(0,0,RIGHT_ANGLE);
    }
}

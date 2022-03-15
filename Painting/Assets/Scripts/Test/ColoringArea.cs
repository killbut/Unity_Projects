using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ColoringArea : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private SpriteRenderer _sr;
    
    private List<GameObject> affiliation;
    private SpriteRenderer _currentSprite;
    private Texture2D _texture;
    protected void Start()
    {
        _camera=Camera.main;
        affiliation = new List<GameObject>();
    }

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mouseGlobalPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            RayToSides(mouseGlobalPos);
            var texture = _currentSprite.sprite.texture;
            var pixel = texture.GetPixel((int) mouseGlobalPos.x, (int) mouseGlobalPos.y);
            Debug.Log(pixel);
            
        }
    }

    private void RayToSides(Vector3 position)
    {
        CheckCollider(new Ray(position,Vector3.right));
        CheckCollider(new Ray(position,Vector3.up));
        CheckCollider(new Ray(position,Vector3.down));
        CheckCollider(new Ray(position,Vector3.left));
        IdentifySprite();
    }

    private void CheckCollider(Ray ray)
    {
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 10f);
        Debug.DrawRay(ray.origin,ray.direction,Color.red,2f);
        if (hit.collider != null)
        {
            affiliation.Add(hit.collider.gameObject);
        }
    }

    private void IdentifySprite()
    {
        var objectSelected = affiliation.Max();
        _currentSprite = objectSelected.GetComponent<SpriteRenderer>();
        affiliation.Clear();
        Debug.Log(_currentSprite);
    }
}
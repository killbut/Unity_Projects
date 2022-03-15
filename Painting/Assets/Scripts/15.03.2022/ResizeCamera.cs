using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResizeCamera : MonoBehaviour
{
    [SerializeField] private bool uniform = true;
    [SerializeField] private bool autoSetUniform = false;
    
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        if (uniform)
            SetUniform();
    }

    protected void LateUpdate()
    {
        if (autoSetUniform && uniform)
            SetUniform();
    }
    private void SetUniform()
    {
        float orthographicSize = _camera.pixelHeight/2;
        if (!Mathf.Approximately( orthographicSize,_camera.orthographicSize))
        {
            _camera.orthographicSize = orthographicSize;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckClick : MonoBehaviour
{
    [SerializeField] private RenderLevel _renderLevel;
    [SerializeField] private GameObject _particle;
    [SerializeField] private AudioClip _hitAudio;
    [SerializeField] private AudioClip _wrongAudio;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name == _renderLevel.NeedFindObjectObject.name)
                {
                    HitObject();
                }
                else
                {
                   WrongObject(hit.collider.gameObject);
                }
            }
        }
    }

    private void HitObject()
    {
        DisableClick(_renderLevel.NeedFindObjectObject);
        UnlinkBackground(_renderLevel.NeedFindObjectObject.transform.GetChild(0));
        GameObject.Instantiate(_particle, _renderLevel.NeedFindObjectObject.transform);
        new AnimationObjects().Bounce(_renderLevel.NeedFindObjectObject,0.3f);
        PlayAudio(_renderLevel.NeedFindObjectObject.GetComponent<AudioSource>(),itHit:true);
        if (_renderLevel.NumberObjectInLevel != 0)
        {
            _renderLevel.SetFindObject();
            Debug.Log("hit");
        }
        else
        {
            StartCoroutine(_renderLevel.NextLevel());
        }
    }

    private void WrongObject(GameObject value)
    {
        DisableClick(value);
        var background = value.transform.GetChild(0);
        UnlinkBackground(background);
        new AnimationObjects().Snake(value,0.3f);
        PlayAudio(value.GetComponent<AudioSource>(),itHit:false);
        StartCoroutine(EnabledClick(value));
        StartCoroutine(LinkBackground(background, value.transform));
    }

    private void DisableClick(GameObject value)
    {
        value.GetComponent<BoxCollider2D>().enabled = false;
    }

    private IEnumerator EnabledClick(GameObject value)
    {
        yield return new WaitForSeconds(0.5f);
        value.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void UnlinkBackground(Transform value)
    {
       value.SetParent(this.gameObject.transform);
    }

    private IEnumerator LinkBackground(Transform value,Transform parent)
    {
        yield return new WaitForSeconds(0.5f);
        value.transform.SetParent(parent);
    }

    private void PlayAudio(AudioSource value,bool itHit)
    {
        if (itHit)
        {
            value.clip = _hitAudio;
            value.Play();
        }
        else
        {
            value.clip = _wrongAudio;
            value.Play();
        }
    }
}

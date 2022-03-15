using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private AudioSource _audio;
    // Start is called before the first frame update
    void Start()
    {
        _audio = gameObject.GetComponent<AudioSource>();
        _audio.volume = _slider.value;
        _slider.onValueChanged.AddListener(ChangeVolume);
    }
    private void ChangeVolume(float arg0)
    {
        _audio.volume = arg0;
    }

}

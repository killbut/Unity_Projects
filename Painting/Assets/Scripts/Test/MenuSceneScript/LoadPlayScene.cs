using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadPlayScene : MonoBehaviour
{
    private Button _button;
    // Start is called before the first frame update
    void Start()
    {
        _button = gameObject.GetComponent<Button>();
        _button.onClick.AddListener((() => { SceneManager.LoadScene("PlayScene");}));
    }
}

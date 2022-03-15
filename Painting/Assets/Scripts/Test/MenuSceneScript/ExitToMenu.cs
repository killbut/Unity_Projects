using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitToMenu : MonoBehaviour
{
    private Button _exitButton;
    // Start is called before the first frame update
    void Start()
    {
        _exitButton = gameObject.GetComponent<Button>();
        _exitButton.onClick.AddListener(NextScene);
    }

    private void NextScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}

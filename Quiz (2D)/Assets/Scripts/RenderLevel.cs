using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Class;
using DG.Tweening;
using DG.Tweening.Core;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Random = System.Random;

public class RenderLevel : MonoBehaviour
{
    private const float START_POSITION_X = -4f; // позиция с которой начинается построение сетки по х
    private const float DELTA_Y = 3f; // разница между ячейками по y

    [SerializeField] 
    // набор уровней , выбирается случайно
    private List<GameData> Game;
    [SerializeField] 
    private Sprite _backgroundCell; // задний фон ячейки ( он красится в initialgamedata)
    [SerializeField] 
    private Sprite[] _spriteForRotate; // Набор спрайтов которые нужно повернуть  на 90 градусов
    [SerializeField] 
    private TextMeshProUGUI _textMeshPro; // текст блок где будет высвечиваться что найти
    [SerializeField] 
    private Button _restartButton; // кнопка рестарта ,
    [SerializeField] 
    private Image _panel; // панель для имитации загрузки
    
    private int _startingLevel = 0;
    private GameData _gameData;
    private int[] _forPositionX = new int[3] {0, 1, -1};
    private int[] _forPositionY = new int[3] {0, -1, 1};
    private List<GameObject> _gameObjectsInLevel = new List<GameObject>();
    private GameObject _needFindObject;
    
    public GameData GameData => _gameData;
    public GameObject NeedFindObjectObject => _needFindObject;
    public int NumberObjectInLevel => _gameObjectsInLevel.Count;

    protected void Start()
    {
        _restartButton.gameObject.SetActive(false);
        _restartButton.onClick.AddListener(Restart);
        Game.Shuffle();
        _gameData = Game.First();
        if (_startingLevel == _gameData.Levels.GetLength(0))
        {
            _textMeshPro.text = "You win!";
            End();
        }
        else
        {
            new InitializationGameData(_gameData.Levels[_startingLevel], _backgroundCell, _spriteForRotate,this.gameObject);
            MoveObjects();
            SetFindObject();
        }
        //todo : расположение клик текстблок эффекты
    }
    
    
    /// <summary>
    /// Метод , который расствляет ячейки по сетке 1x3(2x3,3x3) 
    /// </summary>
    private void MoveObjects()
    {
        for (int y = 0; y < _gameData.Levels[_startingLevel].GameField.GetLength(0); y++)
        {
            for (int x = 0; x < _gameData.Levels[_startingLevel].GameField.GetLength(1); x++)
            {
                _gameObjectsInLevel.Add(_gameData.Levels[_startingLevel].GameField[y, x].Value);
                _gameData.Levels[_startingLevel].GameField[y, x].Value.transform.position =
                    new Vector3(START_POSITION_X * _forPositionX[x], _forPositionY[y]*DELTA_Y, 0);
                if(_startingLevel==0)
                    new AnimationObjects().Bounce(_gameData.Levels[_startingLevel].GameField[y, x].Value,0.3f);
            }
        }
        _gameObjectsInLevel.Shuffle();
    }

    /// <summary>
    /// Меняет текст у текстблока на обьект, который нужно найти
    /// </summary>
    public void SetFindObject()
    {
        _needFindObject = _gameObjectsInLevel.First();
        new AnimationObjects().FadeIn(_textMeshPro,0.3f);
        _textMeshPro.text = "Find: " + NeedFindObjectObject.name;
        _gameObjectsInLevel.Remove(NeedFindObjectObject);
    }
    /// <summary>
    /// После секунды , уровень меняется на другой
    /// </summary>
    /// <returns></returns>
    public IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1f);
        _startingLevel++;
        for (int i = 0; i < gameObject.transform.childCount; i ++ )
        {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }
        Start();
    }
    private void End()
    {
        new AnimationObjects().Fading(_panel,1f);
        _restartButton.gameObject.SetActive(true);
    }

    private void Restart()
    {
        float duration = 3f;
        _restartButton.gameObject.SetActive(false);
        _textMeshPro.gameObject.SetActive(false);
        new AnimationObjects().Restarting(_panel,duration);
        StartCoroutine(LoadScene(duration));
    }

    private IEnumerator LoadScene(float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
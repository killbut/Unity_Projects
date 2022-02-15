using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridCell
{
    public int ID { get; set; }
    public bool IsSelected { get; set; }
    public GameObject Value { get; set; }
    public int Column { get; set; }
    public int Row { get; set; }
    public bool IsDeleted { get; set; }
}

public class MainScript : MonoBehaviour
{
    public TMP_InputField NumberRowsInputField;
    public TMP_InputField NumberColumnsInputField;
    public TMP_InputField NumberScoreInputField;
    public TMP_InputField NumberColorInputField;
    public Button RestartButton;
    private int _rows;
    private int _columns;
    private int _numberColors;
    private bool[] canRestart = new bool[3] {true, true, true};
    private int _id;
    private int _score = 1;
    private GridCell[,] Grid;
    private float _deltaY;

    void Start()
    {
        float x, y, deltaX;
        var panel = GameObject.Find("Panel");
        var panelRect = panel.GetComponent<RectTransform>();
        InitializationField();
        CalculateGlobalXY(panelRect, out x, out y, out deltaX, out _deltaY);
        CreateGrid(panelRect, x, y, deltaX, _deltaY);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SquareGetClicked();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(SceneSwitch());
        }
    }

    #region Initialization

    /// <summary>
    /// Инициализация полей, добавление валидации к inputfield, добавление функции рестарта к кнопке 
    /// </summary>
    private void InitializationField()
    {
        if (PlayerPrefs.HasKey("Rows"))
        {
            _rows = PlayerPrefs.GetInt("Rows");
            _columns = PlayerPrefs.GetInt("Columns");
            _numberColors = PlayerPrefs.GetInt("Colors");
            NumberRowsInputField.text = _rows.ToString();
            NumberColumnsInputField.text = _columns.ToString();
            NumberColorInputField.text = _numberColors.ToString();
            PlayerPrefs.DeleteAll();
        }
        else
        {
            _rows = int.Parse(NumberRowsInputField.text);
            _columns = int.Parse(NumberColumnsInputField.text);
            _numberColors = int.Parse(NumberColorInputField.text);
        }

        AddValidation();
    }

    private void AddValidation()
    {
        RestartButton.onClick.AddListener(Restart);
        NumberRowsInputField.onEndEdit.AddListener(delegate(string text)
        {
            ValidationRowColumn(text, NumberRowsInputField, true);
        });
        NumberColumnsInputField.onEndEdit.AddListener(delegate(string text)
        {
            ValidationRowColumn(text, NumberColumnsInputField, false);
        });
        NumberColorInputField.onEndEdit.AddListener(delegate(string text) { ValidationColors(text); });
    }

    private void ValidationColors(string text)
    {
        int number = int.Parse(text);
        if (number > 1 && number < 6)
        {
            _numberColors = number;
            canRestart[2] = true;
        }
    }

    private void ValidationRowColumn(string text, TMP_InputField inputField, bool itsRow)
    {
        int number = int.Parse(text);
        var textInput = inputField.GetComponentsInChildren<TMPro.TextMeshProUGUI>()
            .First(c => c.gameObject.name == "Text");
        if (number < 51 && number > 9)
        {
            textInput.color = Color.black;
            inputField.text = text;
            if (itsRow)
            {
                _rows = number;
                canRestart[0] = true;
            }

            else
            {
                _columns = number;
                canRestart[1] = true;
            }

            Debug.Log("succesfull");
        }
        else
        {
            textInput.color = Color.red;
            Debug.Log("wrong numbers");
            canRestart[0] = false;
        }
    }

    private void Restart()
    {
        if (canRestart[0] == canRestart[1] == canRestart[2] == true)
        {
            PlayerPrefs.SetInt("Rows", _rows);
            PlayerPrefs.SetInt("Columns", _columns);
            PlayerPrefs.SetInt("Colors", _numberColors);
            PlayerPrefs.Save();
            Debug.Log("Restart");
            SceneManager.UnloadSceneAsync(0);
            SceneManager.LoadScene(0);
        }
        else
            Debug.Log("Check input field");
    }

    IEnumerator SceneSwitch()
    {
        SceneManager.UnloadSceneAsync("Scene");
        AsyncOperation load = SceneManager.LoadSceneAsync("Scene", LoadSceneMode.Additive);
        yield return load;
    }

    #endregion

    #region GameLogic
    /// <summary>
    /// Обработка клика по квардратам, поиск соседей, удаление и смещение вниз
    /// </summary>
    private void SquareGetClicked()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            int column, row;
            FindColumnRow(hit, out column, out row);
            List<move> direction = new List<move>();
            List<GridCell> foundSquares = new List<GridCell>();
            foundSquares.Add(Grid[row, column]);
            SearchSameSquare(row, column, foundSquares, direction);
            DeleteSquares(foundSquares);
        }
        else
            Debug.Log("not are square");
    }

    private void DeleteSquares(List<GridCell> foundSquares)
    {
        if (foundSquares.Count > 2)
        {
            _score += foundSquares.Count;
            NumberScoreInputField.text = _score.ToString();
            foreach (var item in foundSquares)
            {
                if (item != null)
                {
                    var spriteRender = (item.Value.GetComponent<SpriteRenderer>() as SpriteRenderer);
                    spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b,
                        0.5f);
                    Destroy(item.Value, 0.1f);
                    //StartCoroutine(StartDestroy(item.Value));
                    item.IsDeleted = true;
                    item.IsSelected = false;
                }
            }

            Invoke("Falling", 0.2f);
            foundSquares.Clear();
        }
        else
        {
            for (int i = 0; i < foundSquares.Count; i++) foundSquares[i].IsSelected = false;
        }
    }

    private void Falling()
    {
        for (int column = 0; column < Grid.GetLength(1); column++)
        {
            var rowComplete = false;
            for (int row = 0; row < Grid.GetLength(0) && !rowComplete; row++)
            {
                int firstRemote = row;
                if (Grid[row, column].IsDeleted)
                {
                    int lastExistSquare = 0;
                    for (int i = firstRemote + 1; i < Grid.GetLength(0); i++)
                    {
                        Grid[firstRemote, column].IsDeleted = true;
                        if (!Grid[i, column].IsDeleted)
                        {
                            Grid[i, column].IsDeleted = true;
                            Grid[firstRemote, column].Value = Grid[i, column].Value;
                            Grid[firstRemote, column].ID = Grid[i, column].ID;
                            var x = Grid[firstRemote, column].Value.transform.position.x;
                            StartCoroutine(StartFall(firstRemote, column, new Vector3(0, _deltaY), i - firstRemote));
                            Grid[firstRemote, column].IsDeleted = false;
                            firstRemote++;
                        }

                        lastExistSquare = i;
                    }

                    rowComplete = true;
                }
            }
        }

        UpdateGridValue();
    }

    private IEnumerator StartFall(int row, int column, Vector3 direction, int count)
    {
        var x = Grid[row, column].Value.transform.position.x;
        Grid[row, column].Value.transform.position -= direction * count;
        yield return new WaitForSeconds(0.3f);
    }

    private void UpdateGridValue()
    {
        for (int x = 0; x < Grid.GetLength(1); x++)
        {
            for (int y = 0; y < Grid.GetLength(0); y++)
            {
                if (Grid[y, x].IsDeleted && Grid[y, x].Value != null)
                {
                    Grid[y, x].Value = null;
                }
            }
        }
    }

    private void FindColumnRow(RaycastHit2D hit, out int column, out int row)
    {
        bool find = false;
        column = 0;
        row = 0;
        for (int i = 0; i < Grid.GetLength(0) && !find; i++)
        {
            for (int j = 0; j < Grid.GetLength(1) && !find; j++)
            {
                if (Grid[i, j].Value != null)
                {
                    if (Grid[i, j].Value.name == hit.collider.gameObject.name)
                    {
                        column = j;
                        row = i;
                        find = true;
                    }
                }
            }
        }
    }

    enum move
    {
        right,
        down,
        left,
        up
    }

    private void SearchSameSquare(int currentRow, int currentColumn, List<GridCell> foundSquares,
        List<move> previousDirection)
    {
        if (Grid[currentRow, currentColumn].IsDeleted != true)
        {
            Grid[currentRow, currentColumn].IsSelected = true;
            if (currentColumn < Grid.GetLength(1) - 1 && Grid[currentRow, currentColumn + 1].Value != null)
            {
                if (Grid[currentRow, currentColumn].ID == Grid[currentRow, currentColumn + 1].ID &&
                    !Grid[currentRow, currentColumn + 1].IsSelected)
                {
                    previousDirection.Add(move.right);
                    //foundSquares.Add(Grid[currentRow, currentColumn + 1].Value);
                    foundSquares.Add(Grid[currentRow, currentColumn + 1]);
                    SearchSameSquare(currentRow, currentColumn + 1, foundSquares, previousDirection);
                }
            }

            if (currentRow - 1 >= 0 && Grid[currentRow - 1, currentColumn].Value != null)
            {
                if (Grid[currentRow, currentColumn].ID == Grid[currentRow - 1, currentColumn].ID &&
                    !Grid[currentRow - 1, currentColumn].IsSelected)
                {
                    previousDirection.Add(move.down);
                    //foundSquares.Add(Grid[currentRow - 1, currentColumn ].Value);
                    foundSquares.Add(Grid[currentRow - 1, currentColumn]);
                    SearchSameSquare(currentRow - 1, currentColumn, foundSquares, previousDirection);
                }
            }

            if (currentColumn - 1 >= 0 && Grid[currentRow, currentColumn - 1].Value != null)
            {
                if (Grid[currentRow, currentColumn].ID == Grid[currentRow, currentColumn - 1].ID &&
                    !Grid[currentRow, currentColumn - 1].IsSelected)
                {
                    previousDirection.Add(move.left);
                    //foundSquares.Add(Grid[currentRow, currentColumn - 1].Value);
                    foundSquares.Add(Grid[currentRow, currentColumn - 1]);
                    SearchSameSquare(currentRow, currentColumn - 1, foundSquares, previousDirection);
                }
            }

            if (currentRow < Grid.GetLength(0) - 1 && Grid[currentRow + 1, currentColumn].Value != null)
            {
                if (Grid[currentRow, currentColumn].ID == Grid[currentRow + 1, currentColumn].ID &&
                    !Grid[currentRow + 1, currentColumn].IsSelected)
                {
                    previousDirection.Add(move.up);
                    //foundSquares.Add(Grid[currentRow + 1, currentColumn].Value);
                    foundSquares.Add(Grid[currentRow + 1, currentColumn]);
                    SearchSameSquare(currentRow + 1, currentColumn, foundSquares, previousDirection);
                }
            }

            if (previousDirection.Count > 0)
            {
                switch (previousDirection.LastOrDefault())
                {
                    case move.right:
                        previousDirection.RemoveAt(previousDirection.Count - 1);
                        SearchSameSquare(currentRow, currentColumn - 1, foundSquares, previousDirection);
                        break;
                    case move.down:
                        previousDirection.RemoveAt(previousDirection.Count - 1);
                        SearchSameSquare(currentRow + 1, currentColumn, foundSquares, previousDirection);
                        break;
                    case move.left:
                        previousDirection.RemoveAt(previousDirection.Count - 1);
                        SearchSameSquare(currentRow, currentColumn + 1, foundSquares, previousDirection);
                        break;
                    case move.up:
                        previousDirection.RemoveAt(previousDirection.Count - 1);
                        SearchSameSquare(currentRow - 1, currentColumn, foundSquares, previousDirection);
                        break;
                }
            }
            else
                return;
        }

        return;
    }

    #endregion

    #region CreateGameField
    /// <summary>
    /// Создание игрового поля, квардратов, вычисление координат, создание цветовой палитры
    /// </summary>

    private void CalculateGlobalXY(RectTransform field, out float x, out float y, out float deltaX, out float deltaY)
    {
        field.offsetMin = new Vector2(1000 / _columns, 200 / _rows);
        field.offsetMax = new Vector2(-1000 / _columns, field.offsetMax.y);
        var startGlobalPosition = field.transform.TransformPoint(new Vector3(field.pivot.x, field.pivot.y));
        var deltaWidthPixel = new Vector3(field.rect.xMax, field.rect.yMin) -
                              new Vector3(field.rect.xMin, field.rect.yMin);
        var deltaWidthInGlobal = field.transform.TransformVector(deltaWidthPixel);
        deltaX = deltaWidthInGlobal.x / _columns;
        var deltaHeightPixel = new Vector3(field.rect.xMin, field.rect.yMax) -
                               new Vector3(field.rect.xMin, field.rect.yMin);
        var deltaHeightGlobal = field.transform.TransformVector(deltaHeightPixel);
        deltaY = deltaHeightGlobal.y / _rows;
        x = startGlobalPosition.x;
        y = startGlobalPosition.y;
        /*Left rectTransform.offsetMin.x;
        Right rectTransform.offsetMax.x;
        Top rectTransform.offsetMax.y;
        Bottom rectTransform.offsetMin.y*/
        ;
    }

    private void CreateGrid(RectTransform panelRect, float x, float y, float deltaX, float deltaY)
    {
        Grid = new GridCell[_rows, _columns];
        var colorPalette = ColorPaletteCreate();
        var startX = x;
        int numberSquare = 1;
        for (int i = 0; i < _rows; i++, y += deltaY)
        {
            x = startX;
            for (int j = 0; j < _columns; j++, x += deltaX, numberSquare++)
            {
                int height = Mathf.FloorToInt(panelRect.rect.height / _rows);
                int width = Mathf.FloorToInt(panelRect.rect.width / _columns);

                Grid[i, j] = new GridCell();
                Grid[i, j].Value = CreateSquare(new Vector3(x, y, 0), height, width, numberSquare, colorPalette);
                Grid[i, j].ID = _id;
                Grid[i, j].Row = i;
                Grid[i, j].Column = j;
                //Debug.DrawLine(new Vector3(x, y), new Vector3(x + deltaX, y), Color.blue, 100f);
                //Debug.DrawLine(new Vector3(x, y), new Vector3(x, y + deltaY), Color.green, 100f);
            }
        }
    }

    private GameObject CreateSquare(Vector3 setPosition, int height, int width, int numberSquare, Color[] colorPalette)
    {
        int random = Random.Range(0, _numberColors);
        _id = random;

        GameObject obj = new GameObject($"square_number:{numberSquare}_id:{_id}", typeof(SpriteRenderer));
        obj.transform.position = setPosition;

        SpriteRenderer _spriteRender = obj.GetComponent<SpriteRenderer>();

        _spriteRender.color = colorPalette[random];
        _spriteRender.sprite = Sprite.Create(new Texture2D(width, height), new Rect(0, 0, width, height),
            new Vector2(0, 0), 104f);

        obj.AddComponent<BoxCollider2D>();
        return obj;
    }

    private Color[] ColorPaletteCreate()
    {
        int palette = Random.Range(1, 3);
        Color[] color = new Color[_numberColors];
        CreateColor(out color, palette);
        return color;
    }

    private void CreateColor(out Color[] color, int palette)
    {
        color = new Color[_numberColors];
        switch (palette)
        {
            case 1:
                for (int i = 0; i < color.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            color[i].r = 149f / 256f;
                            color[i].g = 94f / 256f;
                            color[i].b = 131f / 256f;
                            color[i].a = 1f;
                            break;
                        case 1:
                            color[i].r = 207f / 256f;
                            color[i].g = 184f / 256f;
                            color[i].b = 195f / 256f;
                            color[i].a = 1f;
                            break;
                        case 2:
                            color[i].r = 243f / 256f;
                            color[i].g = 232f / 256f;
                            color[i].b = 234f / 256f;
                            color[i].a = 1f;
                            break;
                        case 3:
                            color[i].r = 158f / 256f;
                            color[i].g = 192f / 256f;
                            color[i].b = 205f / 256f;
                            color[i].a = 1f;
                            break;
                        case 4:
                            color[i].r = 45f / 256f;
                            color[i].g = 98f / 256f;
                            color[i].b = 108f / 256f;
                            color[i].a = 1f;
                            break;
                    }
                }

                return;
            case 2:
                for (int i = 0; i < color.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            color[i].r = 1f;
                            color[i].g = 0f;
                            color[i].b = 0f;
                            color[i].a = 1f;
                            break;
                        case 1:
                            color[i].r = 0f;
                            color[i].g = 1f;
                            color[i].b = 0f;
                            color[i].a = 1f;
                            break;
                        case 2:
                            color[i].r = 0f;
                            color[i].g = 0f;
                            color[i].b = 1f;
                            color[i].a = 1f;
                            break;
                        case 3:
                            color[i].r = 1f;
                            color[i].g = 1f;
                            color[i].b = 0f;
                            color[i].a = 1f;
                            break;
                        case 4:
                            color[i].r = 1f;
                            color[i].g = 0f;
                            color[i].b = 1f;
                            color[i].a = 1f;
                            break;
                    }
                }

                return;
            case 3:
                for (int i = 0; i < color.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            color[i].r = 1f;
                            color[i].g = 1f;
                            color[i].b = 1f;
                            color[i].a = 1f;
                            break;
                        case 1:
                            color[i].r = 128f / 256f;
                            color[i].g = 0f;
                            color[i].b = 128f / 256f;
                            color[i].a = 1f;
                            break;
                        case 2:
                            color[i].r = 0f;
                            color[i].g = 1f;
                            color[i].b = 1f;
                            color[i].a = 1f;
                            break;
                        case 3:
                            color[i].r = 1f;
                            color[i].g = 165f / 256f;
                            color[i].b = 0f;
                            color[i].a = 1f;
                            break;
                        case 4:
                            color[i].r = 0f;
                            color[i].g = 100f / 256f;
                            color[i].b = 0f;
                            color[i].a = 1f;
                            break;
                    }
                }

                return;
        }
    }

    #endregion
}
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{   
    public GameObject obj;
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public int AddedPoints = 0;
    public int Points = 0;
    public int Record = 0; 
    public bool GameStarted;
    public bool WinScreen;
    public bool LoseScreen;
    public GameObject RestartScreen;
    public string resultTextTemp;

    [SerializeField]
    private readonly TextMeshProUGUI pointsText;
    [SerializeField]
    private readonly TextMeshProUGUI recordText;
    [SerializeField]
    public TextMeshProUGUI resultText;

    void UpdatePoints()
    {
        recordText.text = Record.ToString();
        pointsText.text = Points.ToString();
    }
    public void AddPoints(int points)
    {
        Points += points;
        if (Record <= Points)
            Record = Points;
        UpdatePoints();
    }
    public void TakeAwayPoints(int points)
    {
        if (Record == Points)
        {
            Points -= points;
            Record -= points;
        }
        else Points -= points;
        UpdatePoints();
    }
    void GameStart()
    {
        Field.Instance.GenerateField(true);
        GameStarted = true;
        WinScreen = false;
        LoseScreen = false;
        resultText.text = "";      
    }
    public void SaveGame()
    {
        Save();
    }
    public void Win()
    {
        GameStarted = false;
        WinScreen = true;
        resultText.text = "YOU WON!";
    }
    public void Lose()
    {
        GameStarted = false;
        LoseScreen = true;
        resultText.text = "YOU LOSE";
    }
    public void Restart()
    {
        GameStarted = false;
        Points = 0;
        UpdatePoints();
        RestartScreen.SetActive(false);
        GameStart();
    }

    public void DoNotRestart()
    {
        resultText.text = resultTextTemp;
        GameStarted = true;
        RestartScreen.SetActive(false);
    }
    public void TryRestart()
    {
        resultTextTemp = resultText.text;
        resultText.text = "";
        GameStarted = false;
        RestartScreen.SetActive(true);
    }

    public void Save()
    {
        var data = SaveManager.Load<SaveData.PlayerProfile>(KeyManager.Instance.GetKeyByFieldSize());
        data.playerRecord = Record;
        data.playerCurrentPoints = Points;

        int[,] field2D = new int[Field.FieldSize, Field.FieldSize];
        for (int i = 0; i < Field.FieldSize; i++)
            for (int j = 0; j < Field.FieldSize; j++)
            {
                field2D[i,j] = Field.Instance.Cells[i,j].Number;
            }

        switch (Field.FieldSize)
        {
            case 3: data.field3 = To1D(field2D);
                break;
            case 4: data.field4 = To1D(field2D);
                break;
            case 5: data.field5 = To1D(field2D);
                break;
        }

        data.GameStarted = GameStarted;
        data.resultText = resultText.text;
        data.FieldIsCreated = Field.Instance.FieldIsCreated;
        SaveManager.Save<SaveData.PlayerProfile>(KeyManager.Instance.GetKeyByFieldSize(), data);
    }
    void Load()
    {
        var data = SaveManager.Load<SaveData.PlayerProfile>(KeyManager.Instance.GetKeyByFieldSize());
        Record = data.playerRecord;
        Points = data.playerCurrentPoints;
        Field.Instance.FieldIsCreated = data.FieldIsCreated;
        UpdatePoints();

        GameStarted = data.GameStarted;
        resultText.text = data.resultText;

        int[] fieldToLoad = Field.FieldSize switch
        {
            3 => data.field3,
            4 => data.field4,
            5 => data.field5,
            _ => data.field4
        };

        int[,] field2D = To2D(fieldToLoad);
        Cell[,] field = new Cell[Field.FieldSize, Field.FieldSize];
        for (int i = 0; i < Field.FieldSize; i++)
            for (int j = 0; j < Field.FieldSize; j++)
            {         
                field[i,j] = obj.AddComponent<Cell>(); //NO ERROR
                field[i,j].X = i;
                field[i,j].Y = j;
                field[i,j].Number = field2D[i,j];
            }
        Field.Instance.LoadField(field);          
    }

    public static T[,] To2D<T>(T[] array1D)
        {
            int array2Dlenth = (int)Math.Sqrt(array1D.Length);
            T[,] array2D = new T[array2Dlenth, array2Dlenth];
            for (int i = 0; i < array2Dlenth; i++)
                for (int j = 0; j < array2Dlenth; j++)
                {
                    array2D[i, j] = array1D[i*array2Dlenth+j]; 
                }
            return array2D;
        }
    public static T[] To1D<T>(T[,] array2)
        {
            int array2Dlenth = (int)Math.Sqrt(array2.Length);
            T[] array = new T[array2.Length];
            for (int i = 0; i < array2Dlenth; i++)
            {
                for (int j = 0; j < array2Dlenth; j++)
                {
                    array[i * array2Dlenth + j] = array2[i, j];
                }
            }
            return array;
        }

    void Start()
    {
        if (SaveManager.Load<SaveData.PlayerProfile>(KeyManager.Instance.GetKeyByFieldSize()).FieldIsCreated)
        {
            Load();
        }
        if (GameStarted != true)
        {   
            GameStart();
        }
    }
    public void UpdateField()
    {
        for(int i =0; i<Field.FieldSize; i++)
            for(int j = 0; j<Field.FieldSize; j++)
                {
                    Field.Instance.Cells[i,j].UpdateCell();
                }
    }
    public void LoadMainMenu()
    {
        SwipeDetector.SwipeEvent -= LogicManager.Instance.OnSwipe;
        GameStarted = false;
        SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
    }
}


using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public static Field Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Application.targetFrameRate = 500;
    }

    [Header("Field Propetries")]
    public static int FieldSize = 3;
    public float CellLenth = 200;
    public float Gap = 20;
    public int InitialCellsCount = 2;
    public bool FieldIsCreated;

    public Cell cellPrefab;
    private readonly RectTransform rt;

    public Cell[,] Cells; 

    public void GenerateField(bool generateRandomCell)
    {
        switch (FieldSize)
        {
            case 3: CellLenth = 268;
                    Gap = 24;
                    cellPrefab = Resources.Load<Cell>("Cell3");
            break;
            case 4: CellLenth = 200;
                    Gap = 20;
                    cellPrefab = Resources.Load<Cell>("Cell");
            break;
            case 5: CellLenth = 162;
                    Gap = 15;
                    cellPrefab = Resources.Load<Cell>("Cell5");
            break;
        }
        Cells = new Cell[FieldSize, FieldSize];

        float startX = -450 + Gap + CellLenth/2;
        float startY = 450 - Gap - CellLenth/2;
        for (int i = 0; i<FieldSize; i++)
        {
            for (int k = 0; k<FieldSize; k++)
            {
                var cell = Instantiate(cellPrefab, transform, false);
                cell.transform.localPosition = new Vector2((startX + i*(Gap + CellLenth)), (startY - k*(Gap + CellLenth)));
                Cells[i, k] = cell;
                cell.SetCell(i, k, 0);
            }
        }

        if (generateRandomCell)
        {
            for (int i = 0; i<InitialCellsCount; i++)
            {
                GenerateRandomCell();
            }
        }

        FieldIsCreated = true;
        //Save();
    }
    public void GenerateRandomCell()
    {
        var emptyCells = new List<Cell>();

        for (int i = 0; i < FieldSize; i++)
            for (int j = 0; j < FieldSize; j++)
                if (Cells[i,j].IsNull)
                    emptyCells.Add(Cells[i,j]);

        if (emptyCells.Count == 0)
            return;
        
        System.Random rnd = new();
        int random = rnd.Next(0, emptyCells.Count);
        var cell = emptyCells[random];

        cell.SetCell(cell.X, cell.Y, UnityEngine.Random.Range(0, 10) == 0 ? 4 : 2, false);
        AnimManager.Instance.AppearAnimation(cell);
    }
    public void LoadField(Cell[,] field)
    {
        if (GameManager.Instance.GameStarted)
        {
        GenerateField(false);
        for (int i = 0; i < FieldSize; i++)
            for (int j = 0; j < FieldSize; j++)
                Cells[i,j].SetCell(field[i,j].X, field[i,j].Y, field[i,j].Number);
        }
        else GenerateField(true);
    }
    public Cell[,] SaveField()
    {
        Cell[,] field = new Cell[FieldSize,FieldSize];
        for (int i = 0; i < FieldSize; i++)
            for (int j = 0; j < FieldSize; j++)
            {
                Cell fieldCell = GameManager.Instance.obj.AddComponent<Cell>();
                fieldCell.X = Cells[i,j].X;
                fieldCell.Y = Cells[i,j].Y;
                fieldCell.Number = Cells[i,j].Number; 

                field[i,j] = fieldCell;
            }

        return field;
    }
}

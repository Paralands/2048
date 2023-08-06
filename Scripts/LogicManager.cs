using UnityEngine;

public class LogicManager : MonoBehaviour
{
    public Cell[,] fieldMoveBefore = new Cell[Field.FieldSize, Field.FieldSize];
    public Cell[,] fieldMoveAfter = new Cell[Field.FieldSize, Field.FieldSize];
    public static LogicManager Instance;
    public bool Moved = false;
    public bool MovedFieldBefore = false;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start() 
    {
        SwipeDetector.SwipeEvent += OnSwipe;
    }
    public void OnSwipe(Vector2 direction)
    {
        Move(direction);
    }
    private void Update() 
    {
        if (!Application.isMobilePlatform)
        {
            if(Input.GetKeyDown(KeyCode.A))
                Move(Vector2.left);
            if(Input.GetKeyDown(KeyCode.D))
                Move(Vector2.right);
            if(Input.GetKeyDown(KeyCode.W))
                Move(Vector2.up);
            if(Input.GetKeyDown(KeyCode.S))
                Move(Vector2.down);
        } 
    }
    private Cell FindCellToMove(Cell cell, Vector2 dir)
    {
        Cell cellToMove = null;
        int min = (dir.x > 0 || dir.y < 0) ? 0 : 1;
        int max = (dir.x > 0 || dir.y < 0) ? Field.FieldSize-1 : Field.FieldSize;
        for (int i = dir.x == 0 ? cell.Y : cell.X; i<max && i >= min; i = (dir.x > 0 || dir.y < 0) ? i+1 : i-1)
        {
            int index = (dir.x > 0 || dir.y < 0) ? 1 : -1;
            int XY = dir.x == 0 ? cell.X : cell.Y;
            if (dir.x == 0)
            {
                if (Field.Instance.Cells[XY, i+index].IsNull)
                {
                    cellToMove = Field.Instance.Cells[XY, i+index];
                    continue;
                }
            } 
            else if (Field.Instance.Cells[i+index, XY].IsNull)
            {
                cellToMove = Field.Instance.Cells[i+index, XY];
                continue;
            }  
        }
        return cellToMove;
    }
    private Cell FindCellToConnect(Cell cell, Vector2 dir)
    {
        Cell cellToConnect = null;
        int min = (dir.x > 0 || dir.y < 0) ? 0 : 1;
        int max = (dir.x > 0 || dir.y < 0) ? Field.FieldSize-1 : Field.FieldSize;
        for (int i = dir.x == 0 ? cell.Y : cell.X; i<max && i >= min; i = (dir.x > 0 || dir.y < 0) ? i+1 : i-1)
        {
            int index = (dir.x > 0 || dir.y < 0) ? 1 : -1;
            int XY = dir.x == 0 ? cell.X : cell.Y;
            if (dir.x == 0)
            {
                if (Field.Instance.Cells[XY, i+index].Number == cell.Number && Field.Instance.Cells[XY, i+index].IsConnectable)
                {
                    return Field.Instance.Cells[XY, i+index];
                }
                else if (!Field.Instance.Cells[XY, i+index].IsNull)
                    return null;
            } 
            else if (Field.Instance.Cells[i+index, XY].Number == cell.Number && Field.Instance.Cells[i+index, XY].IsConnectable)
            {
                return Field.Instance.Cells[i+index, XY];
            }
            else if (!Field.Instance.Cells[i+index, XY].IsNull)
                    return null;
        } 
        return cellToConnect;
    }
    private bool FieldCanBeMoved(Vector2 dir)
    {
        for (int i = 0; i < Field.FieldSize; i++)
        {
            for (int j = 0; j < Field.FieldSize; j++)
            {
                if(!Field.Instance.Cells[i,j].IsNull && (FindCellToConnect(Field.Instance.Cells[i,j], dir) || FindCellToMove(Field.Instance.Cells[i,j], dir)))
                    return true;                   
            }
        }
        return false;
    }
    public void Move(Vector2 dir)
    {
        AnimManager.Instance.StopAnimations();
        GameManager.Instance.AddedPoints = 0;

        if (FieldCanBeMoved(dir))
        {
            fieldMoveBefore = Field.Instance.SaveField();
            MovedFieldBefore = true;
        }

        int min = 0;
        int max = Field.FieldSize;
        for (int i = (dir.x == 1) ? Field.FieldSize-1 : 0; i<max && i >= min; i = (dir.x == 1) ? i-1 : i+1)
        {
            for (int j = (dir.y == -1) ? Field.FieldSize-1 : 0; j<max && j >= min; j = (dir.y == -1) ? j-1 : j+1)
            {
                if (!Field.Instance.Cells[i,j].IsNull)
                {
                    
                    if (FindCellToConnect(Field.Instance.Cells[i,j], dir) != null && Field.Instance.Cells[i,j].IsConnectable)
                    {
                        Field.Instance.Cells[i,j].Connect(FindCellToConnect(Field.Instance.Cells[i,j],dir));                       
                        continue;
                    }
                    if(FindCellToMove(Field.Instance.Cells[i,j], dir) != null)
                    {
                        Field.Instance.Cells[i,j].Move(FindCellToMove(Field.Instance.Cells[i,j], dir));
                        continue;
                    }    
                }
            }
        }
        MakeCellsConnectable();
        if(Moved)
            Field.Instance.GenerateRandomCell();   
        CheckField();
        GameManager.Instance.Save();
        Moved = false;
    }
    public void SpawnFieldBefore()
    {
        if (MovedFieldBefore)
        {
            AnimManager.Instance.StopAnimations();
            for (int i = 0; i < Field.FieldSize; i++)
            {
                for (int j = 0; j < Field.FieldSize; j++)
                {
                    Field.Instance.Cells[i,j].SetCell(fieldMoveBefore[i,j].X, fieldMoveBefore[i,j].Y, fieldMoveBefore[i,j].Number);
                }         
            }
        GameManager.Instance.TakeAwayPoints(GameManager.Instance.AddedPoints);
        MovedFieldBefore = false;
        GameManager.Instance.GameStarted = true;
        GameManager.Instance.LoseScreen = false;
        GameManager.Instance.WinScreen = false;
        GameManager.Instance.resultText.text = "";
        }     
    }
    private void MakeCellsConnectable()
    {
        for (int i = 0; i < Field.FieldSize; i++)
        {
            for (int j = 0; j < Field.FieldSize; j++)
            {
                Field.Instance.Cells[i,j].IsConnectable = true;
            }         
        }
    }
    private void CheckField()
    {
        for (int i = 0; i < Field.FieldSize; i++)
        {
            for (int j = 0; j < Field.FieldSize; j++)
            {
                if (Field.Instance.Cells[i,j].Number == 2048)
                {
                    GameManager.Instance.Win();
                    return;
                }
                if (Field.Instance.Cells[i,j].IsNull || 
                    FindCellToConnect(Field.Instance.Cells[i,j], Vector2.up) ||
                    FindCellToConnect(Field.Instance.Cells[i,j], Vector2.down) ||
                    FindCellToConnect(Field.Instance.Cells[i,j], Vector2.left) ||
                    FindCellToConnect(Field.Instance.Cells[i,j], Vector2.right))
                { 
                    return; 
                }
            }
        }
        GameManager.Instance.Lose();
    }
}

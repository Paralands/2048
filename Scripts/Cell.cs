using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Cell : MonoBehaviour
{   
    public int X {get; set;}
    public int Y {get; set;}
    public int Number = 0; 
    public bool IsNull;
    public bool IsConnectable = true;
    public Image CellImage;
    public TextMeshProUGUI CellText;
    
    public void SetCell(int x, int y, int number, bool update)
    {
        X = x; 
        Y = y;
        Number = number;
        IsNull = Number == 0;
        if (update)
        { 
            UpdateCell(); 
        }
    }

    public void SetCell(int x, int y, int number)
    {
        X = x; 
        Y = y;
        Number = number;
        UpdateCell();
    }

    public void UpdateCell()
    {
        IsNull = Number == 0;
        CellText.text = IsNull ? string.Empty : Number.ToString();
        if (Number == 2 || Number == 4)
        {
            CellText.color = ColorManager.Instance.NumberBlackColor;
        }
        else 
        {
            CellText.color = ColorManager.Instance.NumberWhiteColor;
        }

        CellImage.color = IsNull ? ColorManager.Instance.CellColors[0] : ColorManager.Instance.CellColors[Convert.ToInt32(Math.Log(Number, 2))];
    }

    public void Move(Cell cell)
    {
        if (cell.IsNull)
        {
            cell.Number = Number;
            cell.IsNull = false;

            Number = 0;
            IsNull = true;
            UpdateCell();
            
            AnimManager.Instance.MoveAnimation(this, cell, false);
            LogicManager.Instance.Moved = true;
        }
        else Debug.Log("Move Error");
    }

    public void Connect(Cell cell)
    {
        if (cell.Number == Number)
        {
            cell.Number = Number*2;
            Number = 0;
            UpdateCell();
            AnimManager.Instance.MoveAnimation(this, cell, true);
            cell.IsConnectable = false;

            GameManager.Instance.AddPoints(cell.Number);
            GameManager.Instance.AddedPoints += cell.Number;

            LogicManager.Instance.Moved = true;
        }
        else Debug.Log("Connect Error");
    }

    public Cell()
    {

    }

    public Cell(Cell cell)
    {
        X = cell.X; 
        Y = cell.Y;
        Number = cell.Number;      
    }
}
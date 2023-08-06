using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimManager : MonoBehaviour
{   
    public static AnimManager Instance;
    private List<CellAnim> cellsAnim;
    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
        DOTween.Init();
    }
    public CellAnim cellPrefab;

    private void Start() {
        cellsAnim = new List<CellAnim>();        
        switch (Field.FieldSize)
        {
            case 3: cellPrefab = Resources.Load<CellAnim>("CellAnim3");
            break;
            case 4: cellPrefab = Resources.Load<CellAnim>("CellAnim");
            break;
            case 5: cellPrefab = Resources.Load<CellAnim>("CellAnim5");
            break;
        }
    }

    public void MoveAnimation(Cell from, Cell to, bool areConnecting)
    {
        CellAnim cellAnim = Instantiate(cellPrefab, transform, false);
        cellsAnim.Add(cellAnim);
        cellAnim.Move(from, to, areConnecting);
    }

    public void AppearAnimation(Cell cell)
    {
        CellAnim cellAnim = Instantiate(cellPrefab, transform, false);
        cellsAnim.Add(cellAnim);
        cellAnim.Appear(cell);
    }

    public void StopAnimations()
    {
        for (int i = 0; i < cellsAnim.Count; i++)
        {
            cellsAnim[i]?.Destroy();
        }
        cellsAnim.Clear();
    }
}
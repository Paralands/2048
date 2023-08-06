using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System;

public class CellAnim : MonoBehaviour
{   
    Cell cellToUpdate;
    public Image CellImage;
    public TextMeshProUGUI CellText;
    readonly float moveTime = .1f;
    readonly float appearTime = .1f;
    Sequence sequence;

    private void SetCell(Cell cell)
    {   
        CellText.text = cell.IsNull ? string.Empty : cell.Number.ToString();
        if (cell.Number == 2 || cell.Number == 4)
        {
            CellText.color = ColorManager.Instance.NumberBlackColor;
        }
        else 
        {
            CellText.color = ColorManager.Instance.NumberWhiteColor;
        }

        CellImage.color = cell.Number>0 ? ColorManager.Instance.CellColors[Convert.ToInt32(Math.Log(cell.Number, 2))] : CellImage.color;
    }

    public void Move(Cell from, Cell to, bool areConnecting)
    {
        cellToUpdate = to;
        if (!areConnecting)
        {
            SetCell(to); //Setting cell to the "from" cell
        }
        else 
        {
            CellText.text = (to.Number/2).ToString();
            if ((to.Number/2) == 2 || (to.Number/2) == 4)
            {
                CellText.color = ColorManager.Instance.NumberBlackColor;
            }
            else 
            {
                CellText.color = ColorManager.Instance.NumberWhiteColor;
            }

            CellImage.color = (to.Number/2)>0 ? ColorManager.Instance.CellColors[Convert.ToInt32(Math.Log((to.Number/2), 2))] : CellImage.color;
        }

        transform.position = from.transform.position;

        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(to.transform.position, moveTime).SetEase(Ease.InOutQuad));

        if (areConnecting)
        {
            sequence.AppendCallback(() => SetCell(to)); //Setting cell to the "to" cell
            sequence.Append(transform.DOScale(1.2f, appearTime)); //Scale changing animation
            sequence.Append(transform.DOScale(1f, appearTime));
        }

        sequence.AppendCallback(() => {to.UpdateCell(); Destroy();});
    }
    
    public void Appear(Cell cell)
    {
        cellToUpdate = cell;
        transform.localScale = Vector2.zero;
        SetCell(cell);
        transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, cell.transform.position.z);

        sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(1.2f, appearTime*2));
        sequence.Append(transform.DOScale(1f, appearTime*2));

        sequence.AppendCallback(() => {cell.UpdateCell(); Destroy();});
    }
    public void Destroy()
    {
        cellToUpdate.UpdateCell();
        sequence.Kill();
        Destroy(gameObject);
    }
}

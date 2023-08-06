using UnityEngine;


public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance;
    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
    }
    public Color[] CellColors;
    public Color NumberBlackColor;
    public Color NumberWhiteColor;
}

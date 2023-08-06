using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance;
    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
    }
    public string GetKeyByFieldSize()
    {
        return Field.FieldSize switch
        {
            3 => FieldSize3Key,
            4 => FieldSize4Key,
            5 => FieldSize5Key,
            _ => FieldSize4Key,
        };
    }
    public static string FieldSize3Key = "FieldSize3Key";
    public static string FieldSize4Key = "FieldSize4Key";
    public static string FieldSize5Key = "FieldSize5Key";
}

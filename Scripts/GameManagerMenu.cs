using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManagerMenu : MonoBehaviour
{
    public void LoadField(int fieldSize)
    {
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        Field.FieldSize = fieldSize;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class ExitGame : MonoBehaviour
{
    private void OnMouseDown()
    {
        Application.Quit();
    }
}

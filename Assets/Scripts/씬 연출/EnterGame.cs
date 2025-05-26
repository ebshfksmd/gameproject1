using UnityEngine;

public class EnterGame : MonoBehaviour
{
    [SerializeField] GameObject thisMap;
    [SerializeField] GameObject map1;

    private void OnMouseDown()
    {
        thisMap.SetActive(false);
        map1.SetActive(true);
    }
}

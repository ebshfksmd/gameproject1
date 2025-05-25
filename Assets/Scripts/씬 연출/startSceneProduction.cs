using UnityEngine;

public class startSceneProduction : MonoBehaviour
{
    [SerializeField] Camera mainCamera;


    private void Awake()
    {
        mainCamera.orthographicSize = 1.5f;
        mainCamera.transform.position = new Vector3(0.52f, 1.81f, -10);
    }

    void Update()
    {
        if(mainCamera.orthographicSize<=5)
        {
            mainCamera.orthographicSize+=2f*Time.deltaTime;
        }

        if(mainCamera.transform.position.x>=0)
        {
            mainCamera.transform.position -= new Vector3(1, 0, 0)*Time.deltaTime/3.6f;
        }

        if (mainCamera.transform.position.y >= 0)
        {
            mainCamera.transform.position -= new Vector3(0, 1, 0) * Time.deltaTime;
        }
    }
}

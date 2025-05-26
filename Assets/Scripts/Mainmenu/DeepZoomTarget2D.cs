using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeepZoomTarget2D : MonoBehaviour
{
    public BoardZoomController zoomController;
    [Tooltip("Enable to show scrollbar when this object is clicked")]
    public bool showScrollbar = true;


    [Header("시작할때 거치는 보드인지 ")]
    [SerializeField] bool isStartBoard;
    [Header("게임 입장 버튼")]
    [SerializeField] GameObject gameEnterButton;



    void OnMouseDown()
    {
        zoomController.DeepZoomTo(transform, showScrollbar);
        if(isStartBoard )
        {
            gameEnterButton.SetActive(true);
        }
        else
        {
            gameEnterButton.SetActive(false);
        }
    }

}

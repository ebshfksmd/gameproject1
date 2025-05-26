using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeepZoomTarget2D : MonoBehaviour
{
    public BoardZoomController zoomController;
    [Tooltip("Enable to show scrollbar when this object is clicked")]
    public bool showScrollbar = true;


    [Header("�����Ҷ� ��ġ�� �������� ")]
    [SerializeField] bool isStartBoard;
    [Header("���� ���� ��ư")]
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

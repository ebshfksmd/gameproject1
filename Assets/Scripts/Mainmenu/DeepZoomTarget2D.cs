using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeepZoomTarget2D : MonoBehaviour
{
    public BoardZoomController zoomController;
    [Tooltip("Enable to show scrollbar when this object is clicked")]
    public bool showScrollbar = true;

    void OnMouseDown()
    {
        zoomController.DeepZoomTo(transform, showScrollbar);
    }
}

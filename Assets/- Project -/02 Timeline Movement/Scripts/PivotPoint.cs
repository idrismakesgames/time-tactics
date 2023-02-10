using UnityEngine;
using System.Collections.Generic;

public class PivotPoint : MonoBehaviour
{
    [SerializeField] private SpriteRenderer draggedSpriteRenderer;
    [SerializeField] private LineRenderer draggedLineRenderer;
    [SerializeField] private GameObject pivotChild;
    
    private int pivotIndexOnPath;
    private TimelineShip shipParent;
    private List<Vector3> shipLinePath;
    
    private void FixedUpdate()
    {
        if (TimelineController.Instance.SelectedPivot == this)
        {
            // Show ghost indications
            draggedSpriteRenderer.enabled = true;
            draggedLineRenderer.enabled = true;
            pivotChild.transform.position = TimelineController.Instance.MousePosition;
            
            // Draw Ghost Line
            shipLinePath[pivotIndexOnPath] = TimelineController.Instance.MousePosition;
            // Build ghost line from surrounding nodes
            List<Vector3> ghostLinePath = new List<Vector3>
            {
                shipLinePath[pivotIndexOnPath - 1],
                shipLinePath[pivotIndexOnPath]
            };
            if (shipLinePath.Count-1 > pivotIndexOnPath) ghostLinePath.Add(shipLinePath[pivotIndexOnPath+1] );
            // Apply to renderer
            draggedLineRenderer.positionCount = ghostLinePath.Count;
            draggedLineRenderer.SetPositions(ghostLinePath.ToArray());
        }
    }

    public void SetStartingValues(TimelineShip originShip, int pivotIndex, List<Vector3> originLinePath)
    {
        pivotIndexOnPath = pivotIndex;
        shipParent = originShip;
        shipLinePath = originLinePath;
    }

    private void OnMouseDrag()
    {
        if (TimelineController.Instance.HoveredPivot == this)
        {
            TimelineController.Instance.SelectedPivot = this;
        }
    }
    
    private void OnMouseUp()
    {
        // Update Path on parent ship
        Transform pivotTransform = transform;
        pivotTransform.position = pivotChild.transform.position;
        shipLinePath[pivotIndexOnPath] = pivotTransform.position;
        shipParent.UpdateShipLinePath(shipLinePath);
        draggedSpriteRenderer.enabled = false;
        draggedLineRenderer.enabled = false;
        TimelineController.Instance.SelectedPivot = null;
    }
}

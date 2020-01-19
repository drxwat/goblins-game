using UnityEngine;
using UnityEngine.EventSystems;

public class GridPainter : MonoBehaviour
{
    public GameObject tileHoverPrefab;
    public LayerMask walkableLayer;

    GameObject tileHover;
    Camera cam;
    Vector3 heightBias = new Vector3(0, 0.01f, 0);

    UnitManager unitManager;
    Unit hoverUnit;

    void Start()
    {
        cam = Camera.main;
        unitManager = UnitManager.instance;
        tileHover = Instantiate(tileHoverPrefab, new Vector3(0, 0.01f, 0), tileHoverPrefab.transform.rotation);
    }

    void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 100, walkableLayer))
        {
            return;
        }

        Node hitNode = Grid.instance.NodeFromWorldPosition(hit.point);
        if (hitNode.walkable && (tileHover.transform.position.x != hitNode.worldPosition.x || tileHover.transform.position.z != hitNode.worldPosition.z))
        {
            tileHover.transform.position = hitNode.worldPosition + heightBias;

            Unit unit = unitManager.GetUnitFromNode(hitNode);

            if (hoverUnit != null && hoverUnit == unitManager.GetSelectedUnit())
            {
                unitManager.onUnitLeave?.Invoke(hoverUnit);
                hoverUnit = null;
            }

            if (hoverUnit == unit || (unit != null && unit == unitManager.GetSelectedUnit()))
            {
                return;
            }

            if (unit != null)
            {
                if (hoverUnit != null)
                {
                    unitManager.onUnitLeave?.Invoke(hoverUnit);
                }
                hoverUnit = unit;
                unitManager.onUnitHover?.Invoke(unit);
            }
            else if (hoverUnit != null)
            {
                unitManager.onUnitLeave?.Invoke(hoverUnit);
                hoverUnit = null;
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(GridLayoutGroup))]
public class AdaptiveGrid : MonoBehaviour
{
    public GridLayoutGroup grid;
    public int columnsPhone = 2;
    public int columnsTablet = 3;

    void Start()
    {
        StartCoroutine(AdjustGridNextFrame());
    }

    IEnumerator AdjustGridNextFrame()
    {
        yield return new WaitForEndOfFrame();
        AdjustGrid();
    }

    void AdjustGrid()
    {
        float aspectRatio = (float)Screen.width / Screen.height;
        int columns = aspectRatio < 0.65f ? columnsPhone : columnsTablet;

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        float contentWidth = grid.GetComponent<RectTransform>().rect.width;

        float totalSpacing = grid.spacing.x * (columns - 1) + grid.padding.left + grid.padding.right;
        float cellWidth = (contentWidth - totalSpacing) / columns;

        grid.cellSize = new Vector2(cellWidth, cellWidth);
    }
}

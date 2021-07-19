// (c) https://github.com/qiankanglai/LoopScrollRect
// modified version by dragoff

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Loop Horizontal Scroll Rect", 50)]
    [DisallowMultipleComponent]
    public class LoopHorizontalScrollRect : LoopScrollRect
    {
        protected override float GetSize(RectTransform item)
        {
            float size = ContentSpacing;
            if (m_GridLayout != null)
            {
                size += m_GridLayout.cellSize.x;
            }
            else
            {
                size += LayoutUtility.GetPreferredWidth(item);
            }

            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return -vector.x;
        }

        protected override Vector2 GetVector(float value)
        {
            return new Vector2(-value, 0);
        }

        protected override void Awake()
        {
            direction = LoopScrollRectDirection.Horizontal;
            base.Awake();

            GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
            if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedRowCount)
            {
                Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
            }
        }

        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;

            // special case: handling move several page in one frame
            if (viewBounds.max.x < contentBounds.min.x && itemTypeEnd > itemTypeStart)
            {
                float currentSize = contentBounds.size.x;
                float elementSize = (currentSize - ContentSpacing * (CurrentLines - 1)) / CurrentLines;
                ReturnToTempPool(false, itemTypeEnd - itemTypeStart);
                itemTypeEnd = itemTypeStart;

                int offsetCount =
                    Mathf.FloorToInt((contentBounds.min.x - viewBounds.max.x) / (elementSize + ContentSpacing));
                if (TotalCount >= 0 && itemTypeStart - offsetCount * ContentConstraintCount < 0)
                {
                    offsetCount = Mathf.FloorToInt((float) (itemTypeStart) / ContentConstraintCount);
                }

                itemTypeStart -= offsetCount * ContentConstraintCount;
                if (TotalCount >= 0)
                {
                    itemTypeStart = Mathf.Max(itemTypeStart, 0);
                }

                itemTypeEnd = itemTypeStart;

                float offset = offsetCount * (elementSize + ContentSpacing);
                content.anchoredPosition -= new Vector2(offset + (ReverseDirection ? currentSize : 0), 0);
                contentBounds.center -= new Vector3(offset + currentSize / 2, 0, 0);
                contentBounds.size = Vector3.zero;

                changed = true;
            }

            if (viewBounds.min.x > contentBounds.max.x && itemTypeEnd > itemTypeStart)
            {
                int maxItemTypeStart = -1;
                if (TotalCount >= 0)
                {
                    maxItemTypeStart = Mathf.Max(0, TotalCount - (itemTypeEnd - itemTypeStart));
                    maxItemTypeStart = (maxItemTypeStart / ContentConstraintCount) * ContentConstraintCount;
                }

                float currentSize = contentBounds.size.x;
                float elementSize = (currentSize - ContentSpacing * (CurrentLines - 1)) / CurrentLines;
                ReturnToTempPool(true, itemTypeEnd - itemTypeStart);
                // TODO: fix with contentConstraint?
                itemTypeStart = itemTypeEnd;

                int offsetCount =
                    Mathf.FloorToInt((viewBounds.min.x - contentBounds.max.x) / (elementSize + ContentSpacing));
                if (maxItemTypeStart >= 0 && itemTypeStart + offsetCount * ContentConstraintCount > maxItemTypeStart)
                {
                    offsetCount = Mathf.FloorToInt((float) (maxItemTypeStart - itemTypeStart) / ContentConstraintCount);
                }

                itemTypeStart += offsetCount * ContentConstraintCount;
                if (TotalCount >= 0)
                {
                    itemTypeStart = Mathf.Max(itemTypeStart, 0);
                }

                itemTypeEnd = itemTypeStart;

                float offset = offsetCount * (elementSize + ContentSpacing);
                content.anchoredPosition += new Vector2(offset + (ReverseDirection ? 0 : currentSize), 0);
                contentBounds.center += new Vector3(offset + currentSize / 2, 0, 0);
                contentBounds.size = Vector3.zero;

                changed = true;
            }

            if (viewBounds.max.x < contentBounds.max.x - threshold)
            {
                float size = DeleteItemAtEnd(), totalSize = size;
                while (size > 0 && viewBounds.max.x < contentBounds.max.x - threshold - totalSize)
                {
                    size = DeleteItemAtEnd();
                    totalSize += size;
                }

                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.min.x > contentBounds.min.x + threshold)
            {
                float size = DeleteItemAtStart(), totalSize = size;
                while (size > 0 && viewBounds.min.x > contentBounds.min.x + threshold + totalSize)
                {
                    size = DeleteItemAtStart();
                    totalSize += size;
                }

                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.max.x > contentBounds.max.x)
            {
                float size = NewItemAtEnd(), totalSize = size;
                while (size > 0 && viewBounds.max.x > contentBounds.max.x + totalSize)
                {
                    size = NewItemAtEnd();
                    totalSize += size;
                }

                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.min.x < contentBounds.min.x)
            {
                float size = NewItemAtStart(), totalSize = size;
                while (size > 0 && viewBounds.min.x < contentBounds.min.x - totalSize)
                {
                    size = NewItemAtStart();
                    totalSize += size;
                }

                if (totalSize > 0)
                    changed = true;
            }

            if (changed)
            {
                ClearTempPool();
            }

            return changed;
        }
    }
}
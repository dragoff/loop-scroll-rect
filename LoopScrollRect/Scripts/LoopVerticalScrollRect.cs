// (c) https://github.com/qiankanglai/LoopScrollRect
// modified version by dragoff

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Loop Vertical Scroll Rect", 51)]
    [DisallowMultipleComponent]
    public class LoopVerticalScrollRect : LoopScrollRect
    {
        protected override float GetSize(RectTransform item)
        {
            float size = ContentSpacing;
            if (m_GridLayout != null)
            {
                size += m_GridLayout.cellSize.y;
            }
            else
            {
                size += LayoutUtility.GetPreferredHeight(item);
            }

            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return vector.y;
        }

        protected override Vector2 GetVector(float value)
        {
            return new Vector2(0, value);
        }

        protected override void Awake()
        {
            direction = LoopScrollRectDirection.Vertical;
            base.Awake();

            GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
            if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
            {
                Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
            }
        }

        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;

            // special case: handling move several page in one frame
            if (viewBounds.max.y < contentBounds.min.y && itemTypeEnd > itemTypeStart)
            {
                int maxItemTypeStart = -1;
                if (TotalCount >= 0)
                {
                    maxItemTypeStart = Mathf.Max(0, TotalCount - (itemTypeEnd - itemTypeStart));
                }

                float currentSize = contentBounds.size.y;
                float elementSize = (currentSize - ContentSpacing * (CurrentLines - 1)) / CurrentLines;
                ReturnToTempPool(true, itemTypeEnd - itemTypeStart);
                itemTypeStart = itemTypeEnd;

                int offsetCount =
                    Mathf.FloorToInt((contentBounds.min.y - viewBounds.max.y) / (elementSize + ContentSpacing));
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
                content.anchoredPosition -= new Vector2(0, offset + (ReverseDirection ? 0 : currentSize));
                contentBounds.center -= new Vector3(0, offset + currentSize / 2, 0);
                contentBounds.size = Vector3.zero;

                changed = true;
            }

            if (viewBounds.min.y > contentBounds.max.y && itemTypeEnd > itemTypeStart)
            {
                float currentSize = contentBounds.size.y;
                float elementSize = (currentSize - ContentSpacing * (CurrentLines - 1)) / CurrentLines;
                ReturnToTempPool(false, itemTypeEnd - itemTypeStart);
                itemTypeEnd = itemTypeStart;

                int offsetCount =
                    Mathf.FloorToInt((viewBounds.min.y - contentBounds.max.y) / (elementSize + ContentSpacing));
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
                content.anchoredPosition += new Vector2(0, offset + (ReverseDirection ? currentSize : 0));
                contentBounds.center += new Vector3(0, offset + currentSize / 2, 0);
                contentBounds.size = Vector3.zero;

                changed = true;
            }

            if (viewBounds.min.y > contentBounds.min.y + threshold)
            {
                float size = DeleteItemAtEnd(), totalSize = size;
                while (size > 0 && viewBounds.min.y > contentBounds.min.y + threshold + totalSize)
                {
                    size = DeleteItemAtEnd();
                    totalSize += size;
                }

                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.max.y < contentBounds.max.y - threshold)
            {
                float size = DeleteItemAtStart(), totalSize = size;
                while (size > 0 && viewBounds.max.y < contentBounds.max.y - threshold - totalSize)
                {
                    size = DeleteItemAtStart();
                    totalSize += size;
                }

                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.min.y < contentBounds.min.y)
            {
                float size = NewItemAtEnd(), totalSize = size;
                while (size > 0 && viewBounds.min.y < contentBounds.min.y - totalSize)
                {
                    size = NewItemAtEnd();
                    totalSize += size;
                }

                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.max.y > contentBounds.max.y)
            {
                float size = NewItemAtStart(), totalSize = size;
                while (size > 0 && viewBounds.max.y > contentBounds.max.y + totalSize)
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
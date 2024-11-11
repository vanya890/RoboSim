using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

namespace MG_BlocksEngine2.UI
{
    public class CustomScrollRect : ScrollRect
    {
        int maximumTouchCount = 2;

        public Vector2 MultiTouchPosition
        {
            get
            {
                Vector2 position = Vector2.zero;
                for (int i = 0; i < Input.touchCount && i < maximumTouchCount; i++)
                {
                    position += Input.touches[i].position;
                }
                position /= ((Input.touchCount <= maximumTouchCount) ? Input.touchCount : maximumTouchCount);

                return position;
            }
        }

        // v2.10 - custom scrollrect horizontal and vertical normal position are not reset on value change to enable zoom feature
        protected override void OnEnable() { }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnBeginDrag(eventData);
            }
            if (Input.touchCount >= maximumTouchCount)
            {
                eventData.position = MultiTouchPosition;
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnEndDrag(eventData);
            }
            if (Input.touchCount >= maximumTouchCount)
            {
                eventData.position = MultiTouchPosition;
                base.OnEndDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnDrag(eventData);
            }
            if (Input.touchCount >= maximumTouchCount)
            {
                eventData.position = MultiTouchPosition;
                base.OnDrag(eventData);
            }
        }

    }
}

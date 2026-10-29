using UnityEngine;
using UnityEngine.EventSystems;

namespace StinyPopela
{
    public sealed class TouchStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public PrototypeActor actor;
        public RectTransform knob;
        public void OnPointerDown(PointerEventData e) => OnDrag(e);
        public void OnDrag(PointerEventData e)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, e.position, e.pressEventCamera, out var point);
            var value = Vector2.ClampMagnitude(point / 65, 1);
            knob.anchoredPosition = value * 45;
            actor.touchMovement = value;
        }
        public void OnPointerUp(PointerEventData e) { actor.touchMovement = Vector2.zero; knob.anchoredPosition = Vector2.zero; }
        void OnDisable() { if (actor != null) actor.touchMovement = Vector2.zero; }
    }
}

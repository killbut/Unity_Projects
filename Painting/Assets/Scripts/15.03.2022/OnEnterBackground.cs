
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class OnEnterBackground :MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Enter to background");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Exit");
        }
    }

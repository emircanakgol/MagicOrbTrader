using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI
{
    public class InteractButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [FormerlySerializedAs("IsInteracting")] public bool isInteracting;
        private Image image;

        [Header("Prefabs")] 
        [SerializeField] private Sprite pressedSprite;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private TextMeshProUGUI interactTMP;

        private void Awake() {
            image = GetComponent<Image>();
        }

        public void OnPointerDown(PointerEventData eventData) {
            isInteracting = true;
            image.sprite = pressedSprite;
            interactTMP.color = new Color(0.3f, 0, 0.3f);
        }

        public void OnPointerUp(PointerEventData eventData) {
            isInteracting = false;
            image.sprite = normalSprite;
            interactTMP.color = Color.black;
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class StartButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameController gameController;
        
        public void OnPointerClick(PointerEventData eventData) {
            if(!gameController.ShopStarted)
                gameController.StartShop();
        }
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class OrderPanel : MonoBehaviour
    {
        [Header("Prefabs")] 
        [SerializeField] private Sprite bucketSprite;
        [SerializeField] private Sprite bubbleSprite;
        [SerializeField] private Sprite gemSprite;
    
        [Header("References")]
        [SerializeField] private Image orderImage;
        [SerializeField] private TextMeshProUGUI orderValueTMP;
        [SerializeField] private TextMeshProUGUI remainingTimeTMP;
        
        public Customer Customer { get; set; }

        private bool _notFirstEnable;

        private void OnEnable() {
            if (!_notFirstEnable) {
                _notFirstEnable = true;
                return;
            }
            Customer.OnWaitOneSecondCallback += SetRemainingTime;
        }

        private void OnDisable() {
            Customer.OnWaitOneSecondCallback -= SetRemainingTime;
        }

        private IEnumerator Start() {
            yield return new WaitUntil(()=>Customer != null);
            remainingTimeTMP.text = Customer.secondsToWait.ToString();
            Customer.OnWaitOneSecondCallback += SetRemainingTime;
            switch (Customer.CustomerOrderType) {
                case OrderType.BuyMana:
                    orderImage.sprite = bucketSprite;
                    break;
                case OrderType.BuyManaOrb:
                    orderImage.sprite = bubbleSprite;
                    break;
                case OrderType.BuyGem:
                    orderImage.sprite = gemSprite;
                    break;
            }
            orderValueTMP.text = Customer.Order.ToString();
        }

        private void SetRemainingTime() {
            remainingTimeTMP.text = (Customer.secondsToWait - Customer.secondsWaited).ToString();
        }
    }
}

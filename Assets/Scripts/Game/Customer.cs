using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public enum OrderType
    {
        BuyMana, BuyManaOrb, BuyGem
    }
    public class Customer : MonoBehaviour
    {
        //Queue Related
        public event Action OnJoinedQueueCallback;
        public event Action OnQueuePlaceChangedCallback;
        public event Action OnTopOfQueueCallback;
            
        //Order Related
        public event Action OnWaitOneSecondCallback;
        public event Action OnOrderCompletedCallback;
        public event Action OnOrderFailedCallback;
        
        [Header("References")] 
        [SerializeField] private GameObject billboardGO;
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshPro timeTMP;
        [SerializeField] private TextMeshPro orderTypeTMP;
        [SerializeField] private TextMeshPro orderTMP;
        public Renderer skinRenderer;

        [Header("Properties")] 
        public float secondsToWait;
        public int secondsWaited;
        
        private int _queuePlace = -1;
        public int QueuePlace {
            get => _queuePlace;
            set {
                if (_queuePlace == value) return;
                _queuePlace = value;
                OnQueuePlaceChangedCallback?.Invoke();
                if (_queuePlace == 0) {
                    OnTopOfQueueCallback?.Invoke();
                }
            }
        }
        
        public OrderType CustomerOrderType { get; private set; }
        public int Order { get; private set; }
        private static readonly int IsMovingAnim = Animator.StringToHash("IsMoving");
        public int CheckProperty {
            get {
                switch (CustomerOrderType) {
                    case OrderType.BuyMana:
                        return _gameController.house.Mana;
                    case OrderType.BuyManaOrb:
                        return _gameController.runestone.Bubbles;
                    case OrderType.BuyGem:
                        return _gameController.runestone.Gemstones;
                }
                return -1;
            }
        }
    
        private bool _setPositionOnStart;
        private bool _isOnPosition;
        
        private Tween _movingTween;

        private GameController _gameController;
        private List<Customer> _customerQueue;

        private void OnEnable() {
            OnJoinedQueueCallback += OnJoinedQueue;
            OnJoinedQueueCallback += UIController.Instance.UpdateRightPanel;
            OnQueuePlaceChangedCallback += OnQueuePlaceChanged;
            OnTopOfQueueCallback += OnTopOfQueue;
            OnWaitOneSecondCallback += OnWaitOneSecond;
            OnOrderCompletedCallback += OnOrderCompleted;
            OnOrderCompletedCallback += UIController.Instance.UpdateRightPanel;
            OnOrderFailedCallback += OnOrderFailed;
        }

        private void OnDisable() {
            OnJoinedQueueCallback -= OnJoinedQueue;
            OnJoinedQueueCallback -= UIController.Instance.UpdateRightPanel;
            OnQueuePlaceChangedCallback -= OnQueuePlaceChanged;
            OnTopOfQueueCallback -= OnTopOfQueue;
            OnWaitOneSecondCallback -= OnWaitOneSecond;
            OnOrderCompletedCallback -= OnOrderCompleted;
            OnOrderCompletedCallback -= UIController.Instance.UpdateRightPanel;
            OnOrderFailedCallback -= OnOrderFailed;
        }

        private void Start() {
            _gameController = GameController.Instance;;
            _customerQueue = _gameController.Customers;
            OnJoinedQueueCallback?.Invoke();
        }

        private static readonly Dictionary<int, int> _manaOrbWeightedRandom 
            = new () { { 0, 2 }, { 1, 5 } };
        private static readonly Dictionary<int, int> _gemWeightedRandom 
            = new () { { 0, 1 }, { 1, 6 }, {2, 5} };

        private void RandomOrderType() {
            switch (_gameController.currentAge) {
                case 0:
                    CustomerOrderType = OrderType.BuyMana;
                    break;
                case 1:
                    CustomerOrderType = (OrderType)GameController.RandomIntWithWeight(_manaOrbWeightedRandom);
                    break;
                case 2:
                    CustomerOrderType = (OrderType)GameController.RandomIntWithWeight(_gemWeightedRandom);
                    break;
            }
        }
        
        private void RefreshBillboard() {
            if (QueuePlace == 0) 
                timeTMP.gameObject.SetActive(true);
            else if (QueuePlace <= 2) {
                billboardGO.SetActive(true);
                timeTMP.gameObject.SetActive(false);
            }
            else {
                billboardGO.SetActive(false);
                return;
            }
            
            if(CustomerOrderType == OrderType.BuyMana)
                orderTypeTMP.text = "Liquid Mana";
            else if(CustomerOrderType == OrderType.BuyManaOrb)
                orderTypeTMP.text = "Mana Orb";
            else
                orderTypeTMP.text = "Gemstone";
        }

        private void RandomOrder() {
            var difficulty = _gameController.difficulty;
            var age = _gameController.currentAge;
            switch (CustomerOrderType) {
                case OrderType.BuyMana:
                    Order = difficulty * 20 + Random.Range(10 + difficulty * 4, 21 + difficulty * 4) + age * 5;
                    break;
                case OrderType.BuyManaOrb:
                    Order = Random.Range(difficulty - 1, 1 + difficulty + age);
                    break;
                case OrderType.BuyGem:
                    Order = Random.Range(difficulty - 3, difficulty - 1);
                    break;
            }
            
            orderTMP.text = Order.ToString();
        }
        
        private void SetQueuePlace() {
            timeTMP.text = secondsToWait.ToString();
            QueuePlace = _customerQueue.Count - 1;
        }
        
        private void MoveToPosition() {
            _isOnPosition = false;
            if(_movingTween.IsActive())
                _movingTween.Kill();
            animator.SetBool(IsMovingAnim, true);
            transform.forward = _gameController.PositionInQueue(QueuePlace) - transform.position;

            int secondsMultiplier;
            if (_setPositionOnStart)
                secondsMultiplier = 1;
            else {
                secondsMultiplier = 11 - QueuePlace;
                _setPositionOnStart = true;
            }
            
            _movingTween = transform.DOMove(
                _gameController.PositionInQueue(QueuePlace), 
                0.2f * (secondsMultiplier)).SetEase(Ease.Linear).OnComplete(FinishedMoving);
        }

        private void FinishedMoving() {
            _isOnPosition = true;
            animator.SetBool(IsMovingAnim, false);
            transform.forward = _gameController.queuePositions[QueuePlace].forward;
        }

        private void FinishedOrder() {
            _isOnPosition = false;
            switch (CustomerOrderType) {
                case OrderType.BuyMana:
                    _gameController.house.Mana -= Order;
                    _gameController.player.totalGainedGold += _gameController.house.manaPrice * Order;
                    _gameController.player.Golds += _gameController.house.manaPrice * Order;
                    break;
                case OrderType.BuyManaOrb:
                    _gameController.runestone.Bubbles -= Order;
                    _gameController.player.totalGainedGold += _gameController.runestone.bubblePrice * Order;
                    _gameController.player.Golds += _gameController.runestone.bubblePrice * Order;
                    break;
                case OrderType.BuyGem:
                    _gameController.runestone.Gemstones -= Order;
                    _gameController.player.totalGainedGold += _gameController.runestone.gemstonePrice * Order;
                    _gameController.player.Golds += _gameController.runestone.gemstonePrice * Order;
                    break;
            }

            UIController.Instance.UpdateLevelBar();

            if(_movingTween.IsActive())
                _movingTween.Kill();
            animator.SetBool(IsMovingAnim, true);
            var destination = _gameController.finishPos.position;
            transform.forward = destination - transform.position;
            _movingTween = transform.DOMove(
                destination, 
                2).SetEase(Ease.Linear).OnComplete(()=>Destroy(gameObject));
        }
        
        private IEnumerator WaitForOrder() {
            yield return new WaitUntil(()=>_isOnPosition);
            while (CheckProperty < Order) {
                yield return new WaitForSeconds(1);
                secondsWaited++;
                if (secondsWaited >= secondsToWait) {
                    OnOrderFailedCallback?.Invoke();
                    break;
                }
                OnWaitOneSecondCallback?.Invoke();
            }
            OnOrderCompletedCallback?.Invoke();
        }

        public void Ignore() {
            Order = 0;
            FinishedOrder();
        }
        
        //Base callbacks.
        private void OnJoinedQueue() {
            RandomOrderType();
            RandomOrder();
            _customerQueue.Add(this);
            SetQueuePlace();
        }

        private void OnQueuePlaceChanged() {
            MoveToPosition();
            RefreshBillboard();
        }

        private void OnTopOfQueue() {
            StartCoroutine(WaitForOrder());
        }

        private void OnWaitOneSecond() {
            timeTMP.text = (secondsToWait - secondsWaited).ToString();
        }

        private void OnOrderCompleted() {
            _customerQueue.Remove(this);
            _gameController.RefreshQueuePlaces();
            FinishedOrder();
        }

        private void OnOrderFailed() {
            _gameController.GameOver(true);
        }
    }
}

using System;
using UnityEngine;

namespace Game.Areas
{
    public class House : AreaBehaviour
    {
        public event Action OnManaIncreasedCallback;
        public event Action OnManaDecreasedCallback;
        public event Action OnMaxManaChangedCallback;

        [Header("References")] 
        [SerializeField] private PlayerController playerController;
        [SerializeField] private AnimationController animationController;
        [SerializeField] private SpriteRenderer progressSprite;
        
        [Header("Properties")] 
        public int bucketManaValue;
        public int manaPrice;
        
        protected override SpriteRenderer ProgressSprite { get; set; }
        protected override bool CheckProperty {
            get => playerController.Buckets <= 0 || Mana == MaxMana; 
        }
        
        private int _maxMana;
        public int MaxMana {
            get => _maxMana;
            set {
                if(_maxMana == value) return;
                _maxMana = value;
                OnMaxManaChangedCallback?.Invoke();
            }
        }
        private int _mana;
        public int Mana {
            get => _mana;
            set {
                if (_mana == value) return;
                if (_mana > value) {
                    _mana = value;
                    OnManaDecreasedCallback?.Invoke();
                }
                else {
                    _mana = value;
                    OnManaIncreasedCallback?.Invoke();
                }
            }
        }

        private void Awake() {
            ProgressSprite = progressSprite;
        }

        protected override void Start() {
            base.Start();
            MaxMana = 100;
        }

        protected override void OnAreaInteraction() {
            playerController.Bubbles = 0;
            animationController.Put(IsInteracting);
        }

        protected override void OnAreaInteractionTick() {
            playerController.Buckets--;
            if (Mana + bucketManaValue > MaxMana)
                Mana = MaxMana;
            else
                Mana += bucketManaValue;
        }
    }
}

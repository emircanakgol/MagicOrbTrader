using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Areas
{
    public class Runestone : AreaBehaviour
    {
        public event Action OnBubbleIncreasedCallback;
        public event Action OnBubbleDecreasedCallback;
        public event Action OnMaxBubbleChangedCallback;
        public event Action OnGemstoneIncreasedCallback;
        public event Action OnGemstoneDecreasedCallback;
        public event Action OnMaxGemstoneChangedCallback;

        [Header("References")] 
        [SerializeField] private AnimationController animationController;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SpriteRenderer progressSprite;
        [SerializeField] private VisualEffect bubbleVFX;
        
        [Header("Properties")] 
        public int bubbleValue;
        public int bubblePrice;
        public int gemstoneValue;
        public int gemstonePrice;
        protected override SpriteRenderer ProgressSprite { get; set; }
        
        protected override bool CheckProperty {
            get {
                if (playerController.Ingots > 0) 
                    return playerController.Ingots <= 0 || (playerController.Bubbles <= 0 && Bubbles <= 0) || Gemstones >= MaxGemstones;
                else
                    return playerController.Bubbles <= 0 || Bubbles >= MaxBubbles;
            }
        }
        
        [SerializeField] private int _maxBubbles;
        public int MaxBubbles {
            get => _maxBubbles;
            set {
                if(_maxBubbles == value) return;
                _maxBubbles = value;
                OnMaxBubbleChangedCallback?.Invoke();
            }
        }
        
        [SerializeField] private int _bubbles;
        public int Bubbles {
            get => _bubbles;
            set {
                if (_bubbles == value) return;
                if (_bubbles > value) {
                    _bubbles = value;
                    OnBubbleDecreasedCallback?.Invoke();
                }
                else {
                    _bubbles = value;
                    OnBubbleIncreasedCallback?.Invoke();
                }
            }
        }
        
        [SerializeField] private int _maxGemstones;
        public int MaxGemstones {
            get => _maxGemstones;
            set {
                if(_maxGemstones == value) return;
                _maxGemstones = value;
                OnMaxGemstoneChangedCallback?.Invoke();
            }
        }
        
        [SerializeField] private int _gemstones;
        public int Gemstones {
            get => _gemstones;
            set {
                if (_gemstones == value) return;
                if (_gemstones > value) {
                    _gemstones = value;
                    OnGemstoneDecreasedCallback?.Invoke();
                }
                else {
                    _gemstones = value;
                    OnGemstoneIncreasedCallback?.Invoke();
                }
            }
        }
        
        private void Awake() {
            ProgressSprite = progressSprite;
        }

        protected override void OnEnable() {
            base.OnEnable();
            OnBubbleIncreasedCallback += RefreshVFX;
            OnBubbleDecreasedCallback += RefreshVFX;
        }

        protected override void OnDisable() {
            base.OnDisable();
            OnBubbleIncreasedCallback -= RefreshVFX;
            OnBubbleDecreasedCallback -= RefreshVFX;
        }

        protected override void Start() {
            base.Start();
            MaxBubbles = 8;
            MaxGemstones = 8;
        }

        protected override void OnAreaInteraction() {
            playerController.Buckets = 0;
            animationController.Put(IsInteracting);
        }

        protected override void OnAreaInteractionTick() {
            if (playerController.Ingots > 0)
                GemstoneMode();
            else
                BubbleMode();
        }

        private void BubbleMode() {
            playerController.Bubbles--;
            if (Bubbles + bubbleValue > MaxBubbles)
                Bubbles = MaxBubbles;
            else
                Bubbles += bubbleValue;
        }
        
        private void GemstoneMode() {
            playerController.Ingots--;

            if (playerController.Bubbles <= 0) 
                Bubbles--;
            else
                playerController.Bubbles--;
            
            if (Gemstones + gemstoneValue > MaxGemstones)
                Gemstones = MaxGemstones;
            else
                Gemstones += gemstoneValue;
        }
        
        private void RefreshVFX() {
            bubbleVFX.SetInt("Count", Bubbles);
            bubbleVFX.Reinit();
        }
    }
}

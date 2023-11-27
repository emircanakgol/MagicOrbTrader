using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Game.Areas
{
    public abstract class AreaBehaviour : MonoBehaviour
    {
        public event Action OnAreaInteractionTickCallback;
        public event Action OnAreaInteractionCallback;
        
        private bool _isInteracting;
        protected bool IsInteracting {
            get => _isInteracting;
            private set {
                if(_isInteracting == value) return;
                _isInteracting = value;
                OnAreaInteractionCallback?.Invoke();
            }
        }
        
        private GameController _gameController;
        private PlayerController _playerController;
        private Coroutine _takingProcess;
        private Tweener _spriteTweener;
        
        protected abstract SpriteRenderer ProgressSprite { get; set; }
        protected abstract bool CheckProperty { get; }

        protected virtual void OnEnable() {
            OnAreaInteractionTickCallback += OnAreaInteractionTick;
            OnAreaInteractionCallback += OnAreaInteraction;
        }

        protected virtual void OnDisable() {
            OnAreaInteractionTickCallback -= OnAreaInteractionTick;
            OnAreaInteractionCallback -= OnAreaInteraction;
        }
        
        protected virtual void Start() {
            _gameController = GameController.Instance;
            _playerController = _gameController.player;
        }

        protected virtual void OnTriggerStay(Collider other) {
            if(_playerController == null) return;
            if(!other.CompareTag("Player")) return;
            IsInteracting = _playerController.isInteracting;
            if(IsInteracting && _gameController.ShopStarted)
                _takingProcess ??= StartCoroutine(Clock());
        }
        
        private IEnumerator Clock() {
            while (IsInteracting) {
                if (CheckProperty) {
                    _takingProcess = null;
                    yield break;
                }
                _spriteTweener = DOVirtual.Float(0, 
                    2.8f, 
                    1, 
                    SpriteTween).OnComplete(() => OnAreaInteractionTickCallback?.Invoke());
                yield return new WaitForSeconds(1);
            }
            _takingProcess = null;
        }

        private void SpriteTween(float x) {
            ProgressSprite.size = new Vector2(x, 1.8f);
            if (!IsInteracting) {
                _spriteTweener.Rewind();
                _spriteTweener.Kill();
            }
        }

        protected virtual void OnAreaInteractionTick() { }
        protected virtual void OnAreaInteraction() { } 
    }
}

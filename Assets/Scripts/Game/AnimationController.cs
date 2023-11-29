using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class AnimationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private MovementController movementController;
        [SerializeField] private Animator characterAnimator;
        
        public bool handMode;
        public bool holdMode;
        public bool isPutting;

        private bool isMoving;
        private static readonly int HandModeAnim = Animator.StringToHash("HandMode");
        private static readonly int IsMovingAnim = Animator.StringToHash("IsMoving");
        private static readonly int MovementSpeedMultiplierAnim = Animator.StringToHash("MovementSpeedMultiplier");
        private static readonly int IsPuttingAnim = Animator.StringToHash("IsPutting");
        private static readonly int PutAnim = Animator.StringToHash("Put");
        private static readonly int TakeAnim = Animator.StringToHash("Take");
        private static readonly int HoldingModeAnim = Animator.StringToHash("HoldingMode");

        private void OnEnable() {
            playerController.OnBucketAddedCallback += OnAddBucket;
            playerController.OnBucketRemovedCallback += OnRemoveBucket;
            playerController.OnBubbleAddedCallback += OnAddBubble;
            playerController.OnBubbleRemovedCallback += OnRemoveBubble;
            playerController.OnIngotAddedCallback += OnAddIngot;
            playerController.OnIngotRemovedCallback += OnRemoveIngot;
        }

        private void OnDisable() {
            playerController.OnBucketAddedCallback -= OnAddBucket;
            playerController.OnBucketRemovedCallback -= OnRemoveBucket;
            playerController.OnBubbleAddedCallback -= OnAddBubble;
            playerController.OnBubbleRemovedCallback -= OnRemoveBubble;
            playerController.OnIngotAddedCallback -= OnAddIngot;
            playerController.OnIngotRemovedCallback -= OnRemoveIngot;
        }

        private void Update() {
            WalkSpeed();
        }

        public void HandMode(bool _handMode) {
            if(holdMode) return;
            handMode = _handMode;
            characterAnimator.SetBool(AnimationController.HandModeAnim, handMode);
        }

        public void Walk(bool _moving) {
            isMoving = _moving;
            characterAnimator.SetBool(IsMovingAnim, isMoving);
        }

        private void WalkSpeed() {
            if(!isMoving) return;
            characterAnimator.SetFloat(MovementSpeedMultiplierAnim, movementController.movementSpeedProgress);
        }
        
        public void Put(bool _isPutting) {
            if(!handMode && !holdMode) return;
            isPutting = _isPutting;
            characterAnimator.SetBool(IsPuttingAnim, isPutting);
            if(isPutting)
                characterAnimator.SetTrigger(PutAnim);
        }
        
        public void Take(bool _asHandMode) {
            handMode = _asHandMode;
            holdMode = !_asHandMode;
            
            DOVirtual.Float(0,
                1,
                0.3f,
                (x) => characterAnimator.SetLayerWeight(1, x)).OnComplete(
                ()=>DOVirtual.Float(1,
                    0,
                    0.3f,
                    (x) => characterAnimator.SetLayerWeight(1, x)));
            characterAnimator.SetTrigger(TakeAnim);
        }

        public void HoldMode(bool _isHolding) {
            if(_isHolding)
                if(handMode) return;
            
            holdMode = _isHolding;
            characterAnimator.SetBool(HoldingModeAnim, holdMode);
        }

        private void OnAddBucket() {
            if (playerController.Buckets == 1) 
                HoldMode(true);
            Take(false);
        }
        
        private void OnRemoveBucket(bool removedAll) {
            if (!removedAll) return;
            Put(false);
            HoldMode(false);
        }
        
        private void OnAddBubble() {
            if (playerController.Bubbles == 1) 
                HandMode(true);
            Take(true);
        }
        
        private void OnRemoveBubble(bool removedAll) {
            if (!removedAll) return;
            Put(false);
            HandMode(false);
        }
        
        private void OnAddIngot() {
            if (playerController.Ingots == 1) 
                HoldMode(true);
            Take(false);
        }
        
        private void OnRemoveIngot(bool removedAll) {
            if (!removedAll) return;
            Put(false);
            HoldMode(false);
        }
    }
}

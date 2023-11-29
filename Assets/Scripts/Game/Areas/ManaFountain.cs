using UnityEngine;

namespace Game.Areas
{
    public class ManaFountain : AreaBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SpriteRenderer progressSprite;

        protected override SpriteRenderer ProgressSprite { get; set; }
        protected override bool CheckProperty {
            get => playerController.Buckets >= 3;
        }

        private void Awake() {
            ProgressSprite = progressSprite;
        }

        protected override void OnAreaInteractionTick() {
            playerController.Buckets++;
        }

        protected override void OnAreaInteraction() {
            playerController.Bubbles = 0;
            playerController.Ingots = 0;
        }
    }
}

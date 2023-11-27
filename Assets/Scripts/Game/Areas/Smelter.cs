using UnityEngine;

namespace Game.Areas
{
    public class Smelter : AreaBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SpriteRenderer progressSprite;

        protected override SpriteRenderer ProgressSprite { get; set; }
        protected override bool CheckProperty {
            get => playerController.Ingots >= 3;
        }

        private void Awake() {
            ProgressSprite = progressSprite;
        }

        protected override void OnAreaInteractionTick() {
            playerController.Ingots++;
            playerController.Golds -= 20;
        }

        protected override void OnAreaInteraction() {
            playerController.Buckets = 0;
        }
    }
}

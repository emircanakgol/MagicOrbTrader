using UnityEngine;

namespace Game.Areas
{
    public class Cauldron : AreaBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private House house;
        [SerializeField] private SpriteRenderer progressSprite;

        [Header("Properties")] 
        public int bubbleManaCost;
        protected override SpriteRenderer ProgressSprite { get; set; }
        protected override bool CheckProperty {
            get => playerController.Bubbles >= playerController.maxBubbleAtHand || house.Mana < bubbleManaCost;
        }

        private void Awake() {
            ProgressSprite = progressSprite;
        }

        protected override void OnAreaInteractionTick() {
            playerController.Bubbles++;
            house.Mana -= bubbleManaCost;
        }

        protected override void OnAreaInteraction() {
            playerController.Buckets = 0;
        }
    }
}

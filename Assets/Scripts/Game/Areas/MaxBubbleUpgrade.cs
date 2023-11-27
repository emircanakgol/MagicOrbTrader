using TMPro;
using UnityEngine;

namespace Game.Areas
{
    public class MaxBubbleUpgrade : AreaBehaviour
    {
        [Header("References")] 
        [SerializeField] private Runestone runestone;
        [SerializeField] private AnimationController animationController;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SpriteRenderer progressSprite;
        [SerializeField] private TextMeshPro upgradeText;
        [SerializeField] private GameObject upgradeMesh1;
        [SerializeField] private GameObject upgradeMesh2;
        [SerializeField] private GameObject upgradeMesh3;

        [Header("Properties")] 
        [SerializeField] private int upgradeLevel;
        [SerializeField] private int upgradeCost;
        
        protected override SpriteRenderer ProgressSprite { get; set; }
        protected override bool CheckProperty {
            get => (playerController.Golds < upgradeCost || upgradeLevel >= 3);
        }

        private void Awake() {
            ProgressSprite = progressSprite;
        }

        protected override void OnAreaInteractionTick() {
            if (upgradeLevel > 3) 
                return;
            
            upgradeLevel++;
            playerController.Golds -= upgradeCost;
            UpgradeMeshGroups();
            upgradeCost = upgradeLevel * 300 + upgradeCost;
            runestone.MaxBubbles += upgradeLevel * 2;
            if (upgradeLevel == 3) {
                animationController.Put(false);
                gameObject.SetActive(false);
                return;
            }
            upgradeText.text = "Max Mana Orbs +" + (upgradeLevel + 1) * 2 + " " + upgradeCost + "g";
        }
        
        protected override void OnAreaInteraction() {
            animationController.Put(IsInteracting);
        }
        
        //TODO Meshes
        private void UpgradeMeshGroups() {
            switch (upgradeLevel) {
                case 1:
                    upgradeMesh1.SetActive(true);
                    break;
                case 2:
                    upgradeMesh2.SetActive(true);
                    break;
                case 3:
                    upgradeMesh3.SetActive(true);
                    break;
            }
        }
    }
}

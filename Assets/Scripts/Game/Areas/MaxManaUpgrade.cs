using TMPro;
using UnityEngine;

namespace Game.Areas
{
    public class MaxManaUpgrade : AreaBehaviour
    {
        [Header("References")] 
        [SerializeField] private House house;
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
            upgradeCost = upgradeLevel * 200 + upgradeCost;
            house.MaxMana += upgradeLevel * 50;
            if (upgradeLevel == 3) {
                animationController.Put(false);
                gameObject.SetActive(false);
                return;
            }
            upgradeText.text = "Max Liquid Mana +" + (upgradeLevel + 1) * 50 + " " + upgradeCost + "g";
        }
        
        protected override void OnAreaInteraction() {
            animationController.Put(IsInteracting);
        }

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

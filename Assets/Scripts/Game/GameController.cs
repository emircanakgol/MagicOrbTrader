using System;
using System.Collections;
using System.Collections.Generic;
using Game.Areas;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        
        public event Action OnShopStartCallback;
        public event Action<string> OnShopStopCallback;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject customerPrefab;
        [SerializeField] private List<Material> customerMaterials;
        
        [Header("References")] 
        [Header("Customer References")]
        [SerializeField] private Transform spawnPos;
        public List<Transform> queuePositions;
        [SerializeField] private TextMeshPro pricesTMP;
        
        [Header("Gameplay References")]
        public Transform finishPos;
        public House house;
        public Runestone runestone;
        public Cauldron cauldron;
        public Smelter smelter;
        public MaxBubbleUpgrade bubbleUpgrade;
        [SerializeField] private TextMeshPro runestoneTMP;
        public PlayerController player;
        
        [Header("Tutorial References")]
        [SerializeField] private GameObject fountainTutorial;
        [SerializeField] private GameObject houseTutorial;
        [SerializeField] private GameObject runestoneTutorial1;
        [SerializeField] private GameObject cauldronTutorial;
        [SerializeField] private GameObject runestoneTutorial2;
        [SerializeField] private GameObject smelterTutorial;
        
        [Header("Properties")]
        public int currentAge;
        public int difficulty;
        
        // Customers[0] is top of the queue.
        public List<Customer> Customers { get; private set; }
        public bool ShopStarted { get; private set; }
        
        private bool _customerOverload;
        private Coroutine _newCustomersCoroutine;

        private float NewCustomerCooldown() {
            return difficulty switch {
                0 => 18,
                1 => 15,
                2 => 18,
                3 => 18,
                4 => 20,
                5 => 25,
                _ => 15
            };
        }

        private void OnEnable() {
            player.OnGoldAddedCallback += CheckDifficulty;
        }

        private void OnDisable() {
            player.OnGoldAddedCallback -= CheckDifficulty;
        }

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance ??= this;

            Customers = new List<Customer>();
        }

        private IEnumerator NewCustomerClock() {
            while (ShopStarted) {
                if (_customerOverload) {
                    GameOver(false);
                    yield break;
                }
                if (Customers.Count >= 11 && !_customerOverload) 
                    _customerOverload = true;
                
                CreateNewCustomer();
                yield return new WaitForSeconds(NewCustomerCooldown());
            }
        }

        private void CreateNewCustomer() {
            var customerGO = Instantiate(customerPrefab, 
                spawnPos.position, 
                Quaternion.identity,
                transform);
            var customer = customerGO.GetComponent<Customer>();
            customer.skinRenderer.sharedMaterial = customerMaterials[Random.Range(0, customerMaterials.Count)];
        }

        private void Start() {
            LiquidManaAge();
        }

        public void StartShop() {
            ShopStarted = true;
            OnShopStartCallback?.Invoke();
            _newCustomersCoroutine = StartCoroutine(NewCustomerClock());
            fountainTutorial.SetActive(false);
            houseTutorial.SetActive(false);
            runestoneTutorial1.SetActive(false);
            cauldronTutorial.SetActive(false);
            runestoneTutorial2.SetActive(false);
            smelterTutorial.SetActive(false);
        }
        
        public void StopShop(string reason) {
            if(_newCustomersCoroutine!=null)
                StopCoroutine(_newCustomersCoroutine);
            ShopStarted = false;
            foreach (var customer in Customers) {
                customer.Ignore();
            }
            Customers.Clear();
            OnShopStopCallback?.Invoke(reason);
        }

        public void RefreshQueuePlaces() {
            foreach (var customer in Customers) {
                customer.QueuePlace = Customers.IndexOf(customer);
            }
        }

        public readonly int[] DifficultyIncreaseGold = {
            120, 300, 600, 1000
        };
        private void CheckDifficulty() {
            switch (difficulty) {
                case 0:
                    if (player.totalGainedGold >= DifficultyIncreaseGold[0]) {
                        difficulty++;
                        house.manaPrice /= 2;
                        pricesTMP.text = "Liquid Mana: " + house.manaPrice + "g";
                    }
                    break;
                case 1:
                    if (player.totalGainedGold >= DifficultyIncreaseGold[1]) {
                        currentAge++;
                        ManaOrbAge();
                        StopShop("You have unlocked mana orbs. Go to the left side of the island to explore the new production process.\nPress start button to open your shop when you're ready.");
                        difficulty++;
                        pricesTMP.text = "Liquid Mana: " + house.manaPrice + "g\nMana Orb: " + runestone.bubblePrice + "g";
                    }
                    break;
                case 2:
                    if (player.totalGainedGold >= DifficultyIncreaseGold[2]) {
                        difficulty++;
                        runestone.bubblePrice /= 2;
                        pricesTMP.text = "Liquid Mana: " + house.manaPrice + "g\nMana Orb: " + runestone.bubblePrice + "g";
                    }
                    break;
                case 3:
                    if (player.totalGainedGold >= DifficultyIncreaseGold[3]) {
                        currentAge++;
                        GemstoneAge();
                        StopShop("You have unlocked gemstones. Go to the bottom side of the island to explore the new production process.\nYou can hold x2 more mana orbs at hand.\nPress start button to open your shop when you're ready.");
                        difficulty++;
                        player.maxBubbleAtHand *= 2;
                        runestone.bubblePrice /= 2;
                        pricesTMP.text = "Liquid Mana: " + house.manaPrice + "g\nMana Orb: " + runestone.bubblePrice + "g\nGemstone: 60g";
                    }
                    break;
            }
        }

        private void LiquidManaAge() {
            // Gameplay
            runestone.gameObject.SetActive(false);
            cauldron.gameObject.SetActive(false);
            smelter.gameObject.SetActive(false);
            bubbleUpgrade.gameObject.SetActive(false);
            
            // UI
            UIController.Instance.manaBar.SetActive(true);
            UIController.Instance.bubbleBar.SetActive(false);
            UIController.Instance.gemstoneBar.SetActive(false);
            runestoneTMP.text = "Put Mana Orb";
            runestoneTMP.fontSize = 4;
            
            // Tutorial
            fountainTutorial.SetActive(true);
            houseTutorial.SetActive(true);
            runestoneTutorial1.SetActive(false);
            cauldronTutorial.SetActive(false);
            runestoneTutorial2.SetActive(false);
            smelterTutorial.SetActive(false);
        }

        private void ManaOrbAge() {
            // Gameplay
            runestone.gameObject.SetActive(true);
            cauldron.gameObject.SetActive(true);
            bubbleUpgrade.gameObject.SetActive(true);
            smelter.gameObject.SetActive(false);
            
            // UI
            UIController.Instance.manaBar.SetActive(true);
            UIController.Instance.bubbleBar.SetActive(true);
            UIController.Instance.gemstoneBar.SetActive(false);
            runestoneTMP.text = "Put Mana Orb";
            runestoneTMP.fontSize = 4;
            
            // Tutorial
            fountainTutorial.SetActive(false);
            houseTutorial.SetActive(false);
            runestoneTutorial1.SetActive(true);
            cauldronTutorial.SetActive(true);
            runestoneTutorial2.SetActive(false);
            smelterTutorial.SetActive(false);
        }

        private void GemstoneAge() {
            // Gameplay
            runestone.gameObject.SetActive(true);
            cauldron.gameObject.SetActive(true);
            bubbleUpgrade.gameObject.SetActive(true);
            smelter.gameObject.SetActive(true);
            
            // UI
            UIController.Instance.manaBar.SetActive(true);
            UIController.Instance.bubbleBar.SetActive(true);
            UIController.Instance.gemstoneBar.SetActive(true);
            runestoneTMP.text = "Put Mana Orb or\nEnchant Gem with\nGold Ingot + Mana Orb";
            runestoneTMP.fontSize = 3;
            
            // Tutorial
            fountainTutorial.SetActive(false);
            houseTutorial.SetActive(false);
            runestoneTutorial1.SetActive(false);
            cauldronTutorial.SetActive(false);
            runestoneTutorial2.SetActive(true);
            smelterTutorial.SetActive(true);
        }

        public void GameOver(bool lostToLateOrder) {
            ShopStarted = false;
            Time.timeScale = 0;
            if(lostToLateOrder)
                UIController.Instance.GameOver("You have lost the game due to a late order.");
            else
                UIController.Instance.GameOver("You have lost the game due to queue being full.");
        }
        
        public static int RandomIntWithWeight(Dictionary<int, int> outputsWithWeight) {
            var totalWeight = 0;
            foreach (var oww in outputsWithWeight) {
                totalWeight += oww.Value;
            }
            var randomWeight = Random.Range(1, totalWeight + 1);

            var processedWeight = 0;
            foreach (var oww in outputsWithWeight) {
                processedWeight += oww.Value;
                if (randomWeight <= processedWeight)
                    return oww.Key;
            }
            return -1;
        }

        public Vector3 PositionInQueue(int queueIndex) {
            return queuePositions[queueIndex].position;
        }
    }
}

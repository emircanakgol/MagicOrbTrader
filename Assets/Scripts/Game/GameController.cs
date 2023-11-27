using System;
using System.Collections;
using System.Collections.Generic;
using Game.Areas;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private Transform spawnPos;
        [SerializeField] private TextMeshPro pricesTMP;
        [SerializeField] private TextMeshPro runestoneTMP;
        public Transform finishPos;
        public House house;
        public Runestone runestone;
        public Cauldron cauldron;
        public Smelter smelter;
        public PlayerController player;
        public List<Transform> queuePositions;
        
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
                        StopShop("You have unlocked mana orbs. Go to the left side of the island to explore the new production process.\nPress space to open your shop when you're ready.");
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
                        StopShop("You have unlocked gemstones. Go to the bottom side of the island to explore the new production process.\nYou can hold x2 more mana orbs at hand.\nPress space to open your shop when you're ready.");
                        difficulty++;
                        player.maxBubbleAtHand *= 2;
                        runestone.bubblePrice /= 2;
                        pricesTMP.text = "Liquid Mana: " + house.manaPrice + "g\nMana Orb: " + runestone.bubblePrice + "g\nGemstone: 60g";
                    }
                    break;
            }
        }

        private void LiquidManaAge() {
            runestone.gameObject.SetActive(false);
            cauldron.gameObject.SetActive(false);
            UIController.Instance.manaBar.SetActive(true);
            UIController.Instance.bubbleBar.SetActive(false);
            UIController.Instance.gemstoneBar.SetActive(false);
            smelter.gameObject.SetActive(false);
            runestoneTMP.text = "Put Mana Orb";
            runestoneTMP.fontSize = 4;
        }

        private void ManaOrbAge() {
            runestone.gameObject.SetActive(true);
            cauldron.gameObject.SetActive(true);
            UIController.Instance.manaBar.SetActive(true);
            UIController.Instance.bubbleBar.SetActive(true);
            UIController.Instance.gemstoneBar.SetActive(false);
            smelter.gameObject.SetActive(false);
            runestoneTMP.text = "Put Mana Orb";
            runestoneTMP.fontSize = 4;
        }

        private void GemstoneAge() {
            runestone.gameObject.SetActive(true);
            cauldron.gameObject.SetActive(true);
            UIController.Instance.manaBar.SetActive(true);
            UIController.Instance.bubbleBar.SetActive(true);
            UIController.Instance.gemstoneBar.SetActive(true);
            smelter.gameObject.SetActive(true);
            runestoneTMP.text = "Put Mana Orb or\nEnchant Gem with\nGold Ingot + Mana Orb";
            runestoneTMP.fontSize = 3;
        }

        public void GameOver(bool lostToLateOrder) {
            ShopStarted = false;
            if(lostToLateOrder)
                UIController.Instance.GameOver("You have lost the game due to a late order.");
            else
                UIController.Instance.GameOver("You have lost the game due to queue being full.");
        }

        public void RestartScene() {
            SceneManager.LoadScene("Game");
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

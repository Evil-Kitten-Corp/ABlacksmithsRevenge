using UnityEngine;

namespace Game_Systems
{
    public class ManaManager : MonoBehaviour
    {
        public static ManaManager Instance
        {
            get; 
            private set;
        }
    
        [SerializeField] private float maxMana = 100f;
        [SerializeField] private float currentMana;
        [SerializeField] private float manaRegenRate = 5f;
    
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            currentMana = maxMana;
            InvokeRepeating(nameof(RegenerateMana), 1f, 1f);
        }

        private void RegenerateMana()
        {
            currentMana = Mathf.Min(currentMana + manaRegenRate, maxMana);
        }

        public bool SpendMana(float amount)
        {
            if (currentMana >= amount)
            {
                currentMana -= amount;
                return true;
            }
            
            return false;
        }

        public void AddMana(float amount)
        {
            currentMana = Mathf.Min(currentMana + amount, maxMana);
        }
    }
}
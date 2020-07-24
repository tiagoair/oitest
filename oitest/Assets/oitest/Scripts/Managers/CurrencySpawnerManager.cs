using System;
using System.Collections.Generic;
using System.Linq;
using oitest.Scripts.Enums;
using oitest.Scripts.Mono;
using UnityEngine;
using Random = UnityEngine.Random;

namespace oitest.Scripts.Managers
{
    /// <summary>
    /// Manages the spawn of the currencies in the game.
    /// </summary>
    public class CurrencySpawnerManager : MonoBehaviour
    {
        #region Public Fields

        // Sigleton pattern used since there will only be one instance of it running
        // and to make it easy to access
        public static CurrencySpawnerManager Instance { get; protected set; }

        #endregion
        
        #region Serializable Private Fields

        // Minimum and maximum spawn times defined by the designer, currencies will spawn in 
        // a random time between these values
        [SerializeField] private float minSpawnTime;
        [SerializeField] private float maxSpawnTime;

        // Minimum and maximum values in the X axis that the currencies may spawn on as well as an
        // offset to handle correct floor positioning, all defined by the designer
        [SerializeField] private float minXSpawnPosition;
        [SerializeField] private float maxXSpawnPosition;
        [SerializeField] private Vector3 spawnOffsetPosition;
        
        // Maximum size of the pool of currencies
        [SerializeField] private int currencyPoolSize;

        // Reference to the currency prefab
        [SerializeField] private GameObject currencyReference;

        // References to all different currency sprites
        [SerializeField] private List<Sprite> spriteReferences;

        // Time that a currency will be active once it is spawned
        [SerializeField] private float currencyTimeToLive;
        
        
        #endregion

        #region Private Fields
        
        // List for a pool containing active currency objects
        private List<GameObject> _currencyActivePool;
        
        // List for a pool containing non active currency objects
        private List<GameObject> _currencyNonActivePool;

        // Variables that stores the time since the last spawned currency
        private float _currentTimeSinceLastSpawn;
        
        // Variable that stores the time for the next currency spawning
        private float _nextSpawnTime;

        // Accessor to check if the pool reached its maximum capacity
        private bool IsPoolFull => _currencyActivePool.Count + _currencyNonActivePool.Count >= currencyPoolSize;

        #endregion

        #region MonoBehaviour Callbacks

        /// <summary>
        /// Called when the scene loads or object is initialized, this sets the singleton Instance.
        /// </summary>
        /// <exception cref="SystemException">Called if the Singleton already exists</exception>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                throw new SystemException("CurrencySpawnManager Singleton already exists");
            }
            else
            {
                Instance = this;
            }
        }
        
        /// <summary>
        /// Start is called before the first frame update, this initializes the pool and sets the nextSpawnTime.
        /// </summary>
        void Start()
        {
            _currencyActivePool = new List<GameObject>();
            _currencyNonActivePool = new List<GameObject>();

            _nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        }

        /// <summary>
        /// Update is called once per frame, this calls the CountdownToSpawn method.
        /// </summary>
        void Update()
        {
            if(!GameManager.Instance.GameDone)
                CountdownToSpawn();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the sprite relative to the provided currency type.
        /// </summary>
        /// <param name="type">Provided currency type</param>
        /// <returns></returns>
        public Sprite GetCurrencySprite(CurrencyType type)
        {
            return spriteReferences[(int) type];
        }
        
        /// <summary>
        /// Activates a provided currency GameObject by removing it from the non active pool (if it was already a
        /// pooled object) and then generates a new currency type and position, passing them to the Currency's class
        /// own Activate method for initialization. It finishes by adding the object to the active pool.
        /// </summary>
        /// <param name="currency">GameObject provided for activation</param>
        /// <exception cref="SystemException">If by some reason a non pooled GameObject is provided when there's no
        /// more space in the pool, this exception is fired</exception>
        public void ActivateCurrency(GameObject currency)
        {
            if (_currencyNonActivePool.Contains(currency))
            {
                _currencyNonActivePool.Remove(currency);
            } 
            else if (IsPoolFull)
            {
                Destroy(currency);
                throw new SystemException("GameObject destroyed, Currency Pool is already full");
            }
            
            
            CurrencyType pick = (CurrencyType) Random.Range(0, (int)CurrencyType.COUNT);
            Vector3 newPosition = Vector3.right*Random.Range(minXSpawnPosition,maxXSpawnPosition) + spawnOffsetPosition;
            currency.GetComponent<Currency>().Activate(pick, currencyTimeToLive, newPosition, spriteReferences[(int)pick]);
            currency.SetActive(true);
            
            _currencyActivePool.Add(currency);
        }

        /// <summary>
        /// Deactivates a provided currency GameObject by removing it from the active pool and then calling the
        /// Currency's class own Deactivate method. It finishes by adding the object to the non active pool.
        /// </summary>
        /// <param name="currency">GameObject provided for deactivation</param>
        /// <exception cref="SystemException">If by some reason a non pooled GameObject is provided this exception
        /// is fired</exception>
        public void DeactivateCurrency(GameObject currency)
        {
            if (_currencyActivePool.Contains(currency))
            {
                _currencyActivePool.Remove(currency);
            }
            else
            {
                Destroy(currency);
                throw new SystemException("Currency isn't on pool");
            }
            
            currency.GetComponent<Currency>().Deactivate();
            currency.SetActive(false);
            
            _currencyNonActivePool.Add(currency);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Counts the time remaining to spawn the next currency and, when the countdown ends, get a new next spawn
        /// time and calls the SpawnCurrency method.
        /// </summary>
        private void CountdownToSpawn()
        {
            if (_currentTimeSinceLastSpawn <= _nextSpawnTime)
            {
                _currentTimeSinceLastSpawn += Time.deltaTime;
            }
            else
            {
                _currentTimeSinceLastSpawn = 0f;
                _nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
                
                SpawnCurrency();
            }
        }
        
        /// <summary>
        /// Spawns a currency by first checking if there's an object in the non active currency pool, if there is then
        /// it activates it, otherwise checks if the pool is already full, if it isn't then Instantiates a new object
        /// and activates it.
        /// </summary>
        private void SpawnCurrency()
        {
            if (_currencyNonActivePool.Any())
            {
                GameObject spawnedCurrency = _currencyNonActivePool[0];
                ActivateCurrency(spawnedCurrency);
            }
            else if(!IsPoolFull)
            {
                GameObject spawnedCurrency = Instantiate(currencyReference);
                ActivateCurrency(spawnedCurrency);
            }
            
            
        }

        #endregion
    }
}

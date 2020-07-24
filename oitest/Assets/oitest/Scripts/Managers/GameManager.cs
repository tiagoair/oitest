using System;
using System.Collections.Generic;
using System.Linq;
using oitest.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace oitest.Scripts.Managers
{
    /// <summary>
    /// Singleton that manages the game. 
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Public Fields

        // Instance declaration for the singleton pattern
        public static GameManager Instance { get; protected set; }

        #endregion
        
        #region Serializable Private Fields
        
        // List of the game's Items
        [SerializeField] private List<Item> items;

        // List that will store indexes for the random generation of Items
        [SerializeField] private List<int> shufflerList;
        
        #endregion

        #region Private Fields

        // Random variable for the random generation of Items
        private Random _myRng;

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
                throw new SystemException("GameManager Singleton already exists");
            }
            else
            {
                Instance = this;
            }
        }
        
        /// <summary>
        /// Called before the first frame update, initializes the random variable, the shuffler list and loads the
        /// UI scene as additive.
        /// </summary>
        void Start()
        {
            _myRng = new Random();

            for (int i = 0; i < items.Count; i++)
            {
                shufflerList.Add(i);
            }

            SceneManager.LoadSceneAsync(1,LoadSceneMode.Additive);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shuffles the shuffler array and returns the first number items of the shuffled list.
        /// </summary>
        /// <param name="number">Number of items to return</param>
        /// <returns></returns>
        public int[] GetShuffledItems(int number)
        {
            
            int[] shuffled = shufflerList.OrderBy(value => _myRng.Next()).Take(number).ToArray();

            return shuffled;
        }

        /// <summary>
        /// Returns an item corresponding to the specified index in the Items list.
        /// </summary>
        /// <param name="index">Requested index</param>
        /// <returns></returns>
        public Item GetItem(int index)
        {
            return items[index];
        }

        #endregion
    }
}

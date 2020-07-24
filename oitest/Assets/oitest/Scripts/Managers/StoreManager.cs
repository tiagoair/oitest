using System.Collections.Generic;
using oitest.Scripts.Mono;
using oitest.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace oitest.Scripts.Managers
{
    /// <summary>
    /// Manager for the store, takes care of validating and delivering purchases as well as showing and hiding the
    /// store window.
    /// </summary>
    public class StoreManager : MonoBehaviour
    {
        #region Serializable Private Fields

        // Store id
        [SerializeField] private int myId;
        
        // Quantity of minimum and maximum possible items in the store, defined by the designer
        [SerializeField] private int minItems;
        [SerializeField] private int maxItems;

        // Reference to the base store item ui object
        [SerializeField] private GameObject storeItemUiReference;

        // Reference to the panel that should be the parent of the store item ui objects
        [SerializeField] private GameObject storePanelReference;

        // Reference to the panel that has the whole store window
        [SerializeField] private GameObject storeReference;
        
        
        #endregion

        #region Private Fields

        // Number of items in the store
        private int _numItems;

        // List of Store Item UI objects in display
        private List<GameObject> _storeItems;
        
        #endregion
        
        #region MonoBehaviour Callbacks

        /// <summary>
        /// Called when the object is Enabled, this handles the subscription to GameObserverManager events
        /// relating to the store management.
        /// </summary>
        private void OnEnable()
        {
            GameObserverManager.StoreViewToggle += OnStoreViewToggle;
            GameObserverManager.RequestBuy += OnRequestBuy;
        }

        /// <summary>
        /// Called when the object is Disabled, this handles the unsubscription to GameObserverManager events
        /// relating to the store management.
        /// </summary>
        private void OnDisable()
        {
            GameObserverManager.StoreViewToggle -= OnStoreViewToggle;
            GameObserverManager.RequestBuy -= OnRequestBuy;
        }

        /// <summary>
        /// Called before the first frame update, it calls the store initialization method.
        /// </summary>
        void Start()
        {
            InitializeStore();
        }

        #endregion

        #region Delegate Methods
        
        /// <summary>
        /// Toggles the Store Window depending on the received status variable, also selects the leftmost item in
        /// display when the window is shown and sends the player the toggle through the Game Observer Manager
        /// for entering the Interact state of Browsing. When the window is hidden, it then requests that the player
        /// leaves the Browsing state.
        /// </summary>
        /// <param name="storeId">The id of the store that should be toggled</param>
        /// <param name="status">The desired status of the Store Window</param>
        private void OnStoreViewToggle(int storeId, bool status)
        {
            if (storeId == myId)
            {
                storeReference.SetActive(status);
                GameObserverManager.OnBrowsingToggle(status);
                if (_numItems > 0)
                {
                    EventSystem eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(null);
                    eventSystem.SetSelectedGameObject(_storeItems[0]);
                }
                
            }
        }
        
        /// <summary>
        /// Handles a buying request, if the received player currencies has the amount required to buy the selected
        /// product, the purchase is made and the item is delivered to the player through the Game Observer Manager,
        /// as well as removing the item from the store list along with the change for that purchase,
        /// it also selects the leftmost item if there are any left, if there aren't then the game ends.
        /// </summary>
        /// <param name="storeId">The id of the store for the purchase to be made on</param>
        /// <param name="playerCurrencies">The wallet of the player</param>
        private void OnRequestBuy(int storeId, int[] playerCurrencies)
        {
            if (myId == storeId)
            {
                GameObject currentItemGO = EventSystem.current.currentSelectedGameObject;
                Item currentItem = currentItemGO.GetComponent<StoreItemUI>().MyItem;
                int playerCurrency = playerCurrencies[(int) currentItem.currencyType];
                if (playerCurrency >= currentItem.cost)
                {
                    int playerChange = playerCurrency - currentItem.cost;
                    GameObserverManager.OnDeliverItem(currentItem, currentItem.currencyType, playerChange);
                    _storeItems.Remove(currentItemGO);
                    _numItems--;
                    Destroy(currentItemGO);
                    
                    if (_numItems > 0)
                    {
                        _storeItems[0].GetComponent<Selectable>().Select();
                    }
                    else
                    {
                        GameManager.Instance.GameDone = true;
                    }
                    
                }
            }
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Handles the initialization of the store by generating a list of random items and adding them to the
        /// storeItems list.
        /// </summary>
        private void InitializeStore()
        {
            _storeItems = new List<GameObject>();
            _numItems = Random.Range(minItems, maxItems+1);

            int[] storeItems = GameManager.Instance.GetShuffledItems(_numItems);

            for (int i = 0; i < storeItems.Length; i++)
            {
                _storeItems.Add(GetStoreItemUi(GameManager.Instance.GetItem(storeItems[i])));
            }
        }

        /// <summary>
        /// Creates a new Store Item UI game object from a given Item.
        /// </summary>
        /// <param name="item">Item to be passed to the new Store Item UI object</param>
        /// <returns></returns>
        private GameObject GetStoreItemUi(Item item)
        {
            GameObject newItem = Instantiate(storeItemUiReference, storePanelReference.transform);
            
            newItem.GetComponent<StoreItemUI>().Initialize(item);

            return newItem;
        }

        #endregion
    }
}

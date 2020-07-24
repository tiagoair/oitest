using System;
using System.Collections.Generic;
using oitest.Scripts.Mono;
using oitest.Scripts.ScriptableObjects;
using UnityEngine;

namespace oitest.Scripts.Managers
{
    /// <summary>
    /// Manager for the displaying of the player's items inventory.
    /// </summary>
    public class ItemPanelManager : MonoBehaviour
    {
        #region Serializable Private Fields

        // Reference for the base Item UI object
        [SerializeField] private GameObject itemUIReference;

        #endregion

        #region Private Fields

        // List of items in display
        private List<GameObject> _myItems;

        #endregion
        
        #region MonoBehaviour Callbacks

        /// <summary>
        /// Called when the object is Enabled, this handles the subscription to GameObserverManager events
        /// relating to adding a new item in the display.
        /// </summary>
        private void OnEnable()
        {
            GameObserverManager.AddItem += OnAddItem;
        }
        
        /// <summary>
        /// Called when the object is Disabled, this handles the unsubscription to GameObserverManager events
        /// relating to adding a new item in the display.
        /// </summary>
        private void OnDisable()
        {
            GameObserverManager.AddItem -= OnAddItem;
        }
        

        /// <summary>
        /// Called before the first frame update, initializes the _myItems list.
        /// </summary>
        void Start()
        {
            _myItems = new List<GameObject>();
        }

        #endregion

        #region Delegate Methods

        /// <summary>
        /// Adds a new item UI to the display by receiving an Item, instantiating a new object from the reference
        /// Item UI, then setting up the Icon using the Item UI's own method and then adding it to the _myItems list.
        /// </summary>
        /// <param name="item">Item to be displayed</param>
        private void OnAddItem(Item item)
        {
            GameObject newItem = Instantiate(itemUIReference, transform);
            
            newItem.GetComponent<ItemUI>().SetSprite(item.icon);
            
            _myItems.Add(newItem);
        }

        #endregion
    }
}

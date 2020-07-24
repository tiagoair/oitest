using System;
using oitest.Scripts.Enums;
using oitest.Scripts.ScriptableObjects;

namespace oitest.Scripts.Managers
{
    /// <summary>
    /// Observer pattern class that manages game events, serving as a main point for subscription and invocation of
    /// the desired events.
    /// </summary>
    public static class GameObserverManager
    {
        #region UI Events

        // Event called by the player and subscribed by the currency ui to update the number of currencies
        // according to the player's wallet.
        public static event Action<CurrencyType, int> UpdateCurrencies;

        public static void OnUpdateCurrencies(CurrencyType currency, int value)
        {
            UpdateCurrencies?.Invoke(currency,value);
        }

        // Event called by the player and subscribed by the item ui to update the current items in display according
        // to the player's inventory.
        public static event Action<Item> AddItem;

        public static void OnAddItem(Item item)
        {
            AddItem?.Invoke(item);
        }
        
        #endregion

        #region Store Events
        
        // Event called when something wants to close or open the store view, it is subscribed by the store manager
        // and it can be called by the store npc.
        public static event Action<int,bool> StoreViewToggle;

        public static void OnStoreViewToggle(int storeId, bool status)
        {
            StoreViewToggle?.Invoke(storeId, status);
        }
        
        // Event called to change the Interact state of the player to or from the Browsing state, it is subscribed by
        // the player and called by the store manager.
        public static event Action<bool> BrowsingToggle;

        public static void OnBrowsingToggle(bool status)
        {
            BrowsingToggle?.Invoke(status);
        }

        // Event called to request the purchase of an item to the store, it is subscribed by the store manager and
        // called byt the store front npc.
        public static event Action<int, int[]> RequestBuy;

        public static void OnRequestBuy(int storeId, int[] playerCurrencies)
        {
            RequestBuy?.Invoke(storeId, playerCurrencies);
        }

        // Event called to deliver an item to the player from the store, it is subscribed by the player and called
        // by the store manager
        public static event Action<Item, CurrencyType, int> DeliverItem;

        public static void OnDeliverItem(Item item, CurrencyType currencyType, int change)
        {
            DeliverItem?.Invoke(item, currencyType, change);
        }

        #endregion


    }
}

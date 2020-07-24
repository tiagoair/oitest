using oitest.Scripts.Abstracts;
using oitest.Scripts.Interfaces;
using oitest.Scripts.Managers;
using UnityEngine;

namespace oitest.Scripts.Mono
{
    /// <summary>
    /// Handles the NPC of the Store kind, it implements the ITalkable and IStoreFront interfaces.
    /// </summary>
    public class StoreNPC : NPC, ITalkable, IStoreFront
    {
        #region Serializable Private Fields

        // Reference to the Talk Icon
        [SerializeField] private GameObject talkIcon;
        
        #endregion

        #region Public Methods

        public void ToggleTalkIcon(bool state)
        {
            if(talkIcon.activeSelf != state)
                talkIcon.SetActive(state);
        }

        public void DoTalk()
        {
            ToggleStoreView(true);
        }
    
        public void ToggleStoreView(bool status)
        {
            // Calls the StoreManager trough the GameObserverManager for toggling the store window
            GameObserverManager.OnStoreViewToggle(myId, status);
        }

        public void RequestPurchase(int[] currencies)
        {
            // Calls the StoreManager trough the GameObserverManager for requesting a purchase to the player
            GameObserverManager.OnRequestBuy(myId, currencies);
        }

        #endregion

    
    }
}

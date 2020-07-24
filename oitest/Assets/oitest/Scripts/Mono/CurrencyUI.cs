using System;
using oitest.Scripts.Enums;
using oitest.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace oitest.Scripts.Mono
{
    /// <summary>
    /// Class for the Currency UI, which updates its values based on the player's wallet.
    /// </summary>
    public class CurrencyUI : MonoBehaviour
    {
        #region Serializable Private Fields

        // Currency type
        [SerializeField] CurrencyType myType;
        
        // Reference to the text field
        [SerializeField] private TMP_Text tmpText;
        
        #endregion

        #region Private Fields

        // current value of the currency
        private int _myValue;

        #endregion
        
        #region MonoBehaviour Callbacks

        /// <summary>
        /// Called when the object is Enabled, this handles the subscription to GameObserverManager events
        /// relating to the Currency UI
        /// </summary>
        private void OnEnable()
        {
            GameObserverManager.UpdateCurrencies += OnUpdateCurrencies;
        }
        
        /// <summary>
        /// Called when the object is Disabled, this handles the unsubscription to GameObserverManager events
        /// relating to the Currency UI
        /// </summary>
        private void OnDisable()
        {
            GameObserverManager.UpdateCurrencies -= OnUpdateCurrencies;
        }
        
        #endregion

        #region Delegate Methods

        /// <summary>
        /// Updates the value of the currency and the display text based on the received value and currency type.
        /// </summary>
        /// <param name="currency">The currency to update</param>
        /// <param name="value">The current currency value</param>
        private void OnUpdateCurrencies(CurrencyType currency, int value)
        {
            if (currency == myType)
            {
                _myValue = value;
            }

            tmpText.text = "x "+_myValue;
        }

        #endregion


        
    }
}

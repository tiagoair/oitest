using System.Threading;
using oitest.Scripts.Enums;
using oitest.Scripts.Managers;
using oitest.Scripts.ScriptableObjects;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace oitest.Scripts.Mono
{
    /// <summary>
    /// Class that handles the Store Item UI and displays an image of the current item, as well as its name, cost and
    /// currency that is required to buy it.
    /// </summary>
    public class StoreItemUI : MonoBehaviour
    {
        #region Public Fields

        // Accessor for my item
        public Item MyItem => _myItem;

        #endregion
        
        #region Serializable Private Fields

        // Reference to the Image for the item
        [SerializeField] private Image myIcon;

        // Reference to the text for the item name
        [SerializeField] private TMP_Text myName;

        // Reference to the text for the item cost
        [SerializeField] private TMP_Text myCost;

        // Reference to the Image for the currency
        [SerializeField] private Image myCurrency;
        
        #endregion

        #region Private Fields

        // Stores information about this item
        private Item _myItem;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the Store Item UI object by setting its properties to the ones contained in the Item
        /// scriptable object.
        /// </summary>
        /// <param name="item"></param>
        public void Initialize(Item item)
        {
            myIcon.sprite = item.icon;
            myName.text = item.name;
            myCost.text = item.cost + " x";
            myCurrency.sprite = CurrencySpawnerManager.Instance.GetCurrencySprite(item.currencyType);
            _myItem = item;
        }

        #endregion
    }
}

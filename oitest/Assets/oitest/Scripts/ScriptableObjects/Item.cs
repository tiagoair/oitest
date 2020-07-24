using oitest.Scripts.Enums;
using UnityEngine;

namespace oitest.Scripts.ScriptableObjects
{
    /// <summary>
    /// Scriptable Object that has the data for an item.
    /// </summary>
    [CreateAssetMenu(menuName = "oitest/Item")]
    public class Item : ScriptableObject
    {
        #region Public Fields

        // Icon of the item
        public Sprite icon;

        // Name of the item
        public new string name;

        // Currency required to buy the item
        public CurrencyType currencyType;

        // Cost of the item
        public int cost;

        #endregion
    }
}

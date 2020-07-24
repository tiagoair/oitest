using UnityEngine;
using UnityEngine.UI;

namespace oitest.Scripts.Mono
{
    /// <summary>
    /// Class that handles the Item UI display, which shows items based on the player's inventory.
    /// </summary>
    public class ItemUI : MonoBehaviour
    {
        #region Serializable Private Fields

        // Reference to the Image
        [SerializeField] private Image myImage;
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the Image sprite to the desired one.
        /// </summary>
        /// <param name="sprite">Desired sprite to be displayed</param>
        public void SetSprite(Sprite sprite)
        {
            myImage.sprite = sprite;
        }

        #endregion
    }
}

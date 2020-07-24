namespace oitest.Scripts.Interfaces
{
    /// <summary>
    /// Interface for NPCs that can call Store Managers
    /// </summary>
    public interface IStoreFront
    {
        /// <summary>
        /// Toggles the display of the Store UI
        /// </summary>
        /// <param name="status">Desired status of the Store UI</param>
        void ToggleStoreView(bool status);

        /// <summary>
        /// Request a purchase of the current selected item in the Store Manager.
        /// </summary>
        /// <param name="currencies">The player's wallet</param>
        void RequestPurchase(int[] currencies);
    }
}

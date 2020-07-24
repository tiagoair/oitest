using UnityEngine;

namespace oitest.Scripts.Interfaces
{
    /// <summary>
    /// Interface for talkable NPCs
    /// </summary>
    public interface ITalkable
    {
        /// <summary>
        /// Toggles the Talk Icon of the NPC
        /// </summary>
        /// <param name="status">Desired status of the Talk Icon</param>
        void ToggleTalkIcon(bool status);
        
        /// <summary>
        /// Displays the talking content of this NPC.
        /// </summary>
        void DoTalk();
    }
}

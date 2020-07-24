using UnityEngine;

namespace oitest.Scripts.Abstracts
{
    /// <summary>
    /// Abstract class that defines a generic NPC
    /// </summary>
    public abstract class NPC : MonoBehaviour
    {
        #region Serializable Private Fields

        // the Id of the NPC
        [SerializeField] protected int myId;
        
        #endregion

    }
}

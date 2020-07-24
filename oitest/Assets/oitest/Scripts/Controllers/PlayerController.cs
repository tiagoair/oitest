using System;
using System.Collections.Generic;
using oitest.Scripts.Abstracts;
using oitest.Scripts.Enums;
using oitest.Scripts.Interfaces;
using oitest.Scripts.Managers;
using oitest.Scripts.Mono;
using oitest.Scripts.ScriptableObjects;
using UnityEngine;

namespace oitest.Scripts.Controllers
{
    /// <summary>
    /// Class that handles the control of the Player Character.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Private Serializable Fields

        // Defines player's movement limits on the X axis
        [SerializeField] private float minXMovement;
        [SerializeField] private float maxXMovement;
        
        // Player's movement speed
        [SerializeField] private float speed;

        // The range for collecting currencies and interacting with NPCs
        [SerializeField] private float collectRange;
        
        #endregion

        #region Private Fields

        // List of all collectables that are in the player's range
        private List<GameObject> _collectablesInRange;
        
        // List of all NPCs that are able to Talk that are in the player's range
        private GameObject _talkableInRange;

        // The player's wallet
        private int[] currencies;
        
        // The player's inventory
        private List<Item> _myItems; 
        
        // Reference to the player's transform
        private Transform _myTransform;
        
        // Reference for the current state of the mirror state of the player
        private bool _isMirrored;
        
        // Reference for the currency layer mask
        private int _currencyLayerMask = 1 << 8;
        
        // Reference for the NPC layer mask
        private int _npcLayerMask = 1 << 9;

        // The current state of the Player's Interact state, which defines what actions it can perform
        private PlayerInteractState _currentInteractState;

        #endregion

        #region MonoBehaviour Callbacks

        /// <summary>
        /// Called when the object is Enabled, this handles the subscription to GameObserverManager events
        /// relating to the player
        /// </summary>
        private void OnEnable()
        {
            GameObserverManager.BrowsingToggle += OnBrowsingToggle;
            GameObserverManager.DeliverItem += OnDeliverItem;
        }

        /// <summary>
        /// Called when the object is Disabled, this handles the unsubscription to GameObserverManager
        /// events relating to the player
        /// </summary>
        private void OnDisable()
        {
            GameObserverManager.BrowsingToggle -= OnBrowsingToggle;
            GameObserverManager.DeliverItem -= OnDeliverItem;
        }
        
        /// <summary>
        /// Start is called before the first frame update, this handles the initialization of the player
        /// </summary>
        void Start()
        {
            _myTransform = this.transform;
            _collectablesInRange = new List<GameObject>();
            _myItems = new List<Item>();
            _currentInteractState = PlayerInteractState.Collecting;
            currencies = new int[5];
        }

        /// <summary>
        /// Update is called once per frame, this checks the current state of the Player Interact to handle
        /// movement and other interactions
        /// </summary>
        void Update()
        {
            // The Browsing state refers to when the player has the Store open
            if (_currentInteractState != PlayerInteractState.Browsing)
            {
                MovePlayer();
                CheckTalkableInRange();
             
                // The Collecting state refers to when the player isn't near a talking NPC
                if (_currentInteractState == PlayerInteractState.Collecting)
                {
                    CheckCollectableInRange();
                    CollectCurrenciesInRange();
                }

                // The Talking state refers to when the player is near a talking NPC
                if (_currentInteractState == PlayerInteractState.Talking)
                {
                    TalkWithNPC();
                }
            }
            else
            {
                StoreBrowsing();
            }
            
        }

        #endregion

        #region Delegate Methods
        
        /// <summary>
        /// Toggles between the Browsing and Talking states based on the obj variable, called by
        /// the StoreManager via GameObserverManager
        /// </summary>
        /// <param name="obj">Defines if the player is Browsing or Talking</param>
        private void OnBrowsingToggle(bool obj)
        {
            if (obj) _currentInteractState = PlayerInteractState.Browsing;
            else _currentInteractState = PlayerInteractState.Talking;
        }
        
        /// <summary>
        /// Delivers an Item from the StoreManager to the Player's inventory as well as the change of that
        /// transaction, this also updates the Currency and Item UIs.
        /// </summary>
        /// <param name="item">Item bought by the player</param>
        /// <param name="currency">The type of currency used in the transaction</param>
        /// <param name="change">The change from the transaction</param>
        private void OnDeliverItem(Item item, CurrencyType currency, int change)
        {
            _myItems.Add(item);
            currencies[(int) currency] = change;
            GameObserverManager.OnUpdateCurrencies(currency,change);
            GameObserverManager.OnAddItem(item);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The player movement is updated based on the Horizontal Axis input and the defined speed variable,
        /// for the movement to be allowed, the resulting translation must be inside the movement limits defined
        /// by the designer. A check for the current mirror state of the player is also done, which is based on
        /// the character's latest movement direction.
        /// </summary>
        private void MovePlayer()
        {
            Vector3 newTranslation = new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, 0);
            float checkLimits = transform.position.x + newTranslation.x;
            
            if(checkLimits > minXMovement && checkLimits < maxXMovement) _myTransform.Translate(newTranslation);
            

            bool previousIsMirrored = _isMirrored;

            if (newTranslation.x < 0)
            {
                _isMirrored = true;
            } else if (newTranslation.x > 0)
            {
                _isMirrored = false;
            }

            if (previousIsMirrored != _isMirrored)
            {
                if (_isMirrored)
                {
                    transform.localScale = new Vector3(-0.5f,0.5f,0.5f);
                }
                else
                {
                    transform.localScale = Vector3.one * 0.5f;
                }
            }
        }

        /// <summary>
        /// Checks for all collectables that are in range of the player, defined by the collectRange variable.
        /// This uses a CircleCast to get a reference for all colliders that are in range, then add them to the
        /// _collectablesInRange list, after that a distance check is done to see if any collectables have left the
        /// collect range, if they have they are removed from the list. All collectables in the list have their
        /// "Collect" icon enabled, which is disabled if they leave the list.
        /// </summary>
        private void CheckCollectableInRange()
        {
            // The CircleCast done to check colliders in the Currency Layer
            RaycastHit2D[] collectables = Physics2D.CircleCastAll((Vector2) transform.position, collectRange,
                Vector2.zero, 0, _currencyLayerMask);
            if (collectables.Length > 0)
            {
                foreach (RaycastHit2D collectable in collectables)
                {
                    GameObject collectableGO = collectable.transform.gameObject;
                    if (!_collectablesInRange.Contains(collectableGO)) _collectablesInRange.Add(collectableGO);
                }
            }

            // An array of the GameObjects currently in the collectables list is created in order to verify
            // if any of them have left the collect range distance
            GameObject[] collectablesInRange = _collectablesInRange.ToArray();
            foreach (GameObject collectable in collectablesInRange)
            {
                if (!collectable.activeSelf)
                {
                    _collectablesInRange.Remove(collectable);
                }
                else
                {
                    if (Vector3.Distance(transform.position, collectable.transform.position) > collectRange)
                    {
                        collectable.GetComponent<Currency>().ToggleCollectIcon(false);
                        _collectablesInRange.Remove(collectable);
                    }
                    else
                    {
                        collectable.GetComponent<Currency>().ToggleCollectIcon(true);
                    }
                }
            }
        }

        /// <summary>
        /// Toggles Collect Icons from all Collectables in the _collectablesInRange list based on the state variable.
        /// This is usually called when there's a talking NPC nearby, in order to guarantee only one viable action
        /// for the player.
        /// </summary>
        /// <param name="state">Defines the state of the Collect Icons</param>
        private void ToggleAllCollectIcons(bool state)
        {
            foreach (GameObject collectable in _collectablesInRange)
            {
                collectable.GetComponent<Currency>().ToggleCollectIcon(state);
            }
        }

        /// <summary>
        /// This collects all the currencies that are in the _collectablesInRange list, also calling the
        /// CurrencySpawnManager singleton to handle their deactivation in the pool, finally it uses the
        /// GameObserverManager to update the Currencies UI.
        /// </summary>
        private void CollectCurrenciesInRange()
        {
            if (Input.GetButtonDown("Jump"))
            {
                foreach (GameObject collectable in _collectablesInRange)
                {
                    Currency current = collectable.GetComponent<Currency>();
                    currencies[(int) current.MyCurrencyType]++;
                    CurrencySpawnerManager.Instance.DeactivateCurrency(collectable);
                }
                
                for (int i = 0; i < currencies.Length; i++)
                {
                    GameObserverManager.OnUpdateCurrencies((CurrencyType)i,currencies[i]);
                }
            }
        }

        /// <summary>
        /// Checks for the closest Talkable NPC in range using a CircleCast based on collectRange, if it finds one,
        /// then it sets it to the _talkableInRange variable, next a check is done to see if the NPC is still in range
        /// and if it is its Talk Icon is enabled and the Player Interact state is changed to Talking.
        /// </summary>
        private void CheckTalkableInRange()
        {
            // CircleCast to find the closest talkable NPC in range
            RaycastHit2D talkable = Physics2D.CircleCast((Vector2) transform.position, collectRange,
                Vector2.zero, 0, _npcLayerMask);
            if (talkable)
            {
                GameObject npcGO = talkable.transform.gameObject;
                if (ReferenceEquals(_talkableInRange, null) || _talkableInRange != npcGO) _talkableInRange = npcGO;
            }

            // if there's a talkable NPC in range, then check if its active and its distance to the player
            // if it fails, return to the Collecting state, else go to the Talking state. Also toggle the 
            // talk icon.
            if (!ReferenceEquals(_talkableInRange, null))
            {
                if (!_talkableInRange.activeSelf)
                {
                    _talkableInRange = null;
                    _currentInteractState = PlayerInteractState.Collecting;
                }
                else
                {
                    if (Vector3.Distance(transform.position, _talkableInRange.transform.position) > collectRange)
                    {
                        _talkableInRange.GetComponent<ITalkable>().ToggleTalkIcon(false);
                        _talkableInRange = null;
                        _currentInteractState = PlayerInteractState.Collecting;
                    }
                    else
                    {
                        _talkableInRange.GetComponent<ITalkable>().ToggleTalkIcon(true);
                        _currentInteractState = PlayerInteractState.Talking;
                        ToggleAllCollectIcons(false);
                    }
                }
            }
        }

        /// <summary>
        /// Executes the DoTalk function in the Talkable NPC in range when the player presses the interact button.
        /// </summary>
        private void TalkWithNPC()
        {
            if (Input.GetButtonDown("Jump"))
            {
                _talkableInRange.GetComponent<ITalkable>().DoTalk();
            }
        }

        /// <summary>
        /// During store browsing, the player may press Escape to quit the Store UI or press the interact button
        /// to request a product to the Store Front NPC.
        /// </summary>
        private void StoreBrowsing()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _talkableInRange.GetComponent<IStoreFront>().ToggleStoreView(false);
            }

            if (Input.GetButtonDown("Jump"))
            {
                _talkableInRange.GetComponent<IStoreFront>().RequestPurchase(currencies);
            }
        }

        #endregion
    }
}

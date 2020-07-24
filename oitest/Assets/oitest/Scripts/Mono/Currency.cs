using System.Collections;
using System.Collections.Generic;
using oitest.Scripts.Enums;
using oitest.Scripts.Managers;
using UnityEngine;

namespace oitest.Scripts.Mono
{
    /// <summary>
    /// Currency that is spawned in the game, disappears after 5 seconds and can be of 5 different types
    /// given by CurrencyType.
    /// </summary>
    public class Currency : MonoBehaviour
    {
        #region Public Fields

        // Accessor for the type of this currency
        public CurrencyType MyCurrencyType => myCurrencyType; 

        #endregion
        
        #region Serializable Private Fields

        // Type of this currency
        [SerializeField] private CurrencyType myCurrencyType;

        // The time to live of this currency, when this time ends the currency is despawned
        [SerializeField] private float timeToLive;
        
        // Icon for collecting currencies
        [SerializeField] private GameObject collectIcon;
        
        #endregion

        #region Private Fields

        // The current time of that this currency is active since it was spawned
        private float _currentTimeAlive;

        // Reference to the sprite renderer
        private SpriteRenderer _mySpriteRenderer;

        // State of the currency
        private bool _isActive;

        // Blink state of the currency, can be 0 (no blink), 1 (blinking), and 2 (fast blinking)
        private int _blinkState;
        
        // References for opaque and transparent colors
        private readonly Color _opaque = new Color(1,1,1,1);
        private readonly Color _transparent = new Color(0,0,0,0);

        #endregion

        #region MonoBehaviour Callbacks

        /// <summary>
        /// Called once per frame, calls the Time Countdown method.
        /// </summary>
        void Update()
        {
            TimeCountdown();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates a currency with the provided variables.
        /// </summary>
        /// <param name="currencyType">The type of the currency</param>
        /// <param name="aliveTime">How much time will the currency stay active</param>
        /// <param name="position">The position of the currency in the world</param>
        /// <param name="currencySprite">The sprite of the currency</param>
        public void Activate(CurrencyType currencyType, float aliveTime, Vector3 position, Sprite currencySprite)
        {
            myCurrencyType = currencyType;
            timeToLive = aliveTime;
            transform.position = position;
            _currentTimeAlive = 0;
            _mySpriteRenderer = GetComponent<SpriteRenderer>();
            _mySpriteRenderer.sprite = currencySprite;
            _mySpriteRenderer.color = _opaque;
            _blinkState = 0;
            _isActive = true;
        }

        /// <summary>
        /// Deactivates the currency by turning off the collect icon, setting isActive to false and changing the color
        /// to transparent.
        /// </summary>
        public void Deactivate()
        {
            ToggleCollectIcon(false);
            _isActive = false;
            _mySpriteRenderer.color = _transparent;
        }

        /// <summary>
        /// Toggles the Collect Icon display based on the state variable provided.
        /// </summary>
        /// <param name="state">Desired state of the Collect Icon</param>
        public void ToggleCollectIcon(bool state)
        {
            if(collectIcon.activeSelf != state)
                collectIcon.SetActive(state);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Counts down the time since the activation of the currency, from a certain point it starts to blink and
        /// if the time reaches the specified time to live, it calls for the deactivation of itself.
        /// </summary>
        private void TimeCountdown()
        {
            if (_isActive)
            {
                if (_currentTimeAlive <= timeToLive)
                {
                    _currentTimeAlive += Time.deltaTime;
                    
                    // with only 1.5 seconds left it starts to blink 
                    if (_currentTimeAlive>timeToLive-1.5f && _blinkState == 0)
                    {
                        _blinkState = 1;
                        StartCoroutine(Blink(0.2f, 1f));
                    }
                    
                    // with 0.5 seconds left it blinks faster
                    if (_currentTimeAlive>timeToLive-0.5f && _blinkState == 1)
                    {
                        _blinkState = 2;
                        StartCoroutine(Blink(0.1f, 0.5f));
                    }
                }
                else
                {
                    CurrencySpawnerManager.Instance.DeactivateCurrency(this.gameObject);
                }
            }
            
        }

        /// <summary>
        /// Makes the currency blink in the desired speed for the desired duration.
        /// </summary>
        /// <param name="speed">Desired speed of the blink</param>
        /// <param name="duration">Desired duration of the blink</param>
        /// <returns></returns>
        private IEnumerator Blink(float speed, float duration)
        {
            float currentTime = 0f;
            bool blinkOn = false;
            while (currentTime <= duration && _isActive)
            {
                if(blinkOn)
                    _mySpriteRenderer.color = _transparent;
                else
                    _mySpriteRenderer.color = _opaque;

                blinkOn = !blinkOn;
                currentTime += speed;
                yield return new WaitForSeconds(speed);
            }
        }

        #endregion
    }
}

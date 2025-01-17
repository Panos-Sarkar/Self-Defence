﻿using SelfDef.Systems.FireProjectile;
using SelfDef.Systems.UI;
using SelfDef.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

namespace SelfDef.PlayerScripts
{
    public class PlayerLogicScript : MonoBehaviour
    {
#pragma warning disable CS0649
        public static PlayerLogicScript Instance { get; private  set; }

        [Header("References")] 
        [SerializeField] private PlayerVariables playerVariable;
        [SerializeField] private PersistentVariables persistentVariable;
        [SerializeField] private PlayerInput playerInputVar;
        [SerializeField] private SpawnProjectiles projectileSystem;
        [SerializeField] private GameObject headInnerTransform;
        [SerializeField] private GameObject headGridTransform;

        private PlayerInputActions _inputActionsVar;
    
        [Header("Player Attributes")]
        [SerializeField] 
        private float headRotationSpeed;
        
        [SerializeField] 
        private float maxLife = 10;
        private float _life;
    
        [SerializeField] 
        private float maxStamina = 10;
        private float _stamina;

        [SerializeField] 
        private float staminaRegen;
        [SerializeField] 
        private float fireRate = 0.5f;
        
        private float _timeToFire;
    
        private Transform _myTransform;
        private UserInterfaceHandler _uiInstance;
        private Slider _healthRef;
        private Slider _staminaRef;
        private TextMeshProUGUI _moneyRef;
        private TextMeshProUGUI _titleRef;
        private const string Padding = "    ";
        
        //Events 
        public delegate void PlayerEventHandler();
        public event PlayerEventHandler PlayerFiredProjectile;
        
#pragma warning restore CS0649

        private void Awake()
        {
            _timeToFire = 0;
            if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
            _myTransform = transform;
            InitializeInputSystem();
        }
    
        private void Start()
        {
            GetUiRefs();

            InitializeValues();
        }
        
        private void Update()
        {
            RotatePlayerHead();
            IncreaseStamina();
            if (_life <=0)
            {
                KillPlayer();
            }
            else
            {
                UpdatePlayerStats();
            }

            _timeToFire -= Time.deltaTime;
        }
    
        private void RotatePlayerHead()
        {
            headGridTransform.transform.Rotate(Vector3.up * (Time.deltaTime * headRotationSpeed));
            headInnerTransform.transform.Rotate(Vector3.up * (Time.deltaTime * headRotationSpeed));
        }

        private void UpdatePlayerStats()
        {
            _healthRef.value = _life;
            _staminaRef.value = _stamina;
            _moneyRef.text = Padding + playerVariable.money;
#if UNITY_EDITOR
            _uiInstance.PrintToDebug(0,"Life: " + _life + " Stamina: " + _stamina);
#endif
        }

        private void KillPlayer()
        {
            _life = 0;
            _stamina = 0;
            playerVariable.money = 0;

            _healthRef.value = _life;
            _staminaRef.value = _stamina;
            _titleRef.text = Padding + "CPhage is DEAD :(";
            _moneyRef.text = Padding + playerVariable.money;
        }

        private void GetUiRefs()
        {
            _uiInstance = UserInterfaceHandler.Instance;
           
            _healthRef = _uiInstance.healthBar;
            _staminaRef = _uiInstance.staminaBar;

            _titleRef = _uiInstance.titleField;
            _moneyRef = _uiInstance.moneyField;

        }
        
        private void InitializeValues()
        {
            _life = maxLife;
            // Life Init -----------------------------------------------------

            _healthRef.maxValue = maxLife;
            _healthRef.value = _life;
        
            // Stamina Init -----------------------------------------------------
            _stamina = maxStamina / 2;
            _staminaRef.maxValue = maxStamina;
            _staminaRef.value = _stamina;
        
            // Title Init -----------------------------------------------------
            _titleRef.text = Padding + "Defend the CPhage!";

            // playerVariable.money Init -----------------------------------------------------
            _moneyRef.text = Padding + playerVariable.money;
        }

        private  void IncreaseStamina()
        {
            if(!persistentVariable.loading) _stamina += Time.deltaTime*staminaRegen;
        
            if (_stamina > maxStamina) _stamina = maxStamina; 
        }

        public void GiveMoney(int amount)
        {
            playerVariable.money += amount;
        }
    
        public void TakeMoney(int amount)
        {
            if(playerVariable.money<amount) return;
            playerVariable.money -= amount;
        }

        public void GiveLife(int amount)
        {
        
            _life += amount;
        }
    
        public void IncreaseLife(int amount)
        {
            maxLife += amount;
            _life += amount;
        }
    
        public void GiveStamina(int amount)
        {
            if (amount <= 0) return;
            _stamina += amount;
        }
    
        public void IncreaseStamina(int amount)
        {
            maxStamina += amount;
            _stamina += amount;
        }
        
        private void OnEnable()
        {
            _inputActionsVar.Enable();
            InputUser.onChange += OnInputDeviceChange;
        }

        private void OnDisable()
        {
            _inputActionsVar.Disable();
            InputUser.onChange -= OnInputDeviceChange;
        }
        
        private bool PlayerCanFire()
        {
            var cursorIsOverUi = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            
            if (!cursorIsOverUi && (_timeToFire < 0) && (_stamina > 0))
            {
                _timeToFire = fireRate;
                return true;
            }

            return false;
        }
        
        private void FireProjectile(InputAction.CallbackContext context)
        {
            PlayerFiredProjectile?.Invoke();
            
            if (PlayerCanFire())
            {
                projectileSystem.SpawnFireEffect();
                if(!persistentVariable.loading) _stamina -= 1;
            }
        }
        
        private void SpecialAction(InputAction.CallbackContext context)
        {
            if (PlayerCanFire() && playerVariable.playerAbilities[PlayerVariables.PlayerAbilities.StarUltimate])
            {
                
                projectileSystem.SpawnUltimateEffect(_myTransform);
                if(!persistentVariable.loading) _stamina -= 5;
            }
        }

        private void InitializeInputSystem()
        {
            _inputActionsVar = new PlayerInputActions();
        
            _inputActionsVar.PlayerControls.Fire.performed += FireProjectile;
            _inputActionsVar.PlayerControls.ContextMenu.performed += SpecialAction;
        }
    
        private void OnInputDeviceChange(InputUser user, InputUserChange change, InputDevice device) {
            if (change == InputUserChange.ControlSchemeChanged) {_uiInstance.ToggleInputIcon(playerInputVar.currentControlScheme);}
        }

        public void DamagePlayer(float amount)
        {
            _life -= amount;
            if (_life < 0) _life = 0;
        }
    }
}
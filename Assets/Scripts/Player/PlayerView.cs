using UnityEngine;

namespace Player
{
    /// <summary>
    /// Componente View del patrón MVC que maneja la presentación
    /// </summary>
    public class PlayerView : MonoBehaviour
    {
        [Header("Sub-Views")] 
        [SerializeField] private MeleeView meleeView;
        [SerializeField] private PlayerAudioView audioView;

        [SerializeField] public PlayerEffectsView effectsView;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerModel _playerModel;

        private void Awake()
        {
            // Obtener referencias a los otros componentes MVC
            if (_playerController == null) _playerController = GetComponent<PlayerController>();
            if (_playerModel == null) _playerModel = GetComponent<PlayerModel>();
        }

        private void Start()
        {
            InitializeSubViews();

            //Cursor Mouse
            Cursor.visible = false;
        }

        private void InitializeSubViews()
        {
            // Inicializar todas las sub-views
            meleeView?.Initialize();
            audioView?.Initialize();
            effectsView?.Initialize();
        }
        

        // Public methods llamados por el PlayerModel para efectos
        public void PlayDamageEffect()
        {
            effectsView?.PlayDamageFlash();
            audioView?.PlayDamageSound();
        }

        public void PlayHealthEffect()
        {
            audioView?.PlayHealthSound();
        }

        public void SetPlayerController(PlayerController controller)
        {
            _playerController = controller;
        }
    }
}
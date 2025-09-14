using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public static class PlayerHelper
    {
        private static GameObject _playerGO;

        public static void SetPlayer(GameObject player)
        {
            _playerGO = player;
        }

        public static GameObject GetPlayer()
        {
            return _playerGO;
        }

        public static void EnableInput()
        {
            if (_playerGO) _playerGO.GetComponent<PlayerInput>().enabled = true;
        }

        public static void DisableInput()
        {
            if (_playerGO) _playerGO.GetComponent<PlayerInput>().enabled = false;
        }


        public static void ClearPlayer()
        {
            _playerGO = null;
        }
    }
}
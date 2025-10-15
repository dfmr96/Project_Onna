using Player;

namespace Core
{
    public class PlayerInitializedSignal
    {
        public PlayerModel Model;
        public PlayerControllerEffect PlayerEffect;


        public PlayerInitializedSignal(PlayerModel model, PlayerControllerEffect playerEffect)
        {
            Model = model;
            PlayerEffect = playerEffect;
        }
    }
}
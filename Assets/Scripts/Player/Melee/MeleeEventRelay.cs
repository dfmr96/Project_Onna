using UnityEngine;

namespace Player.Melee
{
    public class MeleeEventRelay : MonoBehaviour
    {
        [SerializeField] private MeleeController meleeController; 
   
        //En uso en animation events
        public void ExecuteDamage()
        {
            if (meleeController != null)
                meleeController.ExecuteDamage();
        }
    }
}
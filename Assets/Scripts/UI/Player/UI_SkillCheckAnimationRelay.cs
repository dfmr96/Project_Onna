using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillCheckAnimationRelay : MonoBehaviour
{
    private Player.Weapon.UI_SkillCheck skillCheck;

        void Awake()
        {
            skillCheck = FindObjectOfType<Player.Weapon.UI_SkillCheck>();
        }

        // estos métodos los llaman los Animation Events
        public void OnOpenAnimationEnd()
        {
            skillCheck?.OnOpenAnimationEnd();
        }

        public void OnResultAnimationEnd()
        {
            skillCheck?.OnResultAnimationEnd();
        }
}

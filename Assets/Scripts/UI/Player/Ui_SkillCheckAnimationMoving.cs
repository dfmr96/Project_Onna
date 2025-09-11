using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ui_SkillCheckAnimationMoving : MonoBehaviour
{
    private Player.Weapon.UI_SkillCheck skillCheck;

    void Awake()
    {
        skillCheck = FindObjectOfType<Player.Weapon.UI_SkillCheck>();
    }
        
        
    public void OnResultAnimationEnd()
    {
        skillCheck?.HideMoveBar();
    }
}

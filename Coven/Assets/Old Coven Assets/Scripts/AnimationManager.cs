using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator animator;
    WeaponManager weaponManager;
    CharacterManager characterManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterManager = GetComponentInParent<CharacterManager>();
        weaponManager = transform.GetChild(0).GetComponentInChildren<WeaponManager>();
    }

    public void UpdateAnimator(float delta, float moveAmount, Vector2 moveDirection)
    {
        bool walking = (moveAmount > 0);
        // update animator values based on input
        if (walking != animator.GetBool("Walking")) { animator.SetBool("Walking", walking); }
        if(walking)
        {
            animator.SetFloat("X", moveDirection.x);
            animator.SetFloat("Y", moveDirection.y);
        } 
    }

    public void HandleAttackAnimations()
    {
        if(Mathf.Abs(animator.GetFloat("Y")) > Mathf.Abs(animator.GetFloat("X"))){ animator.SetFloat("X", 0); }
        else if (Mathf.Abs(animator.GetFloat("X")) > Mathf.Abs(animator.GetFloat("Y"))) { animator.SetFloat("Y", 0); }
        animator.SetTrigger("Attack");
        animator.SetBool("Combo", characterManager.comboFlag);
    }

    public void SetWeaponSprite(int index)
    {
        weaponManager.SetSprite(index);
    }

    public void SetIsInteracting(int value)
    {
        if(value == 1) { characterManager.isInteracting = true; }
        else { characterManager.isInteracting = false; }
    }

    public void SetDirection(int value)
    {
        characterManager.SetDirection(value);
    }

    public void ResetComboFlag()
    {
        characterManager.comboFlag = false;
    }

    public void SetCanDoCombo(int value)
    {
        if (value == 1) { characterManager.canDoCombo = true; }
        else { characterManager.canDoCombo = false; }
    }
}

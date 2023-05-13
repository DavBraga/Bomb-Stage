using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A health script that takes fixed damage
public class ShipHealth : Health
{
    Animator MyAnimator;
    [SerializeField]int  damageTaken = 1;
    [SerializeField] GameObject shipFire1, shipFire2, shipFire3;

    private void Start() {
        base.currentHealth = base.maxHealth;
        MyAnimator = GetComponent<Animator>();
    }
     public override bool OnTakeDamage(int damage)
    {
        if(base.damageParticles) Destroy(Instantiate(damageParticles, transform.position,  damageParticles.transform.rotation),FXLifetime);
        currentHealth -=damageTaken;
        MyAnimator.SetTrigger("TakeDamage");
        if(currentHealth<base.maxHealth) shipFire1.SetActive(true);
        if(currentHealth<base.maxHealth/2)
        {
            shipFire2.SetActive(true);
            shipFire3.SetActive(true);
        } 
        if(currentHealth>0) return false;

        MyAnimator.SetTrigger("Death");
        GameManager.Instance.EndLevel(true);
        //OnDeath();
        
        return true;

    }
}

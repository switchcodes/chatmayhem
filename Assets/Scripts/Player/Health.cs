using System;
using UnityEngine;

namespace Player
{
    public class Health : MonoBehaviour
    {
        public int currentHealth = 4;
        public HealthBar bar;
        public PauseMenu pauseMenu;
        public float maxInvincibleTime = 1f;
        private float currentInvincibleTIme;
        
        public void GetDamaged()
        {
            if (currentInvincibleTIme > 0)
            {
                return;
            }
            currentHealth--;
            currentInvincibleTIme = maxInvincibleTime;
            bar.SetHealthBar(currentHealth);
            if (currentHealth <= 0)
            {
                //game over
                pauseMenu.GameOver();
            }
        }

        public void Heal()
        {
            currentHealth++;
            if (currentHealth >= 4)
            {
                currentHealth = 4;
            }
            bar.SetHealthBar(currentHealth);
        }

        private void Update()
        {
            if (currentInvincibleTIme > 0)
            {
                currentInvincibleTIme -= Time.deltaTime;  
            }
            
        }

        
    }
}
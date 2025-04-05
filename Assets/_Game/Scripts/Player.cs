using CamLib;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Singleton<Player>
{
    public int HP;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void TakeDamage()
    {
        HP--;
        if (HP <= 0)
        {
            // Handle player death
            Debug.Log("Player is dead");
            
        }
        else
        {
            // Handle player taking damage
            Debug.Log("Player took damage, remaining HP: " + HP);
        }
    }

    public void Heal()
    {
        HP = 3;
    }
    
    
}

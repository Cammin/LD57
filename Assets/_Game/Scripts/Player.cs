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
        //translate player
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(horiz, vert, 0).normalized;
        
        if (move.magnitude > 0)
        {
            transform.position += move * Time.deltaTime;
        }
    }

    public void TakeDamage()
    {
        HP--;
        if (HP <= 0)
        {
            Debug.Log("Player is dead");
            SceneManager.LoadScene("Gameplay");
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

using CamLib;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : Singleton<Player>
{
    public int HP;

    public float moveSpeed;
    public Rigidbody2D _rb;
    public Light2D flashlight;

    private Vector2 MoveInput;
    
    private void Start()
    {
        
    }

    private void Update()
    {
        DoMoveInput();
        TryCheats();
        UpdateFlashlight();
    }

    private void DoMoveInput()
    {
        //translate player
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(horiz, vert, 0).normalized;
        MoveInput = move;
        _rb.linearVelocity = MoveInput * (moveSpeed);
    }

    private void TryCheats()
    {
        if (Debug.isDebugBuild && Input.GetMouseButtonDown(2))
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void UpdateFlashlight()
    {
        //direction to mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 flashlightPos = Camera.main.transform.position;
        Vector2 dir =  mousePos - flashlightPos;
        
        flashlight.transform.up = dir;
    }

    private void FixedUpdate()
    {
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

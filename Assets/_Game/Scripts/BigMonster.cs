using System.Collections;
using System.Collections.Generic;
using CamLib;
using UnityEngine;

public class BigMonster : Singleton<BigMonster>
{
    public Transform LurkingPosition;

    public GameObject Model;
    public Rigidbody2D Rb;
    public float MoveSpeed;
    
    public Transform Target => Player.Instance.IsEmpty ? Player.Instance.transform : LurkingPosition;

    private IEnumerator Start()
    {
        for (int i = 0; i < 10; i++)
        {
            transform.position = (Vector2)LurkingPosition.position;
            yield return new WaitForSeconds(0.1f);
        }
    }


    private void FixedUpdate()
    {
        Vector2 dirToTarget = (Target.position - transform.position);
        

        if (Mathf.Abs(dirToTarget.x) > 0.5f)
        {
            Rb.linearVelocity = dirToTarget.normalized * MoveSpeed;
            Vector3 scale = Model.transform.localScale;
            float flipValue = dirToTarget.x < 0 ? 1 : -1;
            scale.x = Mathf.Abs(scale.x) * flipValue;
            Model.transform.localScale = scale;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.TryTakeDamage();
        }
    }
}
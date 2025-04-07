using UnityEngine;

public class RandomFlip : MonoBehaviour
{
    public SpriteRenderer Render;

    private void Start()
    {
        Render = GetComponent<SpriteRenderer>();
        Render.flipX = Random.value > 0.5f;
    }
}
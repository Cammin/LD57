using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PulseText : MonoBehaviour
{
    public TextMeshPro Text;
    public float VerticalDistance = 1;
    public float Duration = 1;
    public Vector2 DestinationRandomOffset = new Vector2(0.1f, 0.1f);

    private Sequence Sequence;

    public void ShowText(string text, Color color)
    {
        Text.text = text;
        Text.color = color;
        transform.position += new Vector3(0, 0, -0.01f);

        var destination = transform.position;
        destination.x += Random.Range(-DestinationRandomOffset.x, DestinationRandomOffset.x);
        destination.y += Random.Range(-DestinationRandomOffset.y, DestinationRandomOffset.y);
        destination.y += VerticalDistance;
        destination.z += -0.01f;

        Sequence = DOTween.Sequence(gameObject).Append(transform.DOMove(destination, Duration));

        Destroy(gameObject, Duration + 0.05f);
    }

    private void OnDestroy()
    {
        Sequence.Kill();
    }
}
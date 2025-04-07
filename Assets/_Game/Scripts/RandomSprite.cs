using UnityEngine;
using UnityEngine.U2D;

public class RandomSprite : MonoBehaviour
{
    public GameObject[] Randoms;

    private void Awake()
    {
        int randomChoice = Random.Range(0, Randoms.Length);
        for (int i = 0; i < Randoms.Length; i++)
        {
            Randoms[i].SetActive(i == randomChoice);
        }
    }
}
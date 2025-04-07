using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public GameObject[] Randoms;

    private void Start()
    {
        int randomChoice = Random.Range(0, Randoms.Length);
        for (int i = 0; i < Randoms.Length; i++)
        {
            Randoms[i].SetActive(i == randomChoice);
        }
    }
}
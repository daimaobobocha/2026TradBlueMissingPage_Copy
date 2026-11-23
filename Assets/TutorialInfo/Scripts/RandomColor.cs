using UnityEngine;

public class RandomCubeColor : MonoBehaviour
{
    void Start()
    {
        GetComponent<Renderer>().material.color =
            Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.7f, 1f);

        float size = Random.Range(0.8f, 1f);
        transform.localScale *= size;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBob : MonoBehaviour
{
    [SerializeField] public float amp = 0.25f;
    [SerializeField] public float freq = 1f;

    private Vector3 startPos;

    void Start()
    {
        startPos= transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Mathf.Sin(Time.time * freq) * amp;
        transform.position = startPos + new Vector3(0, offset, 0);

    }
}

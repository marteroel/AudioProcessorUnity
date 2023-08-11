using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreqBandVisualizer : MonoBehaviour
{
    public int _band;
    public float _startScale, _scaleMultiplier;
    public AudioProcessor _audioProcessor;

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x,
            (_audioProcessor._freqBand[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
    }
}

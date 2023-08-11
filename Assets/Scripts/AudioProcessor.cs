using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent (typeof (AudioSource))]

public class AudioProcessor : MonoBehaviour
{
    public AudioSource _audioSource;
    public bool useMicrophone;
    //private AudioClip _audioClip;
    private float[] _samples = new float[256];//fourier in 64 pieces
    private int[] _bandCutoff = new int[] {2, 8, 24, 256};
    public float[] _freqBand = new float[4];
    

    // Start is called before the first frame update
    void Start()
    {
        if(useMicrophone)
            if (Microphone.devices.Length > 0) {

                Debug.Log(Microphone.devices[0].ToString());

                _audioSource.clip = Microphone.Start(Microphone.devices[0].ToString(), true, 10, AudioSettings.outputSampleRate);
                _audioSource.loop = true;

                while(!(Microphone.GetPosition(null) > 0))
                   _audioSource.Play();
            }
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        FrequencyBands();
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void FrequencyBands()//promedio de varias muestras de fourier para componer cuatro bandas
    {
        int indexStartAverage = 0;//array place to start the sum for the average of each band
        int numberOfBands = _freqBand.Length;

        /*
        //equidistant bands, not ideal as there's too much space for high frequency content
        for (int i = 0; i < numberOfBands; i++)//four bands
        {

            Debug.Log((_samples.Length / numberOfBands)/2);
            float average = 0f;
            int sampleCount = ((int)Mathf.Pow(2, i) * ((_samples.Length/numberOfBands)/2)); 

            for (int j = indexStartAverage; j < sampleCount; j++)      
                average += _samples[j];

            //each band is the average from start to end samples arbitrarily multiplied by 10 for a more useful range
            average /= (sampleCount - indexStartAverage) *10;

            indexStartAverage = sampleCount;//to start now from the current sample count

            _freqBand[i] = average;
        }*/
        
        //predefined bands
        for (int i = 0; i < 4; i++)//four bands
        {
            //float average = 0f;
            float rms = 0f;
            int sampleCount = _bandCutoff[i];     

            for (int j = indexStartAverage; j < sampleCount; j++) { //sum
                //average += _samples[j];
                rms += Mathf.Pow(_samples[j], 2);
            }

            // arbitrarily multiplied by 10 for a more useful range
            //average /= (sampleCount - indexStartAverage) * 10;
            rms = Mathf.Sqrt(rms/(sampleCount - indexStartAverage)) * 10;

            indexStartAverage = sampleCount;//to start now from the current sample count

            _freqBand[i] = rms;//much better results with rms than average
        }
    }
}

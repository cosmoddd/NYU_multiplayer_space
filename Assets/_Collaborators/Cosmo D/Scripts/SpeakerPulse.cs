using UnityEngine;
using System.Collections;

public class SpeakerPulse : MonoBehaviour {

    public AudioSource sourceOfAudio;

    public int qSamples = 512;        // array size
    public float refValue = 0.1f;    // RMS value for 0 dB
    public float rmsValue;            // sound level - RMS
    public float dbValue;            // sound level - dB
    public float soundLevelCumulative;
    public float volume = 2;        // set how much the scale will vary

    private float[] samples;        // audio samples

    public SkinnedMeshRenderer[] skinnedMeshRenderers;


    void Start()
    {
        if (sourceOfAudio == null) sourceOfAudio = GetComponent<AudioSource>();
        samples = new float[qSamples];
    }

    // Update is called once per frame
    void Update()
    {
        GetVolume();
        soundLevelCumulative = volume *rmsValue;
        SetBlendShapesElsewhere(Convert(soundLevelCumulative));

    }

    void SetBlendShapesElsewhere(float _soundLevelCumulative)
    {
        foreach (SkinnedMeshRenderer r in skinnedMeshRenderers)
        {
            r.SetBlendShapeWeight(0, Convert(_soundLevelCumulative));
        }
    }

    public float output;

    public float rangeXlow;
    public float rangeXhigh;

    public float rangeYlow = 0;
    public float rangeYhigh = 100;

    public float Convert(float input)
    {
        output = ((input - rangeXlow) * ((rangeYhigh - rangeYlow) / (rangeXhigh - rangeXlow))) + rangeYlow;
        return output;
    }

    void GetVolume()
    {
        if (sourceOfAudio == null) return;
        
        sourceOfAudio.GetOutputData(samples, 0);

        float sum = 0;
        for (int i = 0; i < qSamples; i++)
        {
            sum += samples[i] * samples[i];    // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / qSamples);    // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / refValue);    // calculate dB
        if (dbValue < -160)
        {
            dbValue = -160;        // clamp it to -160 dB min
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviormentState : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private GameObject backgroundSound;
    private AudioSource backgroundSoundAudio;

    private void Start()
    {
        StartCoroutine(Fog());
        backgroundSoundAudio = backgroundSound.GetComponent<AudioSource>();
    }

    private bool fogState = true;
    private IEnumerator Fog()
    {
        yield return new WaitForSeconds(30f);
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (RenderSettings.fogDensity > 0.25f)
            {
                fogState = false;
                yield return new WaitForSeconds(10f);
            }
            else if (RenderSettings.fogDensity < 0.05f)
            {
                yield return new WaitForSeconds(120f);
                fogState = true;
            }

            if (fogState)
            {
                
                _light.intensity -= 0.002f;
                RenderSettings.fogDensity += 0.001f;
            }
            else
            {
                _light.intensity += 0.002f;
                RenderSettings.fogDensity -= 0.001f;
            }
        }
    }

}

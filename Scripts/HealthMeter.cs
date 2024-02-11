using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthMeter : MonoBehaviour
{
    private Slider slider;

    private void Awake() 
    {
        slider = transform.GetComponent<Slider>();

        Stats.HealthPercent = 0.5f; 
        StartCoroutine(HandleHealth());
    }

    private IEnumerator HandleHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (Stats.HungerPercent > 0.9f)
            {
                Stats.HealthPercent += 0.0025f;
                slider.value = Stats.HealthPercent;
            }
            else if (Stats.HungerPercent == 0)
            {
                Stats.HealthPercent -= 0.01f;
                slider.value = Stats.HealthPercent;
            }

            if (Stats.HealthPercent <= 0.01f)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Dead");
                SceneManager.LoadScene("Main Menu");
            }
        }
    }
}

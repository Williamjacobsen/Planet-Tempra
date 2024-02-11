using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class HungerMeter : MonoBehaviour
{
    private Slider slider;

    private void Awake() 
    {
        slider = transform.GetComponent<Slider>();

        Stats.HungerPercent = 1f; 
        StartCoroutine(HandleHunger());
    }

    private IEnumerator HandleHunger()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (Stats.HungerPercent <= 0.002f)
            {
                Stats.HungerPercent = 0;
            }
            else
            {
                Stats.HungerPercent -= 0.002f;
            }
            slider.value = Stats.HungerPercent;
        }
    }
}

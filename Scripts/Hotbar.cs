using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        for (int i = 0; i < 8; i++) // makes every non-active hotbar slot only slightly visible 
        {
            transform.GetChild(i).GetComponent<Image>().color = new Color(255, 255, 255, 0.33f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(ActivateHotbarSlot(slotIndex: 0));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(ActivateHotbarSlot(slotIndex: 1));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(ActivateHotbarSlot(slotIndex: 2));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(ActivateHotbarSlot(slotIndex: 3));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(ActivateHotbarSlot(slotIndex: 4));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartCoroutine(ActivateHotbarSlot(slotIndex: 5));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            StartCoroutine(ActivateHotbarSlot(slotIndex: 6));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartCoroutine(ActivateHotbarSlot(slotIndex: 7));
        }
    }

    private IEnumerator ActivateHotbarSlot(int slotIndex)
    {
        transform.GetChild(slotIndex).GetComponent<Image>().color = new Color(255, 255, 255, 0.66f);

        if (IsFood(slotIndex))
        {
            Eat(slotIndex);
        }
        else if (IsSeed(slotIndex))
        {
            PlantSeed(slotIndex);
        }

        yield return new WaitForSeconds(0.2f);

        transform.GetChild(slotIndex).GetComponent<Image>().color = new Color(255, 255, 255, 0.33f);
    }

    private void Eat(int slotIndex)
    {
        var counter = transform.GetChild(slotIndex).transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>();
        counter.text = (int.Parse(counter.text) - 1).ToString();
        if (int.Parse(counter.text) == 0)
        {
            Destroy(transform.GetChild(slotIndex).transform.GetChild(0).gameObject);
            Destroy(transform.GetChild(slotIndex).transform.GetChild(1).gameObject);
            Inventory.items[slotIndex] = null;
        }
        else
        {
            Inventory.items[slotIndex] = Tuple.Create(Inventory.items[slotIndex].Item1, Inventory.items[slotIndex].Item2 - 1);
        }

        Stats.HungerPercent += 0.2f;
        if (Stats.HungerPercent > 1)
        {
            Stats.HungerPercent = 1;
        }
    }
    
    private void PlantSeed(int slotIndex)
    {
    }

    private bool IsFood(int slotIndex)
    {
        try 
        {
            if (transform.GetChild(slotIndex).transform.GetChild(0)?.transform.name != null && transform.GetChild(slotIndex).transform.GetChild(0).transform.name.Contains("Berry"))
            {
                return true;
            }
        } catch {}

        return false;
    }

    private bool IsSeed(int slotIndex)
    {
        try 
        {
            if (transform.GetChild(slotIndex).transform.GetChild(0)?.transform.name != null && transform.GetChild(slotIndex).transform.GetChild(0).transform.name.Contains("Seed"))
            {
                return true;
            }
        } catch {}

        return false;
    }
}




using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 3f;
    private Vector3 direction;
    private float horizontalInput, verticalInput;
    [SerializeField] private CharacterController characterController;

    [SerializeField] private float groundYOffset;
    [SerializeField] private LayerMask groundMask;
    private Vector3 spherePosition;

    [SerializeField] private float gravity = -10f; 
    private Vector3 velocity;

    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject[] BasinBush;
    [SerializeField] private GameObject[] PadderPod;

    [SerializeField] private GameObject Hotbar;
    [SerializeField] private GameObject BerryAndSeedCounter;
    
    private Dictionary<string, GameObject> BerrysAndSeeds;

    private void Start() 
    {
        BerrysAndSeeds = new Dictionary<string, GameObject>(){
            {"Basin Bush Berry", BasinBush[0]},
            {"Basin Bush Seed", BasinBush[1]},
            {"Padder Pod Berry", PadderPod[0]},
            {"Padder Pod Seed", PadderPod[1]}
        };  

        StartCoroutine(Bored());        
    }

    private void Update()
    {
        _animator.SetBool("isGrounded", IsGrounded());

        Gather();

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("gather"))
        {
            return;
        }

        Run();
        Jump();
        GetDirectionAndMove();
        Gravity();
    }

    private IEnumerator Bored()
    {
        float timeWaited = 0f;
        while (true)
        {
            if (horizontalInput == 0f && verticalInput == 0f)
            {
                timeWaited += 0.1f;
            }
            else 
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("bored"))
                {
                    _animator.SetBool("bored", false);
                }
                timeWaited = 0f;
            }
            
            if (timeWaited > 10f)
            {
                _animator.SetBool("bored", true);
                timeWaited = 0f;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Gather()
    {
        if (!IsGrounded() || !Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        if (
            _animator.GetCurrentAnimatorStateInfo(0).IsName("idle") || 
            _animator.GetCurrentAnimatorStateInfo(0).IsName("bored")
        )
        {
            if (Inventory.PickUpCollider != null)
            {
                _animator.SetTrigger("gather");

                StartCoroutine(PickUpCollider.DestoryPlant());

                UpdateInventoryAndHotbar();
            }
        }
    }

    private void UpdateInventoryAndHotbar()
    {
        bool doesBerryExist = false;
        bool doesSeedExist = false;

        // update inventory berry/seed amount
        for (int i = 0; i < Inventory.items.Length; i++)
        {
            if (Inventory.items[i]?.Item1 == null)
            {
                continue;
            }

            if (Inventory.items[i].Item1 == Inventory.PickUpCollider.ToString() + " Berry")
            {
                Inventory.items[i] = Tuple.Create(Inventory.items[i].Item1, Inventory.items[i].Item2 + new System.Random().Next(1, 3));
                UpdateHotbarCounter(i);
                doesBerryExist = true;
            }
            else if (Inventory.items[i].Item1 == Inventory.PickUpCollider.ToString() + " Seed")
            {
                Inventory.items[i] = Tuple.Create(Inventory.items[i].Item1, Inventory.items[i].Item2 + new System.Random().Next(1, 4));
                UpdateHotbarCounter(i);
                doesSeedExist = true;
            }
        }

        // if it doesn't already exist, add berry/seed to inventory
        for (int i = 0; i < Inventory.items.Length; i++)
        {
            if (Inventory.items[i]?.Item1 == null)
            {
                if (!doesBerryExist)
                {
                    Inventory.items[i] = Tuple.Create(Inventory.PickUpCollider.ToString() + " Berry", new System.Random().Next(1, 3));
                    AddHotbarElement(i);
                    doesBerryExist = true;
                }
                else if (!doesSeedExist)
                {
                    Inventory.items[i] = Tuple.Create(Inventory.PickUpCollider.ToString() + " Seed", new System.Random().Next(1, 4));
                    AddHotbarElement(i);
                    doesSeedExist = true;
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void UpdateHotbarCounter(int i)
    {
        // does counter exist?
        GameObject Counter = Hotbar.transform.GetChild(i).transform.GetChild(1).gameObject;
        if (Counter == null || Counter.name != "Count(Clone)")
        {
            return;
        }

        // counter value
        int currentCounter = int.Parse(Counter.GetComponent<TextMeshProUGUI>().text);

        // is plant 'i' the same as in inventory 'i'
        string plantName = Hotbar.transform.GetChild(i).transform.GetChild(0).gameObject.name.Replace("(Clone)", "");
        if (plantName == Inventory.items[i].Item1)
        {
            // if there is a change if amount of berrys/seeds
            if (currentCounter != Inventory.items[i].Item2)
            {
                Counter.GetComponent<TextMeshProUGUI>().text = Inventory.items[i].Item2.ToString();
            }
        } 
    }

    private void AddHotbarElement(int i)
    {
        // create hotbar image/icon
        GameObject item = Instantiate(BerrysAndSeeds[Inventory.items[i].Item1]);
        item.transform.SetParent(Hotbar.transform.GetChild(i));
        item.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 2.5f, 0);
        item.transform.localScale = Vector3.one;

        // create counter/amount of berrys in slot counter
        item = Instantiate(BerryAndSeedCounter);
        item.transform.SetParent(Hotbar.transform.GetChild(i));
        item.transform.GetComponent<RectTransform>().offsetMax = new Vector2(-5, -32.5f);
        item.transform.localScale = Vector3.one;  

        UpdateHotbarCounter(i);
    }
    
    private void Run()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            _animator.SetBool("running", false);
            movementSpeed = 3f;
        }
        else if (horizontalInput == 0f && verticalInput == 0f && _animator.GetCurrentAnimatorStateInfo(0).IsName("running"))
        {
            _animator.SetBool("running", false);
            movementSpeed = 3f;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && _animator.GetCurrentAnimatorStateInfo(0).IsName("walking"))
        {
            movementSpeed = 8f;
            //movementSpeed = 20f;
            _animator.SetBool("running", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            movementSpeed = 3f;
            _animator.SetBool("running", false);
        }
    }

    private void Jump()
    {
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            if (velocity.y < 1.5f)
            {
                _animator.SetTrigger("jump");
                velocity.y += 6f;
            }
        }
    }

    private float actualMovementSpeed = 3f;
    private void GetDirectionAndMove()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        _animator.SetFloat("horizontalInput", horizontalInput);
        _animator.SetFloat("verticalInput", verticalInput);

        if (IsGrounded())
        {
            direction = transform.forward * verticalInput + transform.right * horizontalInput;
            actualMovementSpeed = movementSpeed;
        }

        characterController.Move(direction * actualMovementSpeed * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        spherePosition = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePosition, characterController.radius - 0.05f, groundMask)) 
        {
            return true;
        }
        return false;
    }

    private void Gravity()
    {
        if (!IsGrounded())
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(spherePosition, characterController.radius - 0.05f);
    //}
}

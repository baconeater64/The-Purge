using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;

    private bool isGrounded;
    
    public float mouseSensitivity = 100f;
    public Transform player;
    public Camera camera;

    private float _xRotation = 0f;
    private Vector3 _velocity;

    public GameObject bullet;
    public float bulletSpeed = 2500f;
    
    public GameObject gun;
    private Transform bulletSpawn;

    private Animator _gunAnimator;

    private AudioSource _gunShootingSound;
    
    
    private void Start()
    {
        Physics.IgnoreLayerCollision(8,10);
        Physics.IgnoreLayerCollision(10,10);
        Cursor.lockState = CursorLockMode.Locked;

        Transform gunModel = gun.transform.GetChild(0).transform;
        
        _gunAnimator = gunModel.gameObject.GetComponent<Animator>();
        bulletSpawn = gunModel.GetChild(gunModel.childCount-1);
        _gunShootingSound = gunModel.GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateLook();
        UpdateMove();
        ApplyForce();

        TestForMouseButtonPress();
    }

    private void TestForMouseButtonPress()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject go = Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            StartCoroutine(RunBullet(go));
            _gunAnimator.Play("Shoot");
            _gunShootingSound.Play();
        }
    }

    private IEnumerator RunBullet(GameObject bullet)
    {
        yield return true;
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletSpeed);
        bullet.transform.parent = null;
        Destroy(bullet, 5);
        yield return true;
    }

    private void ApplyForce()
    {
        GroundCheck();

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        _velocity.y += gravity * Time.deltaTime;
        controller.Move(_velocity * Time.deltaTime);
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && _velocity.y < 0) _velocity.y = -2f;
    }

    private void UpdateMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    private void UpdateLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90, 90f);
        
        camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }
}

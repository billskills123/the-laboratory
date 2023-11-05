using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour {
    [Header("Objects")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameManager gameManager;

    [Header("Player Settings")]
    [SerializeField] private Vector2 playerMoveValue;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float maxRotateSpeed;
    [SerializeField] private float smoothTime = 0.3f;
    private float currentVelocityFloat;
    private float angle;

    //Camera Settings
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private Vector3 velocity;


    private void Update() {
        if (gameManager.inGame == true) {
            transform.Translate(playerSpeed * Time.deltaTime * new Vector3(playerMoveValue.x, playerMoveValue.y, 0));

            Vector3 targetPosition = player.transform.position + offset;
            mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPosition, ref velocity, smoothTime);

            PlayerRotation();
        }
    }

    private void OnMove(InputValue value) {
        playerMoveValue = value.Get<Vector2>();
    }

    private void PlayerRotation() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 direction = mousePosition - player.transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.right, direction);
        angle = Mathf.SmoothDampAngle(angle, targetAngle, ref currentVelocityFloat, smoothTime, maxRotateSpeed);
        player.transform.eulerAngles = new Vector3(0, 0, angle);
    }
}
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour{
    [SerializeField]
    private GameObject ballPrefab;

    [SerializeField]
    private Rigidbody2D pivot;

    [SerializeField]
    private float respawnDelay = 1f;

    [SerializeField]
    private float detachDelay = 0.15f;

    private Rigidbody2D currentBallRigidbody2D;
    private SpringJoint2D currentSpringJoint2D;

    private Camera mainCamera;
    private bool isDragging;

    private void Start(){
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    private void OnEnable(){
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable(){
        EnhancedTouchSupport.Disable();
    }

    private void Update(){
        if (currentBallRigidbody2D == null) return;
        if (Touch.activeTouches.Count == 0){
            if (!isDragging) return;
            LaunchBall();
            isDragging = false;
        }
        else{
            isDragging = true;
            currentBallRigidbody2D.isKinematic = true;

            var touchPosition = new Vector2();
            touchPosition =
                Touch.activeTouches.Aggregate(touchPosition, (current, touch) => current + touch.screenPosition);
            touchPosition /= Touch.activeTouches.Count;

            var worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
            currentBallRigidbody2D.position = worldPosition;
        }
    }

    private void SpawnNewBall(){
        var ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        currentBallRigidbody2D = ballInstance.GetComponent<Rigidbody2D>();
        currentSpringJoint2D = ballInstance.GetComponent<SpringJoint2D>();
        currentSpringJoint2D.connectedBody = pivot;
    }

    private void LaunchBall(){
        currentBallRigidbody2D.isKinematic = false;
        currentBallRigidbody2D = null;
        Invoke(nameof(DetachBall), detachDelay);
    }

    private void DetachBall(){
        currentSpringJoint2D.enabled = false;
        currentSpringJoint2D = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
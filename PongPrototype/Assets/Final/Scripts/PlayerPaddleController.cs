using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPaddleController : MonoBehaviour {
    [Tooltip("Min distance to make paddle automatically track the ball.")]
    public float m_MinTrackingDistance = 4f;
    [Range(0.0f, 1.0f)]
    [Tooltip("How close the paddle is to the ball when using auto tracking.")]
    public float m_TrackingAccuracy = 0.3f;
    [Tooltip("Min distance to hit the ball.")]
    public float m_MinHittingDistance = 1f;
    [Tooltip("For smooth moving of the paddle.")]
    public float m_PaddleSmoothTime = 0.2f;
    [Tooltip("Cooldown time for hitting.")]
    public float m_HittingCooldownTime = 0.5f;
    [Tooltip("Base force when hitting the ball.")]
    public float m_BaseForceMultiplier = 7f;

    private bool hitting = false;
    private Transform ball;
    private Transform player;
    private Transform centerEyeAnchor;
    private Transform gearVRController;
    private Vector3 m_CurrentVelocity = Vector3.zero;

    #region Setters & Getters
    public Transform Ball
    {
        get
        {
            return ball;
        }

        set
        {
            ball = value;
        }
    }
    #endregion

    void Start() {
        GameplayController.Singleton.PlayerPaddle = this.transform;

        ball = GameObject.FindWithTag("Ball") ? GameObject.FindWithTag("Ball").transform : null;
        player = GameObject.FindWithTag("Player") ? GameObject.FindWithTag("Player").transform : null;
        centerEyeAnchor = player ? player.Find("TrackingSpace/CenterEyeAnchor") : null;
        gearVRController = player ? player.Find("TrackingSpace/RightHandAnchor/GearVrController") : null;
    }
    
    void Update() {
        //step 0: freeze paddle if hitting (cooldown)
        if (hitting == true) { 
            return;
        }   

        //step 1: calculate base position according to player's head direction
        float x = centerEyeAnchor.forward.x * 2f;
        float y = centerEyeAnchor.forward.y + 2.5f;
        float z = 1f;

        //step 2: add automatic ball tracking to help the player
        if (ball != null
            && ball.GetComponent<BallScript>().State != BallScript.BallState.Initialized
            && Vector3.Distance(player.position, ball.position) < m_MinTrackingDistance) {
            x *= 1f - m_TrackingAccuracy;
            y *= 1f - m_TrackingAccuracy;
            z *= 1f - m_TrackingAccuracy;

            x += ball.position.x * m_TrackingAccuracy;
            y += ball.position.y * m_TrackingAccuracy;
            z += ball.position.z * m_TrackingAccuracy - 0.5f; //offset to keep paddle behind the ball
        }

        //step 3: set paddle position
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(x, y, z), ref m_CurrentVelocity, m_PaddleSmoothTime);

        //step 4: rotate paddle according to GearVRController with helper (basic idea is pointing at paddle means aiming forward)
        if (gearVRController != null) {
            Vector3 controllerToPaddle = (transform.position - gearVRController.transform.position).normalized;
            Vector3 target = gearVRController.forward * 2f;

            target.x -= controllerToPaddle.x;
            target.y -= controllerToPaddle.y;

            transform.forward = target;
        }

        //step 5: hit the ball if the touchpad is pressed and the ball is in range
        if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad) 
            && ball != null 
            && Vector3.Distance(transform.position, ball.position) < m_MinHittingDistance)
        {
            transform.position = ball.position;

            float hitForce = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;
            hitForce = hitForce * 0.2f + 1; //map [-1, 1] to [0.8, 1.2]

            hit(ball.GetComponent<Rigidbody>(), hitForce); 
        }
    }

    void OnTriggerEnter(Collider col) {
        //may be make paddle opaque only when trigger is pulled?
        if (col.CompareTag("Ball") && hitting == false) {
            hit(col.attachedRigidbody, 1);
        }
    }

    private void hit(Rigidbody ballRigidbody, float hitForceMultiplier) {
        //step 1: set velocity
        ballRigidbody.velocity = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z) 
                               * m_BaseForceMultiplier
                               * hitForceMultiplier;

        //step 2: update ball state
        if (ball.GetComponent<Rigidbody>().isKinematic == true) {
            ball.GetComponent<Rigidbody>().isKinematic = false; // in case the ball is hit for the first time
            ball.GetComponent<BallScript>().State = BallScript.BallState.PlayerServe;
            ball.SetParent(null);
        }
        else {
            ball.GetComponent<BallScript>().State = BallScript.BallState.HitPlayerPaddle;
        }

        //step 3: set cooldown
        StartCoroutine(coolDownHitting(false, m_HittingCooldownTime));
    }

    #region Coroutines
    IEnumerator coolDownHitting(bool value, float delay) {
        hitting = true;
        yield return new WaitForSecondsRealtime(delay);
        hitting = false;
    }

    #endregion
}

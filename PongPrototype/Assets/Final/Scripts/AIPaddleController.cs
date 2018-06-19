using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPaddleController : MonoBehaviour {
    [Tooltip("Min distance to make paddle automatically track the ball.")]
    public float m_MinTrackingDistance = 4f;
    [Range(0.0f, 1.0f)]
    [Tooltip("How close the paddle is to the ball when using auto tracking.")]
    public float m_TrackingAccuracy = 1f;
    [Tooltip("Min distance to hit the ball.")]
    public float m_MinHittingDistance = 1.2f; 
    [Tooltip("For smooth moving of the paddle.")]
    public float m_PaddleSmoothTime = 0.1f; //change to reverse paddle speed?
    [Tooltip("Cooldown time for hitting.")]
    public float m_HittingCooldownTime = 1.8f;
    [Tooltip("Base force when hitting the ball.")]
    public float m_BaseForceMultiplier = 7f;

    private bool hitting = false;
    private Transform ball;
    private Transform player;
    private Transform aI;
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
        GameplayController.Singleton.AIPaddle = this.transform;

        ball = GameObject.FindWithTag("Ball") ? GameObject.FindWithTag("Ball").transform : null;
        player = GameObject.FindWithTag("Player") ? GameObject.FindWithTag("Player").transform : null;
        aI = GameObject.FindWithTag("AI") ? GameObject.FindWithTag("AI").transform : null;
    }

    void Update() {
        //step 0: freeze paddle if hitting (cooldown)
        if (hitting == true) {
            return;
        }

        //step 1: calculate base position
        float x = aI.position.x;
        float y = 2.5f;
        float z = aI.position.z + aI.transform.forward.z;

        //step 2: add automatic ball tracking to help the AI
        if (ball != null 
            && Vector3.Distance(aI.position, ball.position) < m_MinTrackingDistance 
            && ball.GetComponent<BallScript>().State == BallScript.BallState.HitAISide)
        {
            x *= 1f - m_TrackingAccuracy;
            x += ball.position.x * m_TrackingAccuracy;

            y *= 1f - m_TrackingAccuracy;
            y += ball.position.y * m_TrackingAccuracy;
            
            if (Vector3.Distance(transform.position, ball.position) < m_MinHittingDistance) {
                Vector3 target = player.position;
                target.x += Random.Range(-1.5f, 1.5f);
                target.y += Random.Range(2f, 3f);

                //step 4: aim paddle at Player with some noise
                transform.forward = target - transform.position;

                //step 5: hit the ball (with SmoothDamp) if in range
                z = ball.position.z;
            }
        }

        //step 3: set paddle position
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(x, y, z), ref m_CurrentVelocity, m_PaddleSmoothTime);
    }

    void OnTriggerEnter(Collider col) {
        //may be make paddle opaque only when trigger is pulled?
        if (col.CompareTag("Ball") && hitting == false)
            hit(col.attachedRigidbody, Random.Range(1f, 1.1f));
    }

    private void hit(Rigidbody ballRigidbody, float hitForceMultiplier) {
        //step 1: set velocity
        ballRigidbody.velocity = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z)
                               * m_BaseForceMultiplier
                               * hitForceMultiplier;

        //step 2: update ball state
        ball.GetComponent<BallScript>().State = BallScript.BallState.HitAIPaddle;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class AIStateMachine : MonoBehaviour {
    #region public variables
    public Transform player;
    public Transform objectTransform;
    public string petName = "lucky";
    public bool attention = false; 
     
    public float personalDistance;
    public float randomMoveAmplitude;
    public float randomMoveDuration;
    public float randomLookDuration;
    public float randomLookAmplitude;
    public float sitChance;
    #endregion

    #region private variables
    private AudioSource m_AudioSource;
    private Animator m_Animator;
    private HeadLookController m_HeadLookController;
    private AICharacterControl m_AICharacterControl;

    private IEnumerator lookCoroutine;
    private IEnumerator moveCoroutine;
    private IEnumerator attentionCoroutine;

    private bool lookCoroutineRunning = false;
    private bool moveCoroutineRunning = false;
    private bool attentionCoroutineRunning = false;

    private int A_idle = Animator.StringToHash("A_idle");
    private int B_idle = Animator.StringToHash("B_idle");
    private int C_idle = Animator.StringToHash("C_idle");
    #endregion

    [SerializeField]
    private Text debugText;

    void Start () {
        m_Animator = GetComponent<Animator>();
        m_HeadLookController = GetComponent<HeadLookController>();
        m_AICharacterControl = GetComponent<AICharacterControl>();
        m_AudioSource = GetComponent<AudioSource>();

        //setMoveTarget(player.position, 15f);
        //setMoveTarget(objectTransform, 15f);
        setRandomMoveTarget(); 
    }

    //maybe create setAction methods?
    public bool doCommand(int command) {
        int currentState = m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

        switch (command) {
            case 0:
                m_Animator.SetTrigger("Confused");
                break;
            case 1:
                // check if standing first
                if (currentState != A_idle)
                    return false;

                m_Animator.SetTrigger("Sit");
                break;
            case 2:
                // check if sitting first
                if (currentState != B_idle)
                    return false;

                m_Animator.SetTrigger("Paw"); 
                break;
            case 3:
                // check if standing/sitting first
                if (currentState != A_idle && currentState != B_idle)
                    return false;

                m_Animator.SetTrigger("Down");
                break;
            case 4:
                // check if sitting/down first
                if (currentState != B_idle && currentState != C_idle)
                    return false;

                m_Animator.SetTrigger("Up");
                break;
            default:
                break;
        }
        setLookTarget(player.position, 2f);
        return true;
    }

    public void setBark()
    {
        m_Animator.SetTrigger("Bark");
        m_AudioSource.Play();
    }

    public void prepareForMoving(float duration) {
        //step 1: make AI slowly turn before moving 
        if (m_AICharacterControl.agent != null) {
            m_AICharacterControl.agent.speed = 0.2f;
            StartCoroutine(delaySetSpeed(1.0f, 2f + Random.Range(-1.0f, 1.0f)));
        }

        //step 2A: look at target
        //step 2B: look forward
        if (m_AICharacterControl.targetTransform)
            setLookTarget(m_AICharacterControl.targetTransform, duration / 1.6f);
        else
            setLookTarget(Vector3.zero, duration / 1.6f); 

        //step 3: get up before moving!
        int currentState = m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        if (currentState == B_idle || currentState == C_idle)
            m_Animator.SetTrigger("Up");
    }

    #region Move Methods
    public void setMoveTarget(Transform target, float duration) {
        m_AICharacterControl.targetTransform = target;

        prepareForMoving(duration);

        setRandomMoveTarget(duration); //duration becomes delay for next move
    }

    public void setMoveTarget(Vector3 v, float duration) {
        m_AICharacterControl.targetTransform = null;
        m_AICharacterControl.target = v; 

        if (v != Vector3.zero)
            prepareForMoving(duration);

        setRandomMoveTarget(duration); //duration becomes delay for next move
    }

    //use sit chance? use stop chance? <- energy, mood, personality
    //if sitting, don't move (player call name, say 'sit' or 'stay', move away, then AI should not chase), else AI should follow?
    public void setRandomMoveTarget() {
        Vector3 v;

        if (attention)
            v = Vector3.zero;
        else {
            v = new Vector3(
                player.position.x + Random.Range(-randomMoveAmplitude, randomMoveAmplitude),
                transform.position.y,
                player.position.z + Random.Range(-randomMoveAmplitude, randomMoveAmplitude));
        }

        setMoveTarget(v, randomMoveDuration);
    }

    public void setRandomMoveTarget(float delay)
    {
        if (moveCoroutineRunning)
            StopCoroutine(moveCoroutine);

        moveCoroutine = delaySetRandomMoveTarget(delay);
        StartCoroutine(moveCoroutine);
    }
    #endregion

    #region Look Methods
    public void setLookTarget(Transform target, float duration) {
        m_HeadLookController.targetTransform = target;

        setRandomLookTarget(duration); 
    }

    public void setLookTarget(Vector3 v, float duration) {
        if (v == Vector3.zero)
            m_HeadLookController.hasTarget = false;
        else
            m_HeadLookController.hasTarget = true;

        m_HeadLookController.targetTransform = null;
        m_HeadLookController.target = v;

        setRandomLookTarget(duration);
    }

    public void setRandomLookTarget() {
        Vector3 v;
        float random = Random.Range(0, 1f);

        if (attention) {
            if (objectTransform)
            {
                if (random < 0.7)
                {
                    setLookTarget(objectTransform, randomLookDuration);
                }
                else {
                    v = player.position;
                    setLookTarget(v, randomLookDuration);
                }
            }
            else { 
                if (random < 0.7f)
                {
                    v = player.position;
                }
                else
                {
                    v = new Vector3(
                        transform.forward.x + Random.Range(-2.0f, 2.0f),
                        transform.forward.y + Random.Range(-2.0f, 2.0f),
                        transform.forward.z + Random.Range(-2.0f, 2.0f));
                }

                setLookTarget(v, randomLookDuration);
            }
        } else {
            // check if player is looking at AI? yes = look back
            if (random < 0.5f)
            {
                v = player.position;
            }
            else
            {
                // use percentage instead?
                v = new Vector3(
                    transform.forward.x + Random.Range(-randomLookAmplitude, randomLookAmplitude),
                    transform.forward.y + Random.Range(-randomLookAmplitude, randomLookAmplitude),
                    transform.forward.z + Random.Range(-randomLookAmplitude, randomLookAmplitude));
            }

            setLookTarget(v, randomLookDuration);
        } 
    }

    public void setRandomLookTarget(float delay)
    {
        if (lookCoroutineRunning)
            StopCoroutine(lookCoroutine);

        lookCoroutine = delaySetRandomLookTarget(delay);
        StartCoroutine(lookCoroutine);
    }
    #endregion

    public void setStoppingDistance(float value, float duration) {
        if (m_AICharacterControl.agent == null)
            return;

        m_AICharacterControl.agent.stoppingDistance = value;

        if (value != 0.5f && duration > 0)
            StartCoroutine(delaySetStoppingDistance(0.5f, duration));
    }

    public void setAttention(bool value, float duration) {
        attention = value;

        if (value == true && duration > 0)
            setNextAttention(duration);
    }

    public void setNextAttention(float delay) {
        if (attentionCoroutineRunning)
            StopCoroutine(attentionCoroutine);
        
        attentionCoroutine = delaySetAttention(false, delay);
        StartCoroutine(attentionCoroutine);
    }

    #region Calculation methods
    // if energertic = less duration (more frequency)
    private float calculateMoveDuration(float duration) {
        float factor = Random.Range(0.8f, 1.2f);
        return duration * factor;
    }

    private float calculateLookDuration(float duration) {
        float factor = Random.Range(0.8f, 1.2f);
        return duration * factor;
    }
    #endregion

    #region Coroutines
    IEnumerator delaySetRandomMoveTarget(float delay) {
        moveCoroutineRunning = true;
        yield return new WaitForSecondsRealtime(calculateMoveDuration(delay));
        moveCoroutineRunning = false;

        setRandomMoveTarget();
    }

    IEnumerator delaySetRandomLookTarget(float delay) {
        lookCoroutineRunning = true;
        yield return new WaitForSecondsRealtime(calculateLookDuration(delay));
        lookCoroutineRunning = false;

        setRandomLookTarget();
    } 

    IEnumerator delaySetAttention(bool value, float delay) {
        attentionCoroutineRunning = true;
        yield return new WaitForSecondsRealtime(delay);
        attentionCoroutineRunning = false;

        attention = value;
        setRandomMoveTarget();
    }

    IEnumerator delaySetSpeed(float value, float delay) {
        yield return new WaitForSecondsRealtime(delay);

        m_AICharacterControl.agent.speed = value;
    }

    IEnumerator delaySetStoppingDistance(float value, float delay) {
        yield return new WaitForSecondsRealtime(delay);

        m_AICharacterControl.agent.stoppingDistance = value;
    }
    #endregion
} 
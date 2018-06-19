using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Vector3 target = Vector3.zero;
        public Transform targetTransform;  

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;
        }

        private void Update() {
            GetComponent<Rigidbody>().velocity = Vector3.zero; //fixes collision issues

            //case 1: target Transform is given
            if (targetTransform != null)
            {
                agent.SetDestination(targetTransform.position);

                float distance = agent.pathPending ? distance = Vector3.Distance(transform.position, target) : distance = agent.remainingDistance;

                //case 1.1: keep moving
                if (distance > agent.stoppingDistance)
                {
                    character.Move(agent.desiredVelocity, false, false);
                }
                //case 1.2: arrived at an Item
                else if (targetTransform.CompareTag("Item"))
                {
                    //case 1.2.1: not taken by another AI and not held by player 
                    if (targetTransform.parent.name != "Placeholder" && targetTransform.parent.name != "GearVrController")
                    {
                        //put in the mouth
                        targetTransform.GetComponent<Rigidbody>().isKinematic = true;
                        targetTransform.GetComponent<Collider>().isTrigger = true;

                        Transform mouth = transform.Find("cu_puppy_spine/cu_puppy_spine1/cu_puppy_spine2/cu_puppy_neck0/cu_puppy_neck1/cu_puppy_head/Placeholder");

                        if (mouth)
                        {
                            targetTransform.GetComponent<Rigidbody>().transform.position = mouth.position;
                            targetTransform.GetComponent<Rigidbody>().transform.SetParent(mouth);
                        }
                    }

                    AIStateMachine AI = GetComponent<AIStateMachine>();
                    AI.setMoveTarget(AI.player.position, 15f);
                }
                // case 1.3: arrived at other types of target
                else
                {
                    character.Move(Vector3.zero, false, false);
                }
            }
            //case 2: target Vector is given
            else if (target.sqrMagnitude > 0)
            {
                agent.SetDestination(target);

                float distance = agent.pathPending ? distance = Vector3.Distance(transform.position, target) : distance = agent.remainingDistance;

                //case 2.1: keep moving
                if (distance > agent.stoppingDistance)
                {
                    character.Move(agent.desiredVelocity, false, false);
                }
                //case 2.2: arrived at target
                else
                {
                    character.Move(Vector3.zero, false, false);
                } 
            }
            //case 3: not moving
            else
            {
                character.Move(Vector3.zero, false, false);
            } 
        } 
    }
}

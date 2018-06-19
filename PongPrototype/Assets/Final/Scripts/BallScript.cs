using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {
    private BallState state = BallState.Initialized;

    #region Setters & Getters
    public BallState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }
    #endregion

    private void OnCollisionEnter(Collision collision) {
        BallState newState;

        switch (collision.gameObject.name) {
            case "Player Side":
                newState = BallState.HitPlayerSide;

                if (state == newState || state == BallState.HitPlayerPaddle) {
                    GameplayController.Singleton.AIScored();
                } else {
                    state = newState;
                }

                break;

            case "AI Side":
                newState = BallState.HitAISide;

                if (state == newState || state == BallState.HitAIPaddle) {
                    GameplayController.Singleton.PlayerScored();
                } else if (state == BallState.PlayerServe) {
                    GameplayController.Singleton.AIScored();
                } else {
                    state = newState;
                }

                break;

            case "Wall":
                if (state == BallState.HitPlayerSide || state == BallState.HitPlayerPaddle || state == BallState.PlayerServe) {
                    GameplayController.Singleton.AIScored();
                } else {
                    GameplayController.Singleton.PlayerScored();
                }

                break;

            default:
                break;
        }
    }

    public enum BallState {
        Initialized,              // initial state
        PlayerServe,              // served by Player
        HitPlayerSide,            // hit the table on the player side
        HitPlayerPaddle,
        HitAISide,                // hit the table on the AI side
        HitAIPaddle
    }
}

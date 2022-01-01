using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;
        public PCG pcg;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;

        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;

        private bool flipAfterDrag = false;

        /*internal new*/
        public Collider2D collider2d;

        /*internal new*/
        public AudioSource audioSource;
        public Health health;

        public bool vulnerable = true;
        public bool jump;
        public Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = GetModel<PlatformerModel>();
        private int bufferJumpKeyDown = 0;
        private int bufferJumpKeyUp = 0;
        private bool actionReceived = false;
        public Bounds Bounds => collider2d.bounds;
        public float reward = 0;
        public float timePenalty=1f;
        public float rightRecordReward=2f;
        private float xRecord = 0;
        public float wallJumpReward;
        public void Update()
        {
            //Workaround mlagents only working with FixedUpdate
            //If the a button up or down event occurs, don't update until actionReceived
            if (bufferJumpKeyDown == 0)
                bufferJumpKeyDown = Input.GetButtonDown("Jump") ? 1 : 0;
            if (bufferJumpKeyUp == 0)
                bufferJumpKeyUp = Input.GetButtonUp("Jump") ? 1 : 0;
            if (actionReceived)
            {
                bufferJumpKeyDown = Input.GetButtonDown("Jump") ? 1 : 0;
                bufferJumpKeyUp = Input.GetButtonUp("Jump") ? 1 : 0;
                actionReceived = false;
            }
        }

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        public override void OnEpisodeBegin()
        {
            pcg.BuildLevel();
            Schedule<PlayerSpawn>();
            xRecord = 0;
            foreach (var t in health.hearts)
            {
                t.gameObject.SetActive(true);
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            actionReceived = true;
            var continuousActionsOut = actionsOut.ContinuousActions;
            var discreteActionsOut = actionsOut.DiscreteActions;
            continuousActionsOut[0] = Input.GetAxis("Horizontal");
            discreteActionsOut[0] = bufferJumpKeyDown;
            discreteActionsOut[1] = bufferJumpKeyUp;
            bufferJumpKeyDown = Input.GetButtonDown("Jump") ? 1 : 0;
            bufferJumpKeyUp = Input.GetButtonUp("Jump") ? 1 : 0;
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            var jumpKeyDown = actionBuffers.DiscreteActions[0];
            var jumpKeyUp = actionBuffers.DiscreteActions[1];
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt") ||
                animator.GetCurrentAnimatorStateInfo(0).IsTag("dead") ||
                animator.GetCurrentAnimatorStateInfo(0).IsTag("spawn"))
            {
                controlEnabled = false;
                vulnerable = false;
            }
            else
            {
                controlEnabled = true;
                vulnerable = true;
            }

            if (controlEnabled)
            {
                move.x = actionBuffers.ContinuousActions[0];
                if (jumpState == JumpState.Grounded && jumpKeyDown == 1 ||
                    IsDragging && jumpKeyDown == 1)
                    jumpState = JumpState.PrepareToJump;
                else if (jumpKeyUp == 1)
                {
                    stopJump = true;
                    // Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move.x = 0;
            }

            
            UpdateJumpState();
            targetVelocity = Vector2.zero;
            ComputeVelocity();
            UpdateRL();
            //Reward new record in the horizontal axis
            if (transform.position.x > xRecord)
            {
                AddReward(rightRecordReward);
                xRecord = transform.position.x;
            }
                
            
            //Penalize for each frame (favor faster agents)
            AddReward(-timePenalty);
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        // Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }

                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        // Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }

                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    JumpedOff = false;
                    break;
            }
        }

        protected void ComputeVelocity()
        {
            if (flipAfterDrag)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
                flipAfterDrag = false;
            }

            //Flip horizontal move
            if (move.x > 0.1f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.1f)
                spriteRenderer.flipX = true;
            //Flip dragging 
            if (currentNormal.x > 0.1f && IsDragging)
                spriteRenderer.flipX = true;
            else if (currentNormal.x < -0.1f && IsDragging)
                spriteRenderer.flipX = false;

            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (jump && IsDragging)
            {
                JumpedOff = true;
                IsDragging = false;
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                velocity.x = currentNormal.x * jumpTakeOffSpeed * model.jumpModifier;
                AddReward(wallJumpReward);
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            //Wall jump forced flip (velocity is not completely overriden by move)
            if (velocity.x > 0.05f)
                spriteRenderer.flipX = false;
            else if (velocity.x < -0.05f)
                spriteRenderer.flipX = true;


            animator.SetBool("grounded", IsGrounded);
            animator.SetBool("wallJump", JumpedOff);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            if (animator.GetBool("dragging") && !IsDragging)
                flipAfterDrag = true;
            animator.SetBool("dragging", IsDragging);
            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed,
        }
    }
}
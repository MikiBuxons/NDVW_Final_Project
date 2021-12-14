using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Implements game physics for some in game entity.
    /// </summary>
    public class KinematicObject : MonoBehaviour
    {
        /// <summary>
        /// The minimum normal (dot product) considered suitable for the entity sit on.
        /// </summary>
        public float minGroundNormalY = .65f;

        /// <summary>
        /// A custom gravity coefficient applied to this entity.
        /// </summary>
        public float gravityModifier = 1f;
        public float drag = 0.5f;

        /// <summary>
        /// The current velocity of the entity.
        /// </summary>
        public Vector2 velocity;

        /// <summary>
        /// Is the entity currently sitting on a surface?
        /// </summary>
        /// <value></value>
        public bool IsGrounded { get; private set; }
        public bool IsDragging;
        public bool JumpedOff=false;
        public bool controlEnabled = true;
        protected Vector2 targetVelocity;
        protected Vector2 groundNormal;
        public Vector2 currentNormal;
        protected Rigidbody2D body;
        protected ContactFilter2D contactFilter;
        protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

        protected const float minMoveDistance = 0.001f;
        protected const float shellRadius = 0.01f;
        public LayerMask whatIsSolid;

        /// <summary>
        /// Bounce the object's vertical velocity.
        /// </summary>
        /// <param name="value"></param>
        public void Bounce(float value)
        {
            velocity.y = value;
        }

        /// <summary>
        /// Bounce the objects velocity in a direction.
        /// </summary>
        /// <param name="dir"></param>
        public void Bounce(Vector2 dir)
        {
            velocity.y = dir.y;
            velocity.x = dir.x;
        }

        /// <summary>
        /// Teleport to some position.
        /// </summary>
        /// <param name="position"></param>
        public void Teleport(Vector3 position)
        {
            body.position = position;
            velocity *= 0;
            body.velocity *= 0;
        }

        protected virtual void OnEnable()
        {
            body = GetComponent<Rigidbody2D>();
            body.isKinematic = true;
        }

        protected virtual void OnDisable()
        {
            body.isKinematic = false;
        }

        protected virtual void Start()
        {
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            contactFilter.useLayerMask = true;
        }

        protected virtual void Update()
        {
            targetVelocity = Vector2.zero;
            ComputeVelocity();
        }

        protected virtual void ComputeVelocity()
        {

        }

        protected virtual void FixedUpdate()
        {
            //if already falling, fall faster than the jump speed, otherwise use normal gravity.
            if (velocity.y < 0)
            {
                if (IsDragging)
                    velocity.y -= velocity.y * drag;
                velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
            }
            else
                velocity += Physics2D.gravity * Time.deltaTime;

            if (!JumpedOff && controlEnabled) 
                velocity.x= targetVelocity.x;
                //velocity.x = 0.95f * velocity.x + 0.05f * targetVelocity.x;
            else if (JumpedOff)
                velocity.x = 0.97f * velocity.x + 0.03f * targetVelocity.x;
            if (velocity.x * currentNormal.x > 0)
                IsDragging = false;
            Debug.DrawRay(transform.position, transform.right * 0.5f, Color.red);
            Debug.DrawRay(transform.position, transform.right * 0.5f, Color.red);
            RaycastHit2D hitinfo_upleft = Physics2D.Raycast(transform.position , Vector2.left, 0.5f, whatIsSolid);
            RaycastHit2D hitinfo_downleft = Physics2D.Raycast(transform.position+Vector3.down*0.1f , Vector2.left, 0.5f, whatIsSolid);
            RaycastHit2D hitinfo_upright = Physics2D.Raycast(transform.position , Vector2.right, 0.5f, whatIsSolid);
            RaycastHit2D hitinfo_downright = Physics2D.Raycast(transform.position+Vector3.down*0.1f , Vector2.right, 0.5f, whatIsSolid);
            if (hitinfo_upleft.collider == null && hitinfo_upright.collider == null && hitinfo_downleft.collider == null && hitinfo_downright.collider == null) 
            {
                IsDragging = false;
            }

            IsGrounded = false;


            var deltaPosition = velocity * Time.deltaTime;

            var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

            var move = moveAlongGround * deltaPosition.x;

            PerformMovement(move, false);

            move = Vector2.up * deltaPosition.y;

            PerformMovement(move, true);

        }

        void PerformMovement(Vector2 move, bool yMovement)
        {
            var distance = move.magnitude;

            if (distance > minMoveDistance)
            {
                //check if we hit anything in current direction of travel
                var count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
                for (var i = 0; i < count; i++)
                {
                    currentNormal = hitBuffer[i].normal;

                    //is this surface flat enough to land on?
                    if (currentNormal.y > minGroundNormalY)
                    {
                        IsGrounded = true;
                        IsDragging = false;
                        // if moving up, change the groundNormal to new surface normal.
                        if (yMovement)
                        {
                            groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }
                    if (IsGrounded)
                    {
                        //how much of our velocity aligns with surface normal?
                        var projection = Vector2.Dot(velocity, currentNormal);
                        if (projection < 0)
                        {
                            //slower velocity if moving against the normal (up a hill).
                            velocity = velocity - projection * currentNormal;
                        }
                    }
                    else if (currentNormal.normalized.x > 0.95f || currentNormal.normalized.x < -0.95f)
                    {
                        //We are airborne, but hit a wall add drag if falling.
                        IsDragging = true;
                        JumpedOff = false;
                    }
                    //remove shellDistance from actual move distance.
                    var modifiedDistance = hitBuffer[i].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }
            body.position = body.position + move.normalized * distance;
        }

    }
}
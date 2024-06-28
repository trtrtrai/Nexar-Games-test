using UnityEngine;

public class ZombieAnimationControl : MonoBehaviour
{
    [SerializeField] protected Zombie parent;

    [SerializeField] protected Animator animator;
    [SerializeField] protected float turnAroundSpeed = 1f;

    [SerializeField] protected bool humanReact;
    [SerializeField] protected float rotateSpeed = 3f;
    [SerializeField] protected Transform chest;
    [SerializeField] protected Transform head;
    
    [SerializeField] protected bool isLooking;
    [SerializeField] protected Transform headOrigin;
    [SerializeField] protected float resetTimer;
    [SerializeField] protected Quaternion lastRotation;
    
    [Header("Old function")]
    [SerializeField] protected Transform previousTarget;
    [SerializeField] protected float rightRotateAngle;
    [SerializeField] protected float upRotateAngle;
    [SerializeField] protected float curRightRotate;
    [SerializeField] protected float curUpRotate;

    protected void Awake()
    {
        this.parent.ZombieTarget.TargetChange += ZombieTargetOnTargetChange;
        this.previousTarget = this.parent.transform;
    }

    private void ZombieTargetOnTargetChange()
    {
        this.previousTarget = this.parent.ZombieTarget.Targeted ? this.parent.ZombieTarget.Targeted : this.parent.transform;
    }

    protected void Update()
    {
        this.RayToTarget();
        this.AnimationUpdate();
        this.TurnToDirection();
    }

    protected virtual void RayToTarget()
    {
        Debug.DrawRay(this.head.position, this.head.forward, Color.red);
        if (this.parent.ZombieTarget.Targeted)
        {
            Debug.DrawLine(this.head.position, this.parent.ZombieTarget.Targeted.position, Color.green);
        }
    }

    protected void LateUpdate()
    {
        this.UpdateRotateAngle();
        if (this.humanReact)
        {
            this.RotateHeadToTargetedLikeHuman();
        }
        else
        {
            this.RotateHeadToTargeted();
        }
    }
    
    protected virtual void UpdateRotateAngle()
    {
        Vector3 headPosVector = this.head.position;
        Vector3 targetVector = this.previousTarget.position - this.parent.transform.position;
        
        // Get expect rotate angle
        this.rightRotateAngle = this.GetRightRotateAngle(headPosVector, targetVector);
        this.upRotateAngle = this.GetUpRotateAngle(headPosVector, targetVector);
        
        // Get angle change amount per frame (add or subtract)
        float scaleR = Mathf.Sign(this.rightRotateAngle - this.curRightRotate) * 7f;
        float scaleU = Mathf.Sign(this.upRotateAngle - this.curUpRotate) * 7f;
        
        // Clamp right rotate between current and expect angle
        this.curRightRotate = this.rightRotateAngle > this.curRightRotate ? 
            Mathf.Clamp(this.curRightRotate + scaleR, this.curRightRotate, this.rightRotateAngle) : 
            Mathf.Clamp(this.curRightRotate + scaleR, this.rightRotateAngle, this.curRightRotate);

        // Clamp up rotate between current and expect angle
        curUpRotate = upRotateAngle > curUpRotate ? 
            Mathf.Clamp(curUpRotate + scaleU, curUpRotate, upRotateAngle) : 
            Mathf.Clamp(curUpRotate + scaleU, upRotateAngle, curUpRotate);
    }

    protected virtual void RotateHeadToTargetedLikeHuman()
    {
        this.RotateChest(new Vector3(-curRightRotate, 0f, 6.456f));
        //this.RotateHead(new Vector3(0f, 0f, curUpRotate));
        this.RotateHead(); // Use new Rotate head
    }

    protected virtual void RotateHeadToTargeted()
    {
        //this.RotateHead(new Vector3(-curRightRotate, 0f, curUpRotate));
        this.RotateHead(); // Use new Rotate head
    }

    protected virtual void RotateHead(Vector3 eulerAngle)
    {
        /*
         * OLD Function
         * - Rotate head by right and up axis of head (in this case right axis of zombie head is vector forward (Z) of head and
         * - up axis is vector negative right (-X))
         * ==> Wrong direction, don't smooth
         */
        this.head.localEulerAngles = eulerAngle;
    }
    
    protected virtual void RotateHead()
    {
        /*
         * NEW Function
         * - Create a new head with normal rotate in the model
         * - Create an object child of the head. It will rotate exactly rotate of real head (real head write rotate of 
         * this object with ZombieHeadCopyRotation script)
         * - Update rotation of new head and real head also update with right direction
         * - Use Quaternion.Slerp to smooth rotate of the head
         */
        if (this.parent.ZombieTarget.Targeted) // Zombie found target
        {
            if (!this.isLooking) // Is first time looking at the target?
            {
                this.isLooking = true;
                this.lastRotation = this.head.rotation;
            }
            // Create Quaternion look at the target
            Quaternion targetRotation = Quaternion.LookRotation(this.parent.ZombieTarget.Targeted.position - this.head.position);
            // Slerp rotation to smooth action
            this.lastRotation = Quaternion.Slerp(this.lastRotation, targetRotation, this.rotateSpeed * Time.deltaTime);
            
            this.head.rotation = this.lastRotation;
            this.resetTimer = 0.5f;
        }
        else if (isLooking) // Zombie doesn't focus the target anymore
        {
            // Slerp rotation to smooth action with origin head (rotate to normal state)
            this.lastRotation = Quaternion.Slerp(this.lastRotation, this.headOrigin.rotation, this.rotateSpeed * Time.deltaTime);

            this.head.rotation = this.lastRotation;
            // Time limit to rotate
            this.resetTimer -= Time.deltaTime;
            
            if (this.resetTimer > 0f) return;
            // Return normal state, stop looking
            this.head.rotation = this.headOrigin.rotation;
            this.isLooking = false;
        }
    }

    protected virtual void RotateChest(Vector3 eulerAngle)
    {
        this.chest.localEulerAngles = eulerAngle;
    }

    protected virtual float GetRightRotateAngle(Vector3 headPos, Vector3 targetVector)
    {
        headPos.y = 0;
        targetVector.y = 0;
        float sign = Vector3.Dot(targetVector, this.parent.transform.right);
        return sign * Vector3.Angle(headPos, targetVector);
    }
    
    protected virtual float GetUpRotateAngle(Vector3 headPos, Vector3 targetVector)
    {
        headPos.x = 0;
        targetVector.x = 0;
        return Vector3.Angle(headPos, targetVector);
    }

    protected virtual void AnimationUpdate()
    {
        this.animator.SetBool("isMoving", this.parent.Movement.IsMoving);
    }

    protected virtual void TurnToDirection()
    {
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, this.parent.Movement.Direction, turnAroundSpeed * Time.deltaTime, 0f);
        this.parent.transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public virtual void ReactChange(bool react)
    {
        this.humanReact = react;
    }
}
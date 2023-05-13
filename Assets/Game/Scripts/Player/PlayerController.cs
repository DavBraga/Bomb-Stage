using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public StateMachine stateMachine{ get; private set;}
    public IdleState idleState{ get; private set;}
    public WalkingState walkingState{ get; private set;}
    public DeadState deadState{get; private set;}
    public OnAirState onAirState{ get; private set;}

    public Vector2 inputMovmentVector{ get; private set;}
    public Rigidbody myRigidbody{ get; private set;}
    public  Animator animator{ get; private set;}

    private bool grounded= true;

    float atackCooldown= 0;
    float atackInterval= .2f;

    [SerializeField] Collider thisCollider;

    float fVelocityRate;

    [SerializeField] float movmentSpeed = 10f;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float airMovmentSpeedModifier =0.25f;
    [SerializeField] float jumpPower=10f;
    [SerializeField] float stopForce=5f;

    private void Start()
    {
        InitializeStateMachine();
        myRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        thisCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if(GameManager.Instance.currentState != GameManager.GameStates.Active)
        {
            animator.SetTrigger("tCheer");
            return;
        }
         

        atackCooldown -= Time.deltaTime;

        if(ReadAtackInput()) Chop();

        inputMovmentVector = ReadMovmentInput();
        fVelocityRate = 0;
        if(!inputMovmentVector.isZero()){
            animator.SetTrigger("GetReady");
            fVelocityRate = myRigidbody.velocity.magnitude;
            fVelocityRate /=movmentSpeed;
        }
        animator.SetFloat("fVelocity", fVelocityRate);
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        if(GameManager.Instance.currentState != GameManager.GameStates.Active) return;
        stateMachine.FixedUpdate();
        LimitMovmentSpeed();
        if(inputMovmentVector.isZero())StopFaster(stopForce);
    }

       private void InitializeStateMachine()
    {
        stateMachine = new StateMachine();
        idleState = new IdleState(this);
        walkingState = new WalkingState(this);
        onAirState = new OnAirState(this, airMovmentSpeedModifier);
        deadState = new DeadState(this);
        stateMachine.ChangeState(idleState);
    }

    private void StopFaster(float stopPower=1f)
    {
        // using velocity.x + velocityz to avoid costly ".magnitude" operation
        if(Mathf.Abs(myRigidbody.velocity.x+myRigidbody.velocity.z)<.5f) return;

        Vector3 horizontalVelocityWithoutY = new Vector3(myRigidbody.velocity.x, 0f, myRigidbody.velocity.z);
        myRigidbody.AddForce(-horizontalVelocityWithoutY.normalized * stopPower);
    }

    private void LimitMovmentSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(myRigidbody.velocity.x, 0f, myRigidbody.velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            myRigidbody.velocity = new Vector3(horizontalVelocity.x, myRigidbody.velocity.y, horizontalVelocity.z);
        }
    }

    private void LateUpdate() {
        stateMachine.LateUpdate();
    }
    
 
    public void PlayerMovment(float intensity =1)
    {
        Vector3 movmentVector = InputToV3()*GetMovmentSpeed()*intensity;
        movmentVector = GetForward()* movmentVector ;
        myRigidbody?.AddForce(movmentVector,ForceMode.Force);
    }

    public void Jump()
    {
        if(!grounded)  return;
        animator.SetTrigger("tJump");
        myRigidbody.AddForce(Vector3.up*jumpPower, ForceMode.Impulse);
    }

    public void Chop()
    {
        // handle cooldown
        if(atackCooldown>0) return;
        atackCooldown = atackInterval;

        // play animation
        animator.SetTrigger("tAtack");
        
    }

    public void Die()
    {
        if(stateMachine.currentState!= deadState)
            stateMachine.ChangeState(deadState);

        GameManager.Instance.EndLevel();
    }
    
    public void  RotateBodyToFace(){

        if(inputMovmentVector.isZero()) return;

        Quaternion q1 = Quaternion.LookRotation(InputToV3(),Vector3.up); 
        Quaternion q2 = GetForward();
        Quaternion toRotation = q1*q2;
        Quaternion smoothRotation = Quaternion.LerpUnclamped(transform.rotation, toRotation,0.15f);
        myRigidbody.MoveRotation(smoothRotation);

    }

    public bool ReadJumpInput()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            return true;
        }
        return false;
    }

    public bool ReadAtackInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            return true;
        }
        return false;
    }
    
    private  Vector2 ReadMovmentInput()
    {
        bool isUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool isDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool isLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool isRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        float inputZ = isUp ? 1 : isDown ? -1 : 0;
        float inputX = isRight ? 1 : isLeft ? -1 : 0;
        return (new Vector2(inputX,inputZ));
    }
    public float GetMovmentSpeed()
    {
        return movmentSpeed;
    }
    public Quaternion GetForward()
    {
        float eulerY =Camera.main.transform.eulerAngles.y; 
        return Quaternion.Euler(0,eulerY,0);
    }

    public Vector3 InputToV3()
    {
        return new Vector3(inputMovmentVector.x,0, inputMovmentVector.y);
    }

    public bool isGrounded()
    {
        Vector3 direction = Vector3.down;
        Bounds bounds = thisCollider.bounds;
        Vector3 origin = transform.position + new Vector3(0,bounds.size.y,0);
        float radius = bounds.size.x * 0.33f;
        float maxDistance = bounds.size.y;
        Vector3 spherePosition = direction* maxDistance + origin;


        if(Physics.SphereCast (origin,radius,direction,out var hitInfo,maxDistance))
        {
            GameObject hitObject = hitInfo.transform.gameObject;
            if(hitObject.CompareTag("Walkable"))
            {
                animator.SetBool("bOnAir", false);
                return true;
            }        
        }
        animator.SetBool("bOnAir", true);
        return false;
    }
    // For test purpose Only
    // private void OnDrawGizmos() {

        
    //     Vector3 direction = Vector3.down;
    //     Bounds bounds = thisCollider.bounds;
    //     Vector3 origin = transform.position + new Vector3(0,bounds.size.y,0);
    //     float radius = bounds.size.x * 0.33f;
    //     float maxDistance = bounds.size.y;// *0.3f;
    //     Vector3 spherePosition = direction* maxDistance + origin;

    //     Gizmos.color = grounded? Color.green : Color.red;
    //     Gizmos.DrawSphere(spherePosition, radius );
    // }
}

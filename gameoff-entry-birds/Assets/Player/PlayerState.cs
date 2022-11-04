using UnityEngine;

public class PlayerState : MonoBehaviour
{
    protected PlayerStateMachine Machine { get; set; }
    protected PlayerDodge DodgeState { get; set; }
    protected PlayerMovementState MovementState { get; set; }
    protected ObjectPool BulletPool { get; set; }
    
    protected Rigidbody Rigidbody { get; set; }
    
    // Start is called before the first frame update
    protected void Awake()
    {
        this.Machine = GetComponent <PlayerStateMachine> ();
        this.Rigidbody = GetComponent <Rigidbody> ();
        this.MovementState = GetComponent <PlayerMovementState> ();
        this.DodgeState = GetComponent <PlayerDodge> ();
        this.BulletPool = GameObject.Find ("PlayerBulletPool").GetComponent <ObjectPool> ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnStateEnter()
    {
        
    }

    public virtual void OnStateExit()
    {
        
    }
}

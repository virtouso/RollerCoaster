using UnityEngine;
using UnityEngine.Events;

public class LeadingCabin : MonoBehaviour
{
    public static LeadingCabin _instance;

    [SerializeField] private bool DebugSpeed;

    #region path variables

    [SerializeField] public float GoalDistanceThreshold;
    [SerializeField] public float TriggerHeight;

    [SerializeField] private BezierMaster.BezierMaster bezier;
    [SerializeField] private int pathResolution;
    [SerializeField] private int GoalIndex;
    [SerializeField] private int PassedIndex;
    [SerializeField] private float SurfaceCosine;
    private Vector3[] CurvePoints;
    #endregion

    #region physics variables
    [SerializeField] public Vector3 DesiredVelocity;
    [SerializeField] public float EnterLoopVelocity;
    [SerializeField] public float MovingLoopRadius;
    [SerializeField] public float DesiredVelocityMagnitude;

    private Rigidbody rigidBody;
    [SerializeField] private float F;
    [SerializeField] private float MotorForce;
    [SerializeField] private float SpeedToDesiredVelocity;
    [SerializeField] private float SpeedToDesiredRotation;
    [SerializeField] private float GravityFixedEnergy;

    [SerializeField] private float MaximumVelocity;
    #endregion



    #region unity Callbacks
    private void Awake()
    {
        CurvePoints = bezier.GetPath(pathResolution);
        _instance = this;
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

        UpdateIndex();
        if (CurrentState != null)
        {
            CurrentState.Invoke();
        }

        GetDesiredVelocity();
        GetDesiredRotation();
        if (rigidBody.velocity.magnitude > MaximumVelocity)
        {
            rigidBody.velocity = DesiredVelocity * MaximumVelocity;
        }
        if (DebugSpeed)
        {
            Debug.Log(rigidBody.velocity.magnitude);
        }
    }

    #endregion


    #region Utilty Functions 
    public void UpdateIndex()
    {
        if (Vector3.Distance(transform.position, CurvePoints[GoalIndex]) < GoalDistanceThreshold)
        {
            if (GoalIndex < CurvePoints.Length - 1)
            {
                GoalIndex++;
                PassedIndex = GoalIndex - 1;
            }

            else
            {
                CurrentState.RemoveAllListeners();
                CurrentState.AddListener(Idle);
            }
        }
        DesiredVelocity = (CurvePoints[GoalIndex] - transform.position).normalized * DesiredVelocityMagnitude;

    }

    public void GetDesiredVelocity()
    {
        rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, DesiredVelocity, SpeedToDesiredVelocity);
    }

    public void GetDesiredRotation()
    {
        Ray ray = new Ray(transform.position + transform.up * 2, -transform.up);

        Physics.Raycast(ray, out RaycastHit hit);

        if (hit.collider != null)
        {
            if (hit.transform.tag == "subrail")
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, hit.transform.rotation, SpeedToDesiredRotation);
            }
        }
    }
    #endregion


    #region Cabin States

    public UnityEvent CurrentState;
    public void MovingMotorized()
    {
        Ray ray = new Ray(transform.position + transform.up * 2, -transform.up);
        Debug.DrawRay(transform.position + transform.up * 2, -transform.up, Color.red);
        Physics.Raycast(ray, out RaycastHit hit);

        if (hit.collider != null)
        {
            if (hit.transform.tag == "subrail")
            {
                Vector3 horizon = Quaternion.Euler(0, hit.transform.eulerAngles.y, hit.transform.eulerAngles.z) * Vector3.forward;
                SurfaceCosine = Vector3.Dot(hit.transform.forward, horizon) / (hit.transform.forward.magnitude * horizon.magnitude);

            }
        }

        DesiredVelocityMagnitude = MotorForce / rigidBody.mass * SurfaceCosine;
    }
    public void MovingNonMotorized()
    {

        DesiredVelocityMagnitude = MotorForce + Mathf.Sqrt((2 * F / rigidBody.mass) - (2 * GravityFixedEnergy * (transform.position.y - TriggerHeight)));
    }
    public void InLoop()
    {
        DesiredVelocityMagnitude = MotorForce + EnterLoopVelocity + (2 * GravityFixedEnergy * (TriggerHeight - transform.position.y) / MovingLoopRadius);
    }
    public void Idle()
    {
        DesiredVelocity = Vector3.zero;
    }
    #endregion



}

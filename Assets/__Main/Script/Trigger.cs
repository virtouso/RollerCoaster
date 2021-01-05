using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent MyAction;

    public float AdditiveMotorForce;
    public bool EnterLoop;

    public Transform RadiusReference;
    public float radius;

    #region Unity events

    private void Awake()
    {
        if (EnterLoop)
        {
            radius = Mathf.Abs(transform.position.y-RadiusReference.transform.position.y)/2;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "leadcabin")
        {
            MyAction.Invoke();
            Debug.Log("tran passed");
        }
    }

    #endregion




    #region Utility Region

    public void OnMotorize() { }
    public void MaxHill()
    {
        LeadingCabin._instance.TriggerHeight = LeadingCabin._instance.transform.position.y;
        LeadingCabin._instance.CurrentState.RemoveAllListeners();
        LeadingCabin._instance.CurrentState.AddListener(LeadingCabin._instance.MovingNonMotorized);

    }
    public void MinHill()
    {
        //LeadingCabin._instance.TriggerHeight = LeadingCabin._instance.transform.position.y;
        //LeadingCabin._instance.CurrentState.RemoveAllListeners();
        //LeadingCabin._instance.CurrentState.AddListener(LeadingCabin._instance.MovingNonMotorized);
    }
    public void LoopMin()
    {
        LeadingCabin._instance.TriggerHeight = LeadingCabin._instance.transform.position.y;
        LeadingCabin._instance.CurrentState.RemoveAllListeners();
        LeadingCabin._instance.CurrentState.AddListener(LeadingCabin._instance.InLoop);


        if (EnterLoop)
        {
            LeadingCabin._instance.EnterLoopVelocity = LeadingCabin._instance.DesiredVelocityMagnitude;
            LeadingCabin._instance.MovingLoopRadius = radius;
        }
    }
    public void LoopMax()
    {
        LeadingCabin._instance.TriggerHeight = LeadingCabin._instance.transform.position.y;
        LeadingCabin._instance.CurrentState.RemoveAllListeners();
        LeadingCabin._instance.CurrentState.AddListener(LeadingCabin._instance.InLoop);
        if (EnterLoop)
        {
            LeadingCabin._instance.EnterLoopVelocity = LeadingCabin._instance.DesiredVelocityMagnitude;
            LeadingCabin._instance.MovingLoopRadius = radius;
        }

    }

    public void GoIdle()
    {
        LeadingCabin._instance.DesiredVelocity = Vector3.zero;
        LeadingCabin._instance.CurrentState.RemoveAllListeners();
        LeadingCabin._instance.CurrentState.AddListener(LeadingCabin._instance.Idle);
    }
    #endregion








    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "");
    }


}

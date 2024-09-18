using UnityEngine;
using UnityEngine.AI;

public class ForceData
{
    public Rigidbody rb;
    public Vector3 forceDirection;
    public Vector3 hitPoint;
    public NavMeshAgent navMeshAgent;
    public EnemyAI enemy;
    public InteractionObject interactionObject;
    public ForceData(Rigidbody rb, InteractionObject interactionObject, Vector3 forceDirection, Vector3 hitPoint, NavMeshAgent navMeshAgent, EnemyAI enemy)
    {
        this.rb = rb;
        this.forceDirection = forceDirection;
        this.hitPoint = hitPoint;
        this.navMeshAgent = navMeshAgent;
        this.enemy = enemy;
        this.interactionObject = interactionObject;
    }
}
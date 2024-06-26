using System;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTarget : MonoBehaviour
{
    [SerializeField] protected Zombie parent;

    [SerializeField] protected Transform targeted;
    
    [SerializeField] protected float radius = 2;
    [SerializeField] protected float lookAngleLimit = 30f;
    [SerializeField] protected LayerMask layerMask;
    
    [SerializeField] protected Material targetingMaterial;
    [SerializeField] protected Material targetedMaterial;

    public Transform Targeted => targeted;

    public event Action TargetChange;
    
    protected void FixedUpdate()
    {
        this.FindTarget();
    }

    protected virtual void FindTarget()
    {
        Collider[] results = new Collider[28];
        var size = Physics.OverlapSphereNonAlloc(parent.transform.position, this.radius, results, this.layerMask);
        if (size <= 0) return;
        this.LookNearestTarget(this.ConvertColliderToTransform(results, size));
    }

    protected virtual List<Transform> ConvertColliderToTransform(Collider[] targets, int size)
    {
        List<Transform> targetTansforms = new List<Transform>();
        for (int i = 0; i < size; i++)
        {
            targetTansforms.Add(targets[i].transform);
        }
        return targetTansforms;
    }

    protected virtual void LookNearestTarget(List<Transform> targets)
    {
        Transform nearestTarget = this.GetNearestTarget(targets);
        if (ReferenceEquals(nearestTarget, this.targeted)) return;
        this.UpdateTargeted(nearestTarget);
    }

    protected virtual void UpdateTargeted(Transform newTarget)
    {
        if (this.targeted)
        {
            this.SetTargetMaterial(this.targeted);
        }

        this.targeted = newTarget;
        this.TargetChange?.Invoke();
        if (!newTarget) return;
        this.SetTargetedMaterial(newTarget);
    }

    protected virtual void SetTargetMaterial(Transform target)
    {
        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
        meshRenderer.material = this.targetedMaterial;
    }
    
    protected virtual void SetTargetedMaterial(Transform target)
    {
        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
        meshRenderer.material = this.targetingMaterial;
    }
    
    protected virtual Transform GetNearestTarget(List<Transform> targets)
    {
        Transform nearestTarget = null;
        float minDistance = float.MaxValue;
        
        foreach (Transform target in targets)
        {
            Vector3 targetVector = target.position - this.parent.transform.position;
            
            float angleWithZombie = Vector3.Angle(targetVector, this.parent.transform.forward);
            bool targetInFront = Vector3.Dot(targetVector, this.parent.transform.forward) > 0f;
            if (angleWithZombie > this.lookAngleLimit && !targetInFront) continue;
            
            float targetDistance = targetVector.magnitude;
            if (targetDistance >= minDistance) continue;
            minDistance = targetDistance;
            nearestTarget = target;
        }

        return nearestTarget;
    }

    private void OnDrawGizmos()
    {
        if (!parent) return;
        Gizmos.DrawWireSphere(parent.transform.position, radius);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ZombieMovement : MonoBehaviour
{
    [SerializeField] protected Zombie parent;
    
    [FormerlySerializedAs("movement")]
    [Header("Movement Calculate")]
    [SerializeField] protected bool isMoving = true;
    [SerializeField] protected float speed = 1f;
    [SerializeField] protected float progress;
    [SerializeField] protected List<Transform> paths;
    [SerializeField] protected Vector3[] pathPositions;

    [Header("Movement Direction")]
    [SerializeField] protected Vector3 oldPosition;
    [SerializeField] protected Vector3 direction;

    public bool IsMoving => isMoving;
    public float Progress => progress;
    public Vector3 Direction => direction;

    protected void Start()
    {
        if (this.paths is not null && this.paths.Count > 0)
        {
            this.pathPositions = this.paths.Select(p => p.position).ToArray();
            this.oldPosition = this.parent.transform.position;
            return;
        }
        this.isMoving = false;
        Debug.LogWarning("Zombie movement path missing!");
    }

    protected void Update()
    {
        if (!this.isMoving) return;
        this.UpdateMovingProgress();
        this.Moving();
        this.UpdateDirection();
    }

    protected virtual void UpdateMovingProgress()
    {
        float newProgress = this.speed / 10f * Time.deltaTime;
        this.progress = Mathf.Clamp01(this.progress + newProgress);
    }

    protected virtual void Moving()
    {
        Vector3 newPos = this.GetNewPosition(pathPositions);
        this.parent.transform.position = newPos;
        if (Math.Abs(this.progress - 1f) > 0.001f) return;
        this.isMoving = false;
    }

    protected virtual Vector3 GetNewPosition(params Vector3[] pathIndexes)
    {
        if (pathIndexes.Length == 2) return this.BezierCurve(pathIndexes[0], pathIndexes[1]);
        
        Vector3 startPathPosition = this.GetNewPosition(this.GetStartPath(pathIndexes));
        Vector3 endPathPosition = this.GetNewPosition(this.GetEndPath(pathIndexes));
        return this.GetNewPosition(startPathPosition, endPathPosition);
    }

    protected virtual Vector3[] GetStartPath(Vector3[] origin)
    {
        Vector3[] startPath = new Vector3[origin.Length];
        Array.Copy(origin, startPath, origin.Length);
        return startPath[..^1];
    }
    
    protected virtual Vector3[] GetEndPath(Vector3[] origin)
    {
        Vector3[] endParth = new Vector3[origin.Length];
        Array.Copy(origin, endParth, origin.Length);
        return endParth[1..];
    }

    protected virtual Vector3 BezierCurve(Vector3 start, Vector3 end)
    {
        return Vector3.Lerp(start, end, progress);
    }

    protected virtual void UpdateDirection()
    {
        Vector3 curPos = this.parent.transform.position;
        this.direction = (curPos - this.oldPosition).normalized;
        this.oldPosition = curPos;
    }
}
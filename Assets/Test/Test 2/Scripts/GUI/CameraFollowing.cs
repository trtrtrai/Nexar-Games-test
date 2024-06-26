using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] protected Transform target;
    [SerializeField] protected Vector3 offset;
    [SerializeField] protected float speed = 2f;

    protected void Update()
    {
        if (!target) return;
        this.FollowTarget();
    }
    
    protected virtual void FollowTarget()
    {
        Vector3 newPos = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);
        transform.position = newPos;
    }
}
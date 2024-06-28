using UnityEngine;

public class ZombieHeadCopyRotation : MonoBehaviour
{
    [SerializeField] protected Transform target;

    protected void LateUpdate()
    {
        transform.rotation = this.target.rotation;
    }
}
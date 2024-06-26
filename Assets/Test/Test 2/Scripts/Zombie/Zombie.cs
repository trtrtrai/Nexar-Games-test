using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] protected ZombieMovement movement;
    [SerializeField] protected ZombieAnimationControl animationControl;
    [SerializeField] protected ZombieTarget zombieTarget;

    public ZombieMovement Movement => movement;
    public ZombieAnimationControl AnimationControl => animationControl;
    public ZombieTarget ZombieTarget => zombieTarget;

    private void Start()
    {
        this.movement = transform.GetComponentInChildren<ZombieMovement>();
        this.animationControl = transform.GetComponentInChildren<ZombieAnimationControl>();
        this.zombieTarget = transform.GetComponentInChildren<ZombieTarget>();

        if (this.movement && this.animationControl && this.zombieTarget) return;
        Debug.LogWarning("Zombie required module lost!");
        gameObject.SetActive(false);
    }
}
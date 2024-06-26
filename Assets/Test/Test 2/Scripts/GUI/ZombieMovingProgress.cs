using UnityEngine;
using UnityEngine.UI;

public class ZombieMovingProgress : MonoBehaviour
{
    [SerializeField] protected Zombie zombie;
    [SerializeField] protected Image progressImg;

    protected void Start()
    {
        if (zombie && progressImg) return;
        Debug.LogWarning("Progress image is missing some thing!");
        this.enabled = false;
    }

    protected void Update()
    {
        this.UpdateProgress();
    }

    protected virtual void UpdateProgress()
    {
        progressImg.fillAmount = zombie.Movement.Progress;
    }
}
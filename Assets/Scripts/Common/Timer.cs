using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private float time;
    private float value;

    public event UnityAction Finished;
    public float Value => value;

    private void Start() => value = time;
    private void Update()
    {
        value -= Time.deltaTime;

        if (value <= 0)
        {
            enabled = false;

            Finished?.Invoke();
        }
    }
}

using UnityEngine;

public class UIPausePanel : MonoBehaviour, IDependency<Pauser>
{
    [SerializeField] private GameObject pausePanel;
    private Pauser pauser;

    public void Construct(Pauser obj) => pauser = obj;

    private void Start()
    {
        pauser.PauseStateChange += OnPauseStateChange;
        pausePanel.SetActive(false);
    }
    private void OnDestroy()
    {
        pauser.PauseStateChange -= OnPauseStateChange;
    }
    private void OnPauseStateChange(bool isPause)
    {
        pausePanel.SetActive(isPause);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauser.ChangePauseState();
        }
    }
    public void UnPause()
    {
        pauser.UnPause();
    }
}
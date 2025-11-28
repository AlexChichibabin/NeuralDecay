using UnityEngine;

[RequireComponent(typeof(SoundHook))]
public class UIPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;
    private SoundHook soundHook;

    private void Awake() => soundHook = GetComponent<SoundHook>();
    private void Start()
    {
        foreach (GameObject panel in panels) panel.SetActive(false);
    }
    public void OpenPanel(GameObject panelForOpen)
    {
        foreach (GameObject panel in panels)
        {
            if (panel == panelForOpen)
            {
                panel.SetActive(true);
                soundHook.m_Sound = Sound.OnPanelOpened;
                soundHook.Play();
                continue;
            }
            panel.SetActive(false);
        }
    }
}

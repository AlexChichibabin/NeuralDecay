using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SoundHook))]
public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] protected bool Interactable;

    private bool focused;
    protected SoundHook soundHook;
    public bool Focused => focused;

    public UnityEvent OnClick;

    public event UnityAction<UIButton> PointerEnter;
    public event UnityAction<UIButton> PointerExit;
    public event UnityAction<UIButton> PointerClick;

    protected virtual void Awake()
    {
        soundHook = GetComponent<SoundHook>();
    }
    public virtual void SetFocus()
    {
        if (Interactable == false) return;
        focused = true;
    }
    public virtual void SetUnfocus()
    {
        if (Interactable == false) return;
        focused = false;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (Interactable == false) return;

        PointerEnter?.Invoke(this);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (Interactable == false) return;

        PointerExit?.Invoke(this);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (Interactable == false) return;

        soundHook.m_Sound = Sound.OnButtonClicked;
        soundHook.Play();

        PointerClick?.Invoke(this);
        OnClick?.Invoke();
    }
}
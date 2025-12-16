using System;
using ShabuStudio.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayButtonController : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Animator animator;
    public float waitTime = 3f;
    private float timer;
    private bool isHover;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isHover && timer < waitTime)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                animator.SetTrigger("Loop");
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetTrigger("Hover");
        timer = 0;
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetTrigger("Normal");
        isHover = false;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        BattleStateManager.Instance.StartActionSequence();
    }
}

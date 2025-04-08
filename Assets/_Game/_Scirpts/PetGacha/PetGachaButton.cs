using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetGachaButton : MonoBehaviour
{
    public Button showButton;
    public GameObject hiddenPanel;
    private Animator animator;
    private bool isShowing = false;
    void Start()
    {
        animator = hiddenPanel.GetComponent<Animator>();
        hiddenPanel.SetActive(false);
        showButton.onClick.AddListener(PlayShowAnimation);
    }

    void PlayShowAnimation()
    {
        if (hiddenPanel != null && animator != null && !isShowing)
        {
            hiddenPanel.SetActive(true);
            animator.Play("In", 0, 0);
            isShowing = true;
        }
    }
}

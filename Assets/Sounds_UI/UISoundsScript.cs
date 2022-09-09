using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundsScript : MonoBehaviour
{
    public AudioClip ButtonPushed;
    public AudioClip BackButtonPushed;
    public AudioClip MenuButtonPushed;
    public AudioClip CloseMenuButtonPushed;

    private AudioSource audioSource;
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WhenBackButtonPushed()
    {
        audioSource.PlayOneShot(BackButtonPushed);
    }

    public void WhenButtonPushed()
    {
        audioSource.PlayOneShot(ButtonPushed);
    }

    public void WhenMenuButtonPushed()
    {
        audioSource.PlayOneShot(MenuButtonPushed);
    }

    public void WhenCloseMenuButtonPushed()
    {
        audioSource.PlayOneShot(CloseMenuButtonPushed);
    }
}

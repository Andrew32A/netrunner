using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class DisplayText : MonoBehaviour
{
    [Header("Text References")]
    public string tutorialTextName = "TutorialText";
    public string bigTextName = "BigText";
    private TextMeshProUGUI tutorialText;
    private TextMeshProUGUI bigText;

    [Header("Audio")]
    public AudioClip clickSound;
    public AudioMixerGroup outputMixer;

    [Header("Button Dismiss")]
    public List<string> dismissButtons = new List<string>();

    [Header("Big Text Delay")]
    public float bigTextDelay = 1.5f;

    private AudioSource audioSource;
    private bool canDismissTutorial = false;

    private void Start()
    {
        // find TextMeshPro objects by their names
        tutorialText = GameObject.Find(tutorialTextName)?.GetComponent<TextMeshProUGUI>();
        bigText = GameObject.Find(bigTextName)?.GetComponent<TextMeshProUGUI>();

        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = outputMixer;
            audioSource.playOnAwake = false;
            audioSource.clip = clickSound;
        }
    }

    private void Update()
    {
        if (canDismissTutorial)
        {
            foreach (string buttonName in dismissButtons)
            {
                if (Input.GetButtonDown(buttonName))
                {
                    HideTutorialTextAfterDelay(2.0f);
                    canDismissTutorial = false;
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowTutorialText("Insert tutorial message here");
        }
    }

    public void ShowTutorialText(string message, float duration = 3.0f)
    {
        if (tutorialText)
        {
            tutorialText.text = message;
            tutorialText.gameObject.SetActive(true);
            canDismissTutorial = true;
            Invoke(nameof(HideTutorialText), duration);
        }
    }

    public void ShowBigText(string message)
    {
        if (bigText)
        {
            bigText.text = message;
            bigText.gameObject.SetActive(true);
            audioSource.Play();
            Invoke(nameof(HideBigText), bigTextDelay);
        }
    }

    private void HideTutorialText()
    {
        if (tutorialText) tutorialText.gameObject.SetActive(false);
    }

    private void HideBigText()
    {
        if (bigText) bigText.gameObject.SetActive(false);
    }

    private void HideTutorialTextAfterDelay(float delay)
    {
        Invoke(nameof(HideTutorialText), delay);
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class DisplayTextTrigger : MonoBehaviour
{
    [Header("Text")]
    public string displayTutorialText = "testing 1 2 3";
    public string displayBigText = "testing big";

    [Header("Canvas References")]
    public string canvasName = "YourCanvasName"; // may need to change this to gameobject reference
    public string tutorialTextName = "TutorialText";
    public string bigTextName = "BigText";
    private TextMeshProUGUI tutorialText;
    private TextMeshProUGUI bigText;

    [Header("Audio")]
    public AudioClip clickSound;
    public AudioMixerGroup outputMixer;

    [Header("Button Dismiss")]
    public List<KeyCode> dismissKeys = new List<KeyCode>();

    [Header("Text Delay")]
    public float bigTextDelay = 1.5f;
    public float smallTextDelay = 1.0f;

    private AudioSource audioSource;
    private bool canDismissTutorial = false;

    private void Start()
    {
        // find TextMeshPro objects from the specified canvas
        var canvas = GameObject.Find(canvasName);
        tutorialText = canvas?.transform.Find(tutorialTextName)?.GetComponent<TextMeshProUGUI>();
        bigText = canvas?.transform.Find(bigTextName)?.GetComponent<TextMeshProUGUI>();

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
            foreach (KeyCode key in dismissKeys)
            {
                if (Input.GetKeyDown(key))
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
            ShowTutorialText(displayTutorialText);
            Invoke("ShowTriggeredBigText", smallTextDelay);
        }
    }

    public void ShowTutorialText(string message)
    {
        if (tutorialText)
        {
            tutorialText.text = message;
            tutorialText.gameObject.SetActive(true);
            canDismissTutorial = true;
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

    private void ShowTriggeredBigText()
    {
        ShowBigText(displayBigText);
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

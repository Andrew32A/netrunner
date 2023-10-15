using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class DisplayTextTrigger : MonoBehaviour
{
    [Header("Text")]
    public string displayTutorialText = "Enter tutorial text here";
    public List<string> displayBigTexts = new List<string> { "Enter big text here" };

    [Header("Canvas References")]
    public GameObject canvas;
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
    public float bigTextDisplayTime; // default: 0.4f - time before the big text is hidden
    public float bigTextDelay; // default: 0.2f - delay between each of the big text messages
    public float smallTextDismissDelay; // default: 1f - time before the small text is hidden after dismissing

    private AudioSource audioSource;
    private bool canDismissTutorial = false;
    private int currentBigTextIndex = 0;

    private void Start()
    {
        // get TextMeshPro objects from canvas
        tutorialText = canvas.transform.Find(tutorialTextName)?.GetComponent<TextMeshProUGUI>();
        bigText = canvas.transform.Find(bigTextName)?.GetComponent<TextMeshProUGUI>();

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
                    HideTutorialTextAfterDelay(smallTextDismissDelay);
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
            if (displayBigTexts.Count > 0)
                ShowNextBigText();

            // disable collider so this trigger doesn't get triggered again
            GetComponent<Collider>().enabled = false;

            // after 30 seconds destroy object after player triggers it
            Destroy(gameObject, 30.0f);
        }
    }

    public void DisplayTutorialText(string message)
    {
        if (tutorialText)
        {
            tutorialText.text = message;
            tutorialText.gameObject.SetActive(true);
            canDismissTutorial = true;
        }
    }

    public void ShowTutorialText()
    {
        DisplayTutorialText(displayTutorialText);
    }

    public void ShowNextBigText()
    {
        if (currentBigTextIndex < displayBigTexts.Count)
        {
            ShowBigText(displayBigTexts[currentBigTextIndex]);
            currentBigTextIndex++;
        }
        {
            // display tutorial text 2 seconds after all big texts have been shown
            Invoke(nameof(ShowTutorialText), 2.0f);
        }
    }

    public void ShowBigText(string message)
    {
        if (bigText)
        {
            bigText.text = message;
            bigText.gameObject.SetActive(true);
            audioSource.Play();
            Invoke(nameof(HideBigTextAndQueueNext), bigTextDisplayTime);
        }
    }

    private void HideBigTextAndQueueNext()
    {
        HideBigText();
        Invoke(nameof(ShowNextBigText), bigTextDelay);
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

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class TitleManager : MonoBehaviour {
    
    [Header("Logo Components")]
    public Image logoImage;
    public Image background;

    public Image button1;
    public Image button2;

    public TextMeshProUGUI button1Text;
    public TextMeshProUGUI button2Text;

    [Header("Slides")]
    public Sprite[] slides;

    public GameObject slideImages;

    [SerializeField] private int pageNumber;


    void Start() {
        logoImage.color = new Color(0, 0, 0, 1);
        background.color = new Color(0, 0, 0, 1);
        button1.color = new Color(0, 0, 0, 1);
        button2.color = new Color(0, 0, 0, 1);

        button1Text.enabled = false;
        button2Text.enabled = false;

        slideImages.SetActive(false);

        StartCoroutine(FadeLogos());
    }

    void Update() {
        if (pageNumber >= 0 && pageNumber < slides.Length && slides[pageNumber] != null)
            slideImages.GetComponent<Image>().sprite = slides[pageNumber];
    }

    IEnumerator FadeLogos() {
        yield return StartCoroutine(Fade(background, 0, 155, 2f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(Fade(logoImage, 0, 255, 2f));

        StartCoroutine(Fade(button1, 0, 170, 1f));
        yield return StartCoroutine(Fade(button2, 0, 170, 1f));

        yield return new WaitForSeconds(0.5f);

        button1Text.enabled = true;
        button2Text.enabled = true;
    }

    IEnumerator Fade(Image image, float start, float end, float duration) {
        float t = 0f;

        start /= 255f;
        end   /= 255f;

        while (t < duration) {
            float a = Mathf.Lerp(start, end, t / duration);
            image.color = new Color(a, a, a, 1);
            t += Time.deltaTime;
            yield return null;
        }

        image.color = new Color(end, end, end, 1);
    }

    public void StartGame() {
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine() {
        button1.color = new Color(220, 220, 220, 1);

        yield return new WaitForSeconds(0.2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Cave");
    }

    public void Tutorial() {
        pageNumber = 0;
        slideImages.SetActive(true);
    }

    public void ChangeSlide(int val) {
        pageNumber += val;
        pageNumber = Mathf.Clamp(pageNumber, 0, slides.Length);

        if (pageNumber == slides.Length) {
            slideImages.SetActive(false);
            return;
        }
    }
}

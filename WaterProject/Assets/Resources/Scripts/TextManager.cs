using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TextManager : MonoBehaviour
{
    TextMeshProUGUI chapter1Title;
    TextMeshProUGUI chapter2Title;
    TextMeshProUGUI chapter3Title;
    TextMeshProUGUI chapter1Summary;
    TextMeshProUGUI chapter2Summary;
    TextMeshProUGUI chapter3Summary;

    Image chapter1Sprite;
    Image chapter2Sprite;
    Image chapter3Sprite;

    GameObject moreChapters;
    GameObject prevChapters;
    GameObject eventManager;
    
    //index variables are used to keep track of where the user is in chapter select
    int lowerIndex = 0;
    int upperIndex = 2;

    int sceneToLoad;

    [SerializeField]
    public string[] chapterTitles;

    [SerializeField]
    public string[] chapterSummaries;

    [SerializeField]
    public Sprite[] chapterSprites;

    //array to hold scene build order numbers, fill in order chapters 1-6
    [SerializeField]
    public int[] chapterSceneNums;

    /*
     * MAKE SURE THAT IN THE INSPECTOR YOU SET THE SIZE OF THE TITLE, SUMMARY, SPRITE, AND SCENENUM ARRAYS TO BE THE EXACT SAME AND GREATER THAN 0
     */


    //Init global variables from objects in scene
    public void Awake() {
        chapter1Title = GameObject.Find("Chapter1/ChapterTitle").GetComponent<TextMeshProUGUI>();
        chapter2Title = GameObject.Find("Chapter2/ChapterTitle").GetComponent<TextMeshProUGUI>();
        chapter3Title = GameObject.Find("Chapter3/ChapterTitle").GetComponent<TextMeshProUGUI>();
        chapter1Summary = GameObject.Find("Chapter1/ChapterSummary").GetComponent<TextMeshProUGUI>();
        chapter2Summary = GameObject.Find("Chapter2/ChapterSummary").GetComponent<TextMeshProUGUI>();
        chapter3Summary = GameObject.Find("Chapter3/ChapterSummary").GetComponent<TextMeshProUGUI>();
        moreChapters = GameObject.Find("MoreChapters");
        prevChapters = GameObject.Find("BackChapters");
        eventManager = GameObject.Find("EventSystem");
        chapter1Sprite = GameObject.Find("Chapter1/ChapterIcon").GetComponent<Image>();
        chapter2Sprite = GameObject.Find("Chapter2/ChapterIcon").GetComponent<Image>();
        chapter3Sprite = GameObject.Find("Chapter3/ChapterIcon").GetComponent<Image>();
    }

    //called whenever the UI needs to update
    public void UpdateText() {
        //pushes the array variables to the eventManager to be kept track of for the current status of the screen
        eventManager.GetComponent<TextManager>().chapterTitles = chapterTitles;
        eventManager.GetComponent<TextManager>().chapterSummaries = chapterSummaries;
        eventManager.GetComponent<TextManager>().chapterSprites = chapterSprites;
        eventManager.GetComponent<TextManager>().chapterSceneNums = chapterSceneNums;
        //updates the index variables of the pressed button from the values that the evenManager has
        lowerIndex = eventManager.GetComponent<TextManager>().lowerIndex;
        upperIndex = eventManager.GetComponent<TextManager>().upperIndex;

        //determines if the back/next chapters buttons are interactable
        if (lowerIndex == 0 && chapterTitles.Length > 3) {
            //if on first page and the amount of chapters is greater than 3
            moreChapters.GetComponent<Button>().interactable = true;
            prevChapters.GetComponent<Button>().interactable = false;
        } else if(lowerIndex != 0) {
            //if on second page
            moreChapters.GetComponent<Button>().interactable = false;
            prevChapters.GetComponent<Button>().interactable = true;
        }
        //fades out any UI elements to be updated, updates the UI, fades back in
        StartCoroutine(Fade(new Color(1,1,1,1),0.5f));
    }

    //called whenever you switch modules or go back/next through chapters
    public void UpdateIndexBounds(int buttonPressed) {
        //if going to next chapters
        if (buttonPressed == 1) {
            lowerIndex = 3;
            for(int i = 3; i != 0; i--) {
                if(upperIndex + i > chapterTitles.Length) {
                    upperIndex += i;
                    break;
                }
            }
            //if going back chapters
        } else {
            lowerIndex = 0;
            upperIndex = 2;
        }
    }

    //called when you select a chapter
    public void LoadScene(int buttonPressed) {
        //button pressed is equal to 0,1,2 for Chapter1,Chapter2,Chapter3 buttons respectively
        switch (buttonPressed) {
            //if statements to check if user is on first or second page of chapters
            case 0:
            if(upperIndex != 2) {
                //load scene from given index in array
                SceneManager.LoadScene(eventManager.GetComponent<TextManager>().chapterSceneNums[3]);
            } else {
                SceneManager.LoadScene(eventManager.GetComponent<TextManager>().chapterSceneNums[0]);
            }
            break;
            case 1:
            if (upperIndex != 2) {
                SceneManager.LoadScene(eventManager.GetComponent<TextManager>().chapterSceneNums[4]);
            } else {
                SceneManager.LoadScene(eventManager.GetComponent<TextManager>().chapterSceneNums[1]);
            }
            break;
            case 2:
            if (upperIndex != 2) {
                SceneManager.LoadScene(eventManager.GetComponent<TextManager>().chapterSceneNums[5]);
            } else {
                SceneManager.LoadScene(eventManager.GetComponent<TextManager>().chapterSceneNums[2]);
            }
            break;
        }
    }

    //fades UI out, changes an elements that need to be changed, fades UI in
    IEnumerator Fade(Color color, float time) {
        //elapsedTime needed to keep track of time from when the function is called to the target fade time
        float elapsedTime = 0;

        //fade UI out
        while(elapsedTime < time) {
            //fades the alpha channel from the color value given down to 0 over the given float time
            chapter1Title.color = new Color(color.r, color.g, color.b,Mathf.Lerp(color.a,0,(elapsedTime / time)));
            chapter2Title.color = new Color(color.r,color.g,color.b,Mathf.Lerp(color.a,0,(elapsedTime / time)));
            chapter3Title.color = new Color(color.r,color.g,color.b,Mathf.Lerp(color.a,0,(elapsedTime / time)));
            chapter1Summary.color = new Color(color.r,color.g,color.b,Mathf.Lerp(color.a,0,(elapsedTime / time)));
            chapter2Summary.color = new Color(color.r,color.g,color.b,Mathf.Lerp(color.a,0,(elapsedTime / time)));
            chapter3Summary.color = new Color(color.r,color.g,color.b,Mathf.Lerp(color.a,0,(elapsedTime / time)));
            //0.31764f is hardcoded to keep track of sprit color
            chapter1Sprite.color = new Color(0.3176471f,0.3176471f,0.3176471f,Mathf.Lerp(color.a,0,(elapsedTime / time)));
            chapter2Sprite.color = new Color(0.3176471f,0.3176471f,0.3176471f,Mathf.Lerp(color.a,0,(elapsedTime / time)));
            chapter3Sprite.color = new Color(0.3176471f,0.3176471f,0.3176471f,Mathf.Lerp(color.a,0,(elapsedTime / time)));

            //update elapsed time
            elapsedTime += Time.deltaTime;
            yield return null;  
        }
        //reset elapsedTime so that the UI can fade back in
        elapsedTime = 0;

        //set color to be the same as given, but fully transparent
        color = new Color(color.r,color.g,color.b,0);

        //lists to be populated with the text and image elements that need to be faded back in
        List<TextMeshProUGUI> tTFI = new List<TextMeshProUGUI>();
        List<Image> iTFI = new List<Image>();

        //keep track if try/catch fails
        bool trySuccess = false;

        switch (upperIndex != 2) {
            //case true if on second page of chapters
            case true:
            //assuming that if user is on second page of chapters index 3 will always be in bounds
            //update top title text
            chapter1Title.text = chapterTitles[3];
            //add top title text element to list of text elements to be faded in
            tTFI.Add(chapter1Title);
            //not always going to be more than one chapter on a page
            //if chapterTitles[4] is out of bounds catch and make the button not interactable
            try { chapter2Title.text = chapterTitles[4]; trySuccess = true; } catch { GameObject.Find("Chapter2").GetComponent<Button>().interactable = false; }
            if (trySuccess) {
                //if chapterTitles[4] is in bounds add text element to list to be faded in
                tTFI.Add(chapter2Title);
                //reset trySuccess
                trySuccess = !trySuccess;
                //set button to be interactable
                GameObject.Find("Chapter2").GetComponent<Button>().interactable = true;
            }
            try { chapter3Title.text = chapterTitles[5]; trySuccess = true; } catch { GameObject.Find("Chapter3").GetComponent<Button>().interactable = false; }
            if (trySuccess) {
                tTFI.Add(chapter3Title);
                trySuccess = !trySuccess;
                GameObject.Find("Chapter3").GetComponent<Button>().interactable = true;
            }
            chapter1Summary.text = chapterSummaries[3];
            tTFI.Add(chapter1Summary);
            try { chapter2Summary.text = chapterSummaries[4]; trySuccess = true; } catch { }
            if (trySuccess) {
                tTFI.Add(chapter2Summary);
                trySuccess = !trySuccess;
            }
            try { chapter3Summary.text = chapterSummaries[5]; trySuccess = true; } catch { }
            if (trySuccess) {
                tTFI.Add(chapter3Summary);
                trySuccess = !trySuccess;
            }
            chapter1Sprite.sprite = chapterSprites[3];
            iTFI.Add(chapter1Sprite);
            try { chapter2Sprite.sprite = chapterSprites[4]; trySuccess = true; } catch { }
            if (trySuccess) {
                iTFI.Add(chapter2Sprite);
                trySuccess = !trySuccess;
            }
            try { chapter3Sprite.sprite = chapterSprites[5]; trySuccess = true; } catch { }
            if (trySuccess) {
                iTFI.Add(chapter3Sprite);
                trySuccess = !trySuccess;
            }
            break;
            //case false if on first page of chapters
            case false:
            chapter1Title.text = chapterTitles[0];
            tTFI.Add(chapter1Title);
            try { chapter2Title.text = chapterTitles[1]; trySuccess = true; } catch { GameObject.Find("Chapter2").GetComponent<Button>().interactable = false; }
            if (trySuccess) {
                tTFI.Add(chapter2Title);
                trySuccess = !trySuccess;
                GameObject.Find("Chapter2").GetComponent<Button>().interactable = true;
            }
            try { chapter3Title.text = chapterTitles[2]; trySuccess = true; } catch { GameObject.Find("Chapter3").GetComponent<Button>().interactable = false; }
            if (trySuccess) {
                tTFI.Add(chapter3Title);
                trySuccess = !trySuccess;
                GameObject.Find("Chapter3").GetComponent<Button>().interactable = true;
            }
            chapter1Summary.text = chapterSummaries[0];
            tTFI.Add(chapter1Summary);
            try { chapter2Summary.text = chapterSummaries[1]; trySuccess = true; } catch { }
            if (trySuccess) {
                tTFI.Add(chapter2Summary);
                trySuccess = !trySuccess;
            }
            try { chapter3Summary.text = chapterSummaries[2]; trySuccess = true; } catch { }
            if (trySuccess) {
                tTFI.Add(chapter3Summary);
                trySuccess = !trySuccess;
            }
            chapter1Sprite.sprite = chapterSprites[0];
            iTFI.Add(chapter1Sprite);
            try { chapter2Sprite.sprite = chapterSprites[1]; trySuccess = true; } catch { }
            if (trySuccess) {
                iTFI.Add(chapter2Sprite);
                trySuccess = !trySuccess;
            }
            try { chapter3Sprite.sprite = chapterSprites[2]; trySuccess = true; } catch { }
            if (trySuccess) {
                iTFI.Add(chapter3Sprite);
                trySuccess = !trySuccess;
            }
            break;
        }

        //fade UI back in
        while (elapsedTime < time) {
            //fade each text element from list back in from the given color value to an alpha of 1
            foreach(TextMeshProUGUI elem in tTFI) {
                elem.color = new Color(color.r,color.g,color.b,Mathf.Lerp(color.a,1,(elapsedTime / time)));
            }
            //fade each sprite element from list back in from the hard coded value to an alpha of 1
            foreach (Image elem in iTFI) {
                elem.color = new Color(0.3176471f,0.3176471f,0.3176471f,Mathf.Lerp(color.a,1,(elapsedTime / time)));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

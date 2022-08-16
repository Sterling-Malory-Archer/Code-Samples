using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class JSONController : MonoBehaviour
{

    public string jsonURLTutorials;
    public string jsonURLQuestions;
    public string jsonURLRewards;
    public Text tutorialTxt;
    static JSONDataClass tutTXT;
    static JSONDataClassQuestions questionTxt;
    static JSONDataRewards rewardTxt;
    public GameObject continueButton;
    public GameObject quitButton;
    public GameObject questionCanvas;
    public GameObject cluePopup;
    public Text clueText;

    public Text questionText;
    [SerializeField]
    public List<Text> answerText;

    [SerializeField]
    public List<GameObject> answerID;

    [SerializeField]
    private int answerCount;

    //TutorialImage
    [SerializeField]
    private RawImage tutorialImage;

    // Rewards GameObjects and UI
    public Text rewardsIDText;
    public RawImage rewardsImage;
    private string storageUrl = "https://animalquest.ix42.com/storage/";
    public InputField emailInput;
    public Text placeholderMailText;


    public GameObject tutorialCanvas;
    public GameObject languageCanvas;

    private int j;
    public bool isFrench;
    public bool isEnglish;

    // Navigation Bar Language
    public GameObject navBarEN;
    public GameObject navBarFR;


    // NewUI Settings
    public Color correctAnswerColor;
    public Color wrongAnswerColor;
    public GameObject correctAnswerPanel;
    public GameObject wrongAnswerPanel;

    public GameObject testImage;
    public List<GameObject> textAnswerGO;

    // Sound + Picture Answers
    public List<GameObject> pictureAnswerGO;
    public List<Text> pictureAnswerText;
    public List<GameObject> pictureAnswerID;

    public GameObject pictureQuestionAudio;
    public AudioSource pictureQuestionAudioClip;
    public List<RawImage> animalPictureAnswers;

    public RawImage pictureForImageQuestion;

    public List<Sprite> clueAnimalMarkerSprite;
    public Image clueImage;

    public Animator tutorialSlideBackground;
    public Animator quizTitleAnimation;

    public string animalToDetect;
    public string animalToDetectFR;

    public GameObject englishButton;
    public GameObject frenchButton;
    public GameObject startButton;

    [SerializeField]
    int[] questionsAnsweredId;
    [SerializeField]
    private int q = 0;

    public string filterString;

    public string animalLocationEn;
    public string animalLocationFr;

    public int currentQuestionID;

    public int wildTourQuestionID;

    public int[] wildTourQuestions;

    public Animator titleAnimator;

    Coroutine coTest;

    public Image humanImage;
    public Sprite[] humanSprites;

    public int humanSpriteCounter;
    public bool wentBackForClue;

    private void Awake()
    {
        titleAnimator.Play("TitleAnimation");
        LanguageChosenFrench();
        //////Checking if user has went through language selection and tutorials
        if (PlayerPrefs.HasKey("LanguageEN") || PlayerPrefs.HasKey("LanguageFR"))
        {
            englishButton.SetActive(false);
            frenchButton.SetActive(false);
            startButton.SetActive(true);
            //startButton.GetComponent<Animator>().enabled = true;
        }
        //// If PlayerPrefs has the key FirstPass, the user won't see tutorial canvas
        //if (PlayerPrefs.HasKey("FirstPass"))
        //{
        //    tutorialCanvas.SetActive(false);
        //}
        // Setting the Language boolean to match the chosen language in case of reopening the app
        if (PlayerPrefs.HasKey("IsFrench"))
        {
            isFrench = Convert.ToBoolean(PlayerPrefs.GetString("IsFrench"));
            isEnglish = false;
            PlayerPrefs.DeleteKey("IsEnglish");
            LanguageChosenFrench();
        }
        if (PlayerPrefs.HasKey("IsEnglish"))
        {
            isEnglish = Convert.ToBoolean(PlayerPrefs.GetString("IsEnglish"));
            isFrench = false;
            PlayerPrefs.DeleteKey("IsFrench");
            LanguageChosenEnglish();
        }
        if (PlayerPrefs.HasKey("QuestionsAnswered"))
        {
            foreach (var number in PlayerPrefsX.GetIntArray("QuestionsAnswered"))
            {
                if (number != 0)
                {
                    questionsAnsweredId.SetValue(number, q++);
                }
            }
            Debug.Log("HasKey");
        }

        if (PlayerPrefs.HasKey("CurrentQuestion"))
        {
            wildTourQuestionID = PlayerPrefs.GetInt("CurrentQuestion");
            humanSpriteCounter = wildTourQuestionID;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        emailInput.textComponent.text = PlayerPrefs.GetString("Email", "");
        if (PlayerPrefs.HasKey("Email"))
        {
            emailInput.textComponent.text = PlayerPrefs.GetString("Email");
        }
    }

    private void Update()
    {

        humanImage.sprite = humanSprites[humanSpriteCounter];
    }

    #region QuestionGetter
    IEnumerator GetDataQuestions()
    {

        UnityWebRequest _www = UnityWebRequest.Get(jsonURLQuestions + wildTourQuestions[wildTourQuestionID]);
        //_www.SetRequestHeader("isNewWildTour", "true");
        //Debug.Log(jsonURLQuestions + wildTourQuestions[wildTourQuestionID]);
        //_www.SetRequestHeader("hasAnimation", "true");
        //_www.SetRequestHeader("is_wild_tour", "true");
        //_www.SetRequestHeader("isSerbian", "true");
        yield return _www.SendWebRequest();

        if (_www.isNetworkError || _www.isHttpError)
        {
            Debug.Log(_www.error);
        }
        else
        {
            questionTxt = JsonUtility.FromJson<JSONDataClassQuestions>(_www.downloadHandler.text);
            if (isEnglish == true)
            {
                questionText.text = questionTxt.question.body.en;
                clueText.text = questionTxt.question.new_wild_tour_hint_en;
                animalLocationEn = questionTxt.question.wild_tour_location_en;
                SwapClueMarker();
            }
            else if (isFrench == true)
            {
                questionText.text = questionTxt.question.body.fr;
                clueText.text = questionTxt.question.clue;
                animalToDetect = questionTxt.question.animal_marker;
                animalToDetectFR = questionTxt.question.animal_marker_fr;
                animalLocationFr = questionTxt.question.wild_tour_location_fr;
                SwapClueMarker();
            }

            answerCount = questionTxt.answers.Count;
            currentQuestionID = questionTxt.question.id;
            PlayerPrefs.SetInt("LastQuestion", currentQuestionID);
            PlayerPrefs.Save();
            // If the question is text
            if (questionTxt.question.question_type_id == 1)
            {
                foreach (GameObject textAnswer in textAnswerGO)
                {
                    textAnswer.SetActive(true);
                }

                foreach (GameObject pictureTextAnswer in pictureAnswerGO)
                {
                    pictureTextAnswer.SetActive(false);
                }

                testImage.SetActive(false);
                pictureQuestionAudio.SetActive(false);

                for (j = 0; j < answerText.Count; j++)
                {
                    if (isEnglish == true)
                    {

                        answerText[j].text = questionTxt.answers[j].body.en;
                        answerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                    if (isFrench == true)
                    {

                        answerText[j].text = questionTxt.answers[j].body.fr;
                        answerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                }
            }

            // If the question is with sound
            if (questionTxt.question.question_type_id == 4)
            {
                testImage.SetActive(true);
                pictureQuestionAudio.SetActive(true);
                //StartCoroutine(DownloadSoundForQuestion(storageUrl + questionTxt.question.sound));
                foreach (GameObject pictureTextAnswer in pictureAnswerGO)
                {
                    pictureTextAnswer.SetActive(true);
                    foreach (GameObject textAnswer in textAnswerGO)
                    {
                        textAnswer.SetActive(false);
                    }
                }
                for (j = 0; j < pictureAnswerText.Count; j++)
                {
                    if (isEnglish == true)
                    {
                        if (questionTxt.answers[j].hide_text == true)
                        {
                            pictureAnswerText[j].enabled = false;
                        }
                        pictureAnswerText[j].text = questionTxt.answers[j].body.en;
                        pictureAnswerID[j].name = questionTxt.answers[j].id.ToString();
                        StartCoroutine(DownloadAnimalPictures((storageUrl + questionTxt.answers[j].image), animalPictureAnswers[j]));
                    }
                    if (isFrench == true)
                    {
                        if (questionTxt.answers[j].hide_text == true)
                        {
                            pictureAnswerText[j].enabled = false;
                        }
                        pictureAnswerText[j].text = questionTxt.answers[j].body.fr;
                        pictureAnswerID[j].name = questionTxt.answers[j].id.ToString();
                        StartCoroutine(DownloadAnimalPictures((storageUrl + questionTxt.answers[j].image), animalPictureAnswers[j]));
                    }
                }
            }

            if (questionTxt.question.question_type_id == 3)
            {
                testImage.SetActive(true);
                pictureQuestionAudio.SetActive(false);
                foreach (GameObject pictureTextAnswer in pictureAnswerGO)
                {
                    pictureTextAnswer.SetActive(true);
                    foreach (GameObject textAnswer in textAnswerGO)
                    {
                        textAnswer.SetActive(false);
                    }
                }
                for (j = 0; j < pictureAnswerText.Count; j++)
                {
                    if (isEnglish == true)
                    {
                        if (questionTxt.answers[j].hide_text == true)
                        {
                            pictureAnswerText[j].enabled = false;
                        }
                        StartCoroutine(DownloadAnimalPictures((storageUrl + questionTxt.answers[j].image), animalPictureAnswers[j]));
                        pictureAnswerText[j].text = questionTxt.answers[j].body.en;
                        pictureAnswerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                    if (isFrench == true)
                    {
                        if (questionTxt.answers[j].hide_text == true)
                        {
                            pictureAnswerText[j].enabled = false;
                        }
                        StartCoroutine(DownloadAnimalPictures((storageUrl + questionTxt.answers[j].image), animalPictureAnswers[j]));
                        pictureAnswerText[j].text = questionTxt.answers[j].body.fr;
                        pictureAnswerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                }
            }

            // If the question is video
            if (questionTxt.question.question_type_id == 2)
            {
                pictureQuestionAudio.SetActive(false);
                foreach (GameObject textAnswer in textAnswerGO)
                {
                    textAnswer.SetActive(true);
                }
                for (j = 0; j < answerText.Count; j++)
                {
                    if (isEnglish == true)
                    {

                        answerText[j].text = questionTxt.answers[j].body.en;
                        answerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                    if (isFrench == true)
                    {

                        answerText[j].text = questionTxt.answers[j].body.fr;
                        answerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                }
            }
        }
    }
    #endregion

    #region RewardsGetter
    public void GetRewards()
    {
        StartCoroutine(GetDataRewards());
    }

    IEnumerator GetDataRewards()
    {
        UnityWebRequest _www = UnityWebRequest.Get(jsonURLRewards);
        yield return _www.SendWebRequest();

        if (_www.isNetworkError || _www.isHttpError)
        {
            Debug.Log(_www.error);
        }
        else
        {
            rewardTxt = JsonUtility.FromJson<JSONDataRewards>(_www.downloadHandler.text);

            //  unused code for now
            //if (isEnglish == true)
            //{
            //    rewardsIDText.text = rewardTxt.title.en;
            //}
            //else if (isFrench == true)
            //{
            //    rewardsIDText.text = rewardTxt.title.sr;
            //}

            rewardsIDText.text = rewardTxt.title.en;
            StartCoroutine(DownloadRewardImage(storageUrl + rewardTxt.picture));
        }
    }
    #endregion

    // Downloads RewardImage for the reward
    #region DownloadingRewardImage
    IEnumerator DownloadRewardImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            rewardsImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
    #endregion


    // Downloads the sound for sound questions
    #region DownloadSoundForQuestion
    IEnumerator DownloadSoundForQuestion(string url)
    {
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            pictureQuestionAudioClip.clip = ((DownloadHandlerAudioClip)request.downloadHandler).audioClip;
        }
    }
    #endregion


    // Downloads the pictures for the answers that contain pictures in them.
    #region DownloadingAnimalPictures
    IEnumerator DownloadAnimalPictures(string url, RawImage animalPicture)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            animalPicture.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
    #endregion

    // Checks to see if the ID of the selected answer is the correct one
    #region CorrectAnswerChecker
    public void CheckIfAnswerCorrect()
    {
        if (EventSystem.current.currentSelectedGameObject.name == questionTxt.question.correct_answer_id.ToString())
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = correctAnswerColor;
            StartCoroutine(TurnOnCorrectAnswer());
            questionsAnsweredId.SetValue(questionTxt.question.id, q++);
        }
        else
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = wrongAnswerColor;
            StartCoroutine(TurnOnWrongAnswer());
            //coTest = StartCoroutine(TurnOffWrongAnswer());
        }
    }
    #endregion


    // Method for e-mail sending
    #region EmailSending
    public void SendEmailTest()
    {
        StartCoroutine(SendEmail());
    }


    IEnumerator SendEmail()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailInput.textComponent.text); // email koji pokupis iz inputa nekog
        form.AddField("reward_id", rewardTxt.id); // id od nagrade koji smo pokupili juce

        using (UnityWebRequest www = UnityWebRequest.Post("https://animalquest.ix42.com/api/subscribers", form))
        {
            yield return www.SendWebRequest();
            Debug.Log("working");
        }
        PlayerPrefs.SetString("Email", emailInput.textComponent.text);
    }
    #endregion


    // Method for calling the second question after sending e-mail 
    #region GetSecondQuestion
    public void GetSecondQuestion()
    {
        StartCoroutine(GetDataQuestions());
    }
    #endregion

    //#region GetFirstQuestion
    //public void GetFirstQuestion()
    //{
    //    StartCoroutine(GetDataFirstQuestions());
    //}
    //#endregion

    // Sets the PlayerPrefs to know that the user has completed tutorial and first question
    #region PlayerProgress
    public void FirstAnswerCorrect()
    {
        PlayerPrefs.SetString("FirstPass", "true");
    }
    #endregion

    //  Show the user that he has selected the correct answer;
    #region CorrectAnswerNew
    IEnumerator TurnOnCorrectAnswer()
    {
        yield return new WaitForSeconds(0.5f);
        correctAnswerPanel.SetActive(true);
    }
    #endregion

    // Show the user that he has selected the wrong answer
    #region WrongAnswer
    IEnumerator TurnOnWrongAnswer()
    {
        yield return new WaitForSeconds(0.3f);
        wrongAnswerPanel.SetActive(true);
    }

    IEnumerator TurnOffWrongAnswer()
    {
        yield return new WaitForSeconds(4f); // bilo je 2f
        wrongAnswerPanel.SetActive(false);
    }
    #endregion

    public void StopCoroutine()
    {
        //StopCoroutine(TurnOffWrongAnswer());
        //StopCoroutine(coTest);
    }

    // French Language settings
    #region FrenchLanguage
    public void LanguageChosenFrench()
    {
        isFrench = true;
        isEnglish = false;
        navBarEN.SetActive(true);
        navBarFR.SetActive(false);
        PlayerPrefs.SetString("LanguageFR", "french");
        PlayerPrefs.SetString("IsFrench", isFrench.ToString());
        PlayerPrefs.DeleteKey("IsEnglish");
    }
    #endregion

    // English Language settings
    #region EnglishLanguage
    public void LanguageChosenEnglish()
    {
        isEnglish = true;
        isFrench = false;
        navBarFR.SetActive(true);
        navBarEN.SetActive(false);
        PlayerPrefs.SetString("LanguageEN", "English");
        PlayerPrefs.SetString("IsEnglish", isEnglish.ToString());
        PlayerPrefs.DeleteKey("IsFrench");
    }
    #endregion


    /* Unused
    #region FirstQuestionGetter
    IEnumerator GetDataFirstQuestions()
    {
        UnityWebRequest _www = UnityWebRequest.Get(jsonURLQuestions + "/27");
        //_www.SetRequestHeader("isSerbian", "true");
        yield return _www.SendWebRequest();

        if (_www.isNetworkError || _www.isHttpError)
        {
            Debug.Log(_www.error);
        }
        else
        {
            questionTxt = JsonUtility.FromJson<JSONDataClassQuestions>(_www.downloadHandler.text);

            if (isEnglish == true)
            {
                questionText.text = questionTxt.question.body.en;
                clueText.text = questionTxt.question.clue_en;
                SwapClueMarker();
            }
            else if (isFrench == true)
            {
                questionText.text = questionTxt.question.body.fr;
                clueText.text = questionTxt.question.clue;
                SwapClueMarker();
            }

            answerCount = questionTxt.answers.Count;

            // If the question is text
            if (questionTxt.question.question_type_id == 1)
            {
                foreach (GameObject textAnswer in textAnswerGO)
                {
                    textAnswer.SetActive(true);
                }
                for (j = 0; j < answerText.Count; j++)
                {
                    if (isEnglish == true)
                    {

                        answerText[j].text = questionTxt.answers[j].body.en;
                        answerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                    if (isFrench == true)
                    {

                        answerText[j].text = questionTxt.answers[j].body.fr;
                        answerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                }
            }


            // If the question is sound
            if (questionTxt.question.question_type_id == 4)
            {
                testImage.SetActive(true);
                pictureQuestionAudio.SetActive(true);
                StartCoroutine(DownloadSoundForQuestion(storageUrl + questionTxt.question.sound));

                foreach (GameObject pictureTextAnswer in pictureAnswerGO)
                {
                    pictureTextAnswer.SetActive(true);
                    foreach (GameObject textAnswer in textAnswerGO)
                    {
                        textAnswer.SetActive(false);
                    }
                }
                for (j = 0; j < pictureAnswerText.Count; j++)
                {
                    if (isEnglish == true)
                    {
                        if (questionTxt.answers[j].hide_text == true)
                        {
                            pictureAnswerText[j].enabled = false;
                        }
                        pictureAnswerText[j].text = questionTxt.answers[j].body.en;
                        pictureAnswerID[j].name = questionTxt.answers[j].id.ToString();
                        StartCoroutine(DownloadAnimalPictures((storageUrl + questionTxt.answers[j].image), animalPictureAnswers[j]));
                    }
                    if (isFrench == true)
                    {
                        if (questionTxt.answers[j].hide_text == true)
                        {
                            pictureAnswerText[j].enabled = false;
                        }
                        else
                        {
                            pictureAnswerText[j].text = questionTxt.answers[j].body.fr;
                        }
                        pictureAnswerID[j].name = questionTxt.answers[j].id.ToString();
                        StartCoroutine(DownloadAnimalPictures((storageUrl + questionTxt.answers[j].image), animalPictureAnswers[j]));
                    }
                }
            }
            if (questionTxt.question.question_type_id == 3)
            {
                testImage.SetActive(true);
                foreach (GameObject pictureTextAnswer in pictureAnswerGO)
                {
                    pictureTextAnswer.SetActive(true);
                    foreach (GameObject textAnswer in textAnswerGO)
                    {
                        textAnswer.SetActive(false);
                    }
                }
                for (j = 0; j < pictureAnswerText.Count; j++)
                {
                    if (isEnglish == true)
                    {
                        if (questionTxt.answers[j].hide_text == true)
                        {
                            pictureAnswerText[j].enabled = false;
                        }
                        StartCoroutine(DownloadAnimalPictures((storageUrl + questionTxt.answers[j].image), animalPictureAnswers[j]));
                        pictureAnswerText[j].text = questionTxt.answers[j].body.en;
                        pictureAnswerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                    if (isFrench == true)
                    {
                        if (questionTxt.answers[j].hide_text == true)
                        {
                            pictureAnswerText[j].enabled = false;
                        }
                        StartCoroutine(DownloadAnimalPictures((storageUrl + questionTxt.answers[j].image), animalPictureAnswers[j]));
                        pictureAnswerText[j].text = questionTxt.answers[j].body.fr;
                        pictureAnswerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                }
            }


            // If the question is video
            if (questionTxt.question.question_type_id == 2)
            {
                foreach (GameObject textAnswer in textAnswerGO)
                {
                    textAnswer.SetActive(true);
                }
                for (j = 0; j < answerText.Count; j++)
                {
                    if (isEnglish == true)
                    {

                        answerText[j].text = questionTxt.answers[j].body.en;
                        answerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                    if (isFrench == true)
                    {

                        answerText[j].text = questionTxt.answers[j].body.fr;
                        answerID[j].name = questionTxt.answers[j].id.ToString();
                    }
                }
            }
        }
    }
    #endregion
    */

    public void SwapClueMarker()
    {
        if (questionTxt.question.animal_marker != null)
        {
            if (questionTxt.question.animal_marker.Equals("Boa"))
            {
                animalToDetect = questionTxt.question.animal_marker;
                clueImage.sprite = clueAnimalMarkerSprite[0];
            }
            if (questionTxt.question.animal_marker.Equals("Giraffe"))
            {
                animalToDetect = questionTxt.question.animal_marker;
                clueImage.sprite = clueAnimalMarkerSprite[1];
            }
            if (questionTxt.question.animal_marker.Equals("Elephant"))
            {
                animalToDetect = questionTxt.question.animal_marker;
                clueImage.sprite = clueAnimalMarkerSprite[2];
            }
            if (questionTxt.question.animal_marker.Equals("Ara"))
            {
                animalToDetect = questionTxt.question.animal_marker;
                clueImage.sprite = clueAnimalMarkerSprite[8];
            }
            if (questionTxt.question.animal_marker.Equals("Wolf"))
            {
                animalToDetect = questionTxt.question.animal_marker;
                clueImage.sprite = clueAnimalMarkerSprite[10];
            }
            if (questionTxt.question.animal_marker.Equals("Beaver"))
            {
                animalToDetect = questionTxt.question.animal_marker;
                clueImage.sprite = clueAnimalMarkerSprite[13];
            }
            //if (questionTxt.question.animal_marker.Equals("Dolphin"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[3];
            //}
            //if (questionTxt.question.animal_marker.Equals("Stingray"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[4];
            //}
            //if (questionTxt.question.animal_marker.Equals("Hammerhead"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[5];
            //}
            //if (questionTxt.question.animal_marker.Equals("Panda"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[6];
            //}
            //if (questionTxt.question.animal_marker.Equals("Flamingo"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[7];
            //}

            //if (questionTxt.question.animal_marker.Equals("Polarbear"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[9];
            //}
            //if (questionTxt.question.animal_marker.Equals("Zebra"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[11];
            //}
            //if (questionTxt.question.animal_marker.Equals("Crocodile"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[12];
            //}
            //if (questionTxt.question.animal_marker.Equals("Deer"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[14];
            //}
            //if (questionTxt.question.animal_marker.Equals("Lynx"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[15];
            //}
            //if (questionTxt.question.animal_marker.Equals("Jaguar"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[16];
            //}
            //if (questionTxt.question.animal_marker.Equals("Pangolin"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[17];
            //}
            //if (questionTxt.question.animal_marker.Equals("Lion"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[17];
            //}
            //if (questionTxt.question.animal_marker.Equals("Tiger"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[18];
            //}
            //if (questionTxt.question.animal_marker.Equals("Hyena"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[19];
            //}
            //if (questionTxt.question.animal_marker.Equals("Pangolin"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[20];
            //}
            //if (questionTxt.question.animal_marker.Equals("SpottedHyena"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[21];
            //}
            //if (questionTxt.question.animal_marker.Equals("TasmanianDevil"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[22];
            //}
            //if (questionTxt.question.animal_marker.Equals("Wallaby"))
            //{
            //    animalToDetect = questionTxt.question.animal_marker;
            //    clueImage.sprite = clueAnimalMarkerSprite[23];
            //}
        }

    }


    public void QuestionHasBeenAnswered()
    {
        currentQuestionID = 0;
    }

    public void DeleteKey()
    {
        PlayerPrefs.DeleteKey("LastQuestion");
    }

    public void SaveArrayPref()
    {
        PlayerPrefsX.SetIntArray("QuestionsAnswered", questionsAnsweredId);
        Debug.Log("SavedArray");
    }

    public void IncreaseCount()
    {
        if (wentBackForClue == false)
        {
            wildTourQuestionID++;
        }
        else
        {
            return;
        }
        PlayerPrefs.SetInt("CurrentQuestion", wildTourQuestionID);
    }

    public void IncreaseHumanSprite()
    {
        humanSpriteCounter++;
        PlayerPrefs.SetInt("SpriteCount", humanSpriteCounter);
    }
    public void WentBackForClue()
    {
        wentBackForClue = true;
    }

    public void CheckedTheClue()
    {
        wentBackForClue = false;
    }

    //public void FormatLinkString()
    //{
    //    filterString = string.Format($"?except[]={questionsAnsweredId[0]}&except[]={questionsAnsweredId[1]}&except[]={questionsAnsweredId[2]}&except[]={questionsAnsweredId[3]}&except[]={questionsAnsweredId[4]}&except[]={questionsAnsweredId[5]}&except[]={questionsAnsweredId[6]}&except[]={questionsAnsweredId[7]}&except[]={questionsAnsweredId[8]}" +
    //        $"&except[]={questionsAnsweredId[9]}&except[]={questionsAnsweredId[10]}&except[]={questionsAnsweredId[11]}&except[]={questionsAnsweredId[12]}&except[]={questionsAnsweredId[13]}&except[]={questionsAnsweredId[14]}&except[]={questionsAnsweredId[15]}");
    //}
}

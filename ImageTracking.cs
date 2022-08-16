using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    public ARTrackedImageManager trackedImageManager;


    public GameObject scanBar, scanningBar;
    public Text scanBarTXT;
    public Image scanBarImage;
    public GameObject infoButton, cameraButton, presentButton, soundButton, scanMarker, scanText;
    public GameObject scaleUpButton, scaleDownButton, inventoryButton;

    public List<Sprite> animalImage;


    public string animalQuizUrlGiraffe;
    public string animalQuizUrlPenguin;
    public BrowserOpener browserOpener;

    public Color waterColor;
    public Color jungleColor;
    public Color desertColor;
    public Color polarColor;

    public Image lockBoa;
    public Image lockGiraffe;
    public Image lockDolphin;
    public Image lockPenguin;
    public Image lockHammerhead;
    public Image lockStingray;
    public Image lockPanda;
    public Image lockAra;
    public Image lockPolarBear;
    public Image lockWolf;
    public Image lockZebra;
    public Image lockCrocodile;
    public Image lockDeer;
    public Image lockBeaver;
    public Image lockFlamingo;
    public Image lockLynx;

    public GameObject lockBoaText;
    public GameObject lockGiraffeText;
    public GameObject lockDolphinText;
    public GameObject lockPenguinText;
    public GameObject lockHammerheadText;
    public GameObject lockStingrayText;
    public GameObject lockPandaText;
    public GameObject lockAraText;
    public GameObject lockPolarbearText;
    public GameObject lockWolfText;
    public GameObject lockZebraText;
    public GameObject lockCrocodileText;
    public GameObject lockDeerText;
    public GameObject lockBeaverText;
    public GameObject lockFlamingoText;
    public GameObject lockLynxText;

    public Button lockBoaButton;
    public Button lockGiraffeButton;
    public Button lockDolphinButton;
    public Button lockPenguinButton;
    public Button lockHammerheadButton;
    public Button lockStingrayButton;
    public Button lockPandaButton;
    public Button lockAraButton;
    public Button lockPolarbearButton;
    public Button lockWolfButton;
    public Button lockZebraButton;
    public Button lockCrocButton;
    public Button lockDeerButton;
    public Button lockBeaverButton;
    public Button lockFlamingoButton;
    public Button lockLynxButton;

    GameObject animalPrefab;

    public bool canScaleObj;

    public bool backToScan;


    public AudioClip polarBearClip;
    public AudioSource animalAudioSource;

    public JSONController jSONController;

    public int markersScannedCounter;

    public bool scannedDolphin = false;
    public bool scannedBoa = false;
    public bool scannedPanda = false;
    public bool scannedGiraffe = false;
    public bool scannedShark = false;
    public bool scannedStingray = false;
    public bool scannedAra = false;
    public bool scannedPolarbear = false;
    public bool scannedWolf = false;
    public bool scannedZebra = false;
    public bool scannedDeer = false;
    public bool scannedCroc = false;
    public bool scannedBeaver = false;
    public bool scannedFlamingo = false;
    public bool scannedLynx = false;
    public bool scannedPenguin = false;

    public Image nextButtonImg;
    public Sprite nextButtonSpriteGreen;
    public Sprite nextButtonSpriteGray;

    [Header("Wrong Animal Scanned")]
    public GameObject wrongAnimalScanned;
    public bool wrongAnimalScannedBool;

    public bool correctAnimalScanned;

    public GameObject inventoryButtonQuiz;
    public GameObject restartQuestButtonQuiz;


    public bool popupShouldBeClosed = false;

    public TurnOffPopup turnOffPopupScript;

    public ImageTracking imageTracking;

    public Animator markersScannedCounterAnimator;

    public bool isFirstPath;
    public bool isSecondPath;


    public bool isFirstPathReversed;
    public bool isSecondPathReversed;

    public GameObject writingText;

    //public SkipTimer skipTimer;
    private void Awake()
    {
        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, prefab.transform.rotation);
            //GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            //GameObject newPrefab = Instantiate(prefab, prefab.transform.position, Quaternion.identity);
            newPrefab.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
    }

    private void Start()
    {
        animalPrefab = null;
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs obj)
    {
        foreach (ARTrackedImage trackedImage in obj.added)
        {
            AddARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in obj.updated)
        {
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in obj.removed)
        {
            spawnedPrefabs[trackedImage.name].SetActive(false);
        }
    }
    private void UpdateARImage(ARTrackedImage trackedImage)
    {
        // Assign and Place Game Object
        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position, trackedImage);
    }

    private void AddARImage(ARTrackedImage trackedImage)
    {
        AssignGameObjectOnAdded(trackedImage.referenceImage.name, trackedImage.transform.position, trackedImage);
    }
    void AssignGameObject(string name, Vector3 newPosition, ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            if (name == "Ship")
            {
                writingText.SetActive(true);
                Debug.Log("That's the shiitt");
            }

            if (jSONController.animalToDetect == name)
            {
                if (placeablePrefabs != null)
                {
                    animalPrefab = spawnedPrefabs[name];
                    animalPrefab.SetActive(true);
                    animalPrefab.transform.position = newPosition;
                    StartCoroutine(TurnOnAnimator(animalPrefab.GetComponentInChildren<Animator>()));
                }
                turnOffPopupScript.lookingForAnimalNewAnimal = false;
                scanMarker.SetActive(false);
                scanText.SetActive(false);
                scanningBar.SetActive(false);
                scanBar.SetActive(true);

                cameraButton.SetActive(true);
                infoButton.SetActive(true);
                inventoryButton.SetActive(true);

                foreach (GameObject go in spawnedPrefabs.Values)
                {
                    if (go.name != name)
                    {
                        go.SetActive(false);
                    }
                }
                nextButtonImg.sprite = nextButtonSpriteGreen;
                nextButtonImg.gameObject.GetComponent<Button>().enabled = true;

                inventoryButtonQuiz.SetActive(true);
                restartQuestButtonQuiz.SetActive(true);
                if (name == "Beaver")
                {
                    PlayerPrefs.SetString("Beaver", "scanned");
                    scanBarImage.sprite = animalImage[13];
                    lockBeaver.sprite = animalImage[13];
                    lockBeaver.color = waterColor;
                    lockBeaverText.SetActive(true);
                    lockBeaverButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Castor";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedBeaver == false)
                    {
                        scannedBeaver = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("BeaverScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
                if (name == "Giraffe")
                {
                    PlayerPrefs.SetString("Giraffe", "scanned");
                    scanBarImage.sprite = animalImage[2];
                    lockGiraffe.sprite = animalImage[2];
                    lockGiraffe.color = desertColor;
                    browserOpener.pageToOpen = animalQuizUrlGiraffe;
                    lockGiraffeText.SetActive(true);
                    lockGiraffeButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Girafe";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedGiraffe == false)
                    {
                        scannedGiraffe = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("GiraffeScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
                if (name == "Ara")
                {
                    PlayerPrefs.SetString("Ara", "scanned");
                    scanBarImage.sprite = animalImage[7];
                    lockAra.sprite = animalImage[7];
                    lockAra.color = jungleColor;
                    lockAraText.SetActive(true);
                    lockAraButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Ara";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedAra == false)
                    {
                        scannedAra = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("AraScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
                if (name == "Elephant") // Elephant
                {
                    PlayerPrefs.SetString("Penguin", "scanned");
                    scanBarImage.sprite = animalImage[5];
                    lockPenguin.sprite = animalImage[5];
                    lockPenguin.color = polarColor;
                    browserOpener.pageToOpen = animalQuizUrlPenguin;
                    lockPenguinText.SetActive(true);
                    lockPenguinButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Elephant";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedPenguin == false)
                    {
                        scannedPenguin = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("PenguinScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
                if (name == "Wolf")
                {
                    scanBarImage.sprite = animalImage[9];
                    lockWolf.sprite = animalImage[9];
                    lockWolf.color = desertColor;
                    lockWolfText.SetActive(true);
                    lockWolfButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Loup";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedWolf == false)
                    {
                        scannedWolf = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("WolfScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
                if (name == "Boa")
                {
                    PlayerPrefs.SetString("Boa", "scanned");
                    scanBarImage.sprite = animalImage[1];
                    lockBoa.sprite = animalImage[1];
                    lockBoa.color = jungleColor;
                    lockBoaText.SetActive(true);
                    lockBoaButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Boa";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedBoa == false)
                    {
                        scannedBoa = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("BoaScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }

            }
            else
            {
                turnOffPopupScript.lookingForAnimalNewAnimal = false;
                nextButtonImg.sprite = nextButtonSpriteGray;
                nextButtonImg.gameObject.GetComponent<Button>().enabled = false;
                //if (wrongAnimalScannedBool == false)
                //{
                //    wrongAnimalScanned.SetActive(true);
                //    wrongAnimalScannedBool = true;
                //}
                name = null;
            }
        }
    }

    void AssignGameObjectOnAdded(string name, Vector3 newPosition, ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            if (jSONController.animalToDetect == name)
            {
                if (placeablePrefabs != null)
                {
                    animalPrefab = spawnedPrefabs[name];
                    animalPrefab.SetActive(true);
                    StartCoroutine(TurnOnAnimator(animalPrefab.GetComponentInChildren<Animator>()));
                    animalPrefab.transform.position = newPosition;
                }
                turnOffPopupScript.lookingForAnimalNewAnimal = false;
                scanMarker.SetActive(false);
                scanText.SetActive(false);
                scanningBar.SetActive(false);
                scanBar.SetActive(true);
                scanBarTXT.text = name;

                cameraButton.SetActive(true);
                infoButton.SetActive(true);
                inventoryButton.SetActive(true);

                foreach (GameObject go in spawnedPrefabs.Values)
                {
                    if (go.name != name)
                    {
                        go.SetActive(false);
                    }
                }
                nextButtonImg.sprite = nextButtonSpriteGreen;
                nextButtonImg.gameObject.GetComponent<Button>().enabled = true;

                inventoryButtonQuiz.SetActive(true);
                restartQuestButtonQuiz.SetActive(true);

                if (name == "Beaver")
                {
                    PlayerPrefs.SetString("Beaver", "scanned");
                    scanBarImage.sprite = animalImage[13];
                    lockBeaver.sprite = animalImage[13];
                    lockBeaver.color = waterColor;
                    lockBeaverText.SetActive(true);
                    lockBeaverButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Castor";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedBeaver == false)
                    {
                        scannedBeaver = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("BeaverScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
                if (name == "Giraffe")
                {
                    PlayerPrefs.SetString("Giraffe", "scanned");
                    scanBarImage.sprite = animalImage[2];
                    lockGiraffe.sprite = animalImage[2];
                    lockGiraffe.color = desertColor;
                    browserOpener.pageToOpen = animalQuizUrlGiraffe;
                    lockGiraffeText.SetActive(true);
                    lockGiraffeButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Girafe";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedGiraffe == false)
                    {
                        scannedGiraffe = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("GiraffeScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
                if (name == "Ara")
                {
                    PlayerPrefs.SetString("Ara", "scanned");
                    scanBarImage.sprite = animalImage[7];
                    lockAra.sprite = animalImage[7];
                    lockAra.color = jungleColor;
                    lockAraText.SetActive(true);
                    lockAraButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Ara";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedAra == false)
                    {
                        scannedAra = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("AraScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
                if (name == "Elephant") // Elephant
                {
                    PlayerPrefs.SetString("Penguin", "scanned");
                    scanBarImage.sprite = animalImage[5];
                    lockPenguin.sprite = animalImage[5];
                    lockPenguin.color = polarColor;
                    browserOpener.pageToOpen = animalQuizUrlPenguin;
                    lockPenguinText.SetActive(true);
                    lockPenguinButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Elephant";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedPenguin == false)
                    {
                        scannedPenguin = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("PenguinScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
                if (name == "Wolf")
                {
                    scanBarImage.sprite = animalImage[9];
                    lockWolf.sprite = animalImage[9];
                    lockWolf.color = desertColor;
                    lockWolfText.SetActive(true);
                    lockWolfButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Loup";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedWolf == false)
                    {
                        scannedWolf = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("WolfScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
                if (name == "Boa")
                {
                    PlayerPrefs.SetString("Boa", "scanned");
                    scanBarImage.sprite = animalImage[1];
                    lockBoa.sprite = animalImage[1];
                    lockBoa.color = jungleColor;
                    lockBoaText.SetActive(true);
                    lockBoaButton.enabled = true;
                    if (jSONController.isFrench == true)
                    {
                        scanBarTXT.text = "Boa";
                    }
                    else
                    {
                        scanBarTXT.text = name;
                    }
                    if (scannedBoa == false)
                    {
                        scannedBoa = true;
                        markersScannedCounterAnimator.enabled = true;
                        markersScannedCounter++;
                        StartCoroutine(TurnOfFScanbar());
                        PlayerPrefs.SetString("BoaScanned", "scanned");
                    }
                    PlayerPrefs.Save();
                }
            }
            else
            {
                turnOffPopupScript.lookingForAnimalNewAnimal = false;
                nextButtonImg.sprite = nextButtonSpriteGray;
                nextButtonImg.gameObject.GetComponent<Button>().enabled = false;
                //if (wrongAnimalScannedBool == false)
                //{
                //    wrongAnimalScanned.SetActive(true);
                //    wrongAnimalScannedBool = true;
                //}
                name = null;
            }

            //if (name == null)
            //{
            //    scanMarker.SetActive(true);
            //    scanText.SetActive(true);
            //    scanningBar.SetActive(true);
            //    scanBar.SetActive(false);
            //    cameraButton.SetActive(false);
            //    infoButton.SetActive(false);
            //    inventoryButton.SetActive(false);
            //}
            //if (jSONController.animalToDetect != name && name != null)
            //{
            //    name = null;
            //    nextButtonImg.sprite = nextButtonSpriteGray;
            //    nextButtonImg.gameObject.GetComponent<Button>().enabled = false;
            //    if (wrongAnimalScannedBool == false)
            //    {
            //        wrongAnimalScannedBool = true;
            //        wrongAnimalScanned.SetActive(true);
            //    }
            //}
        }
    }

    public void TurnOffAnimals()
    {
        ////if (GameObject.Find("Lion") != null)
        ////{
        ////    GameObject.Find("Lion").SetActive(false);
        ////}
        ////if (GameObject.Find("Hyena") != null)
        ////{
        ////    GameObject.Find("Hyena").SetActive(false);
        ////}
        ////if (GameObject.Find("Pangolin") != null)
        ////{
        ////    GameObject.Find("Pangolin").SetActive(false);
        ////}
        ////if (GameObject.Find("SpottedHyena") != null)
        ////{
        ////    GameObject.Find("SpottedHyena").SetActive(false);
        ////}
        ////if (GameObject.Find("Tiger") != null)
        ////{
        ////    GameObject.Find("Tiger").SetActive(false);
        ////}
        ////if (GameObject.Find("Wallaby") != null)
        ////{
        ////    GameObject.Find("Wallaby").SetActive(false);
        ////}

        //#region unused
        ////if (GameObject.Find("Dolphin") != null)
        ////{
        ////    GameObject.Find("Dolphin").SetActive(false);
        ////}
        ////}
        ////if (GameObject.Find("Boa") != null)
        ////{
        ////    GameObject.Find("Boa").SetActive(false);
        ////}
        //if (GameObject.Find("Giraffe") != null)
        //{
        //    GameObject.Find("Giraffe").SetActive(false);
        //}
        ////if (GameObject.Find("Hammerhead") != null)
        ////{
        ////    GameObject.Find("Hammerhead").SetActive(false);
        ////}
        //if (GameObject.Find("Panda") != null)
        //{
        //    GameObject.Find("Panda").SetActive(false);
        //}
        //if (GameObject.Find("Penguin") != null)
        //{
        //    GameObject.Find("Penguin").SetActive(false);
        //}
        ////if (GameObject.Find("Stingray") != null)
        ////{
        ////    GameObject.Find("Stingray").SetActive(false);
        ////}
        ////if (GameObject.Find("Ara") != null)
        ////{
        ////    GameObject.Find("Ara").SetActive(false);
        ////}
        ////if (GameObject.Find("Polarbear") != null)
        ////{
        ////    GameObject.Find("Polarbear").SetActive(false);
        ////}
        ////if (GameObject.Find("Wolf") != null)
        ////{
        ////    GameObject.Find("Wolf").SetActive(false);
        ////}
        //if (GameObject.Find("Zebra") != null)
        //{
        //    GameObject.Find("Zebra").SetActive(false);
        //}
        //if (GameObject.Find("Crocodile") != null)
        //{
        //    GameObject.Find("Crocodile").SetActive(false);
        //}
        ////if (GameObject.Find("Deer") != null)
        ////{
        ////    GameObject.Find("Deer").SetActive(false);
        ////}
        ////if (GameObject.Find("Beaver") != null)
        ////{
        ////    GameObject.Find("Beaver").SetActive(false);
        ////}
        ////if (GameObject.Find("Flamingo") != null)
        ////{
        ////    GameObject.Find("Flamingo").SetActive(false);
        ////}
        ////if (GameObject.Find("Lynx") != null)
        ////{
        ////    GameObject.Find("Lynx").SetActive(false);
        ////}
        //#endregion
    }

    IEnumerator TurnOnAnimator(Animator animator)
    {
        yield return new WaitForSeconds(1.0f);
        animator.enabled = true;
    }
    IEnumerator TurnOfFScanbar()
    {
        yield return new WaitForSeconds(1.0f);
        markersScannedCounterAnimator.enabled = false;
        scanBar.SetActive(false);
    }


    public void BoolToFalse()
    {
        wrongAnimalScannedBool = false;
    }

    public void SkipScanTest()
    {
        if (jSONController.animalToDetect == "Beaver" && scannedBeaver == false)
        {
            PlayerPrefs.SetString("Beaver", "scanned");
            scanBarImage.sprite = animalImage[13];
            lockBeaver.sprite = animalImage[13];
            lockBeaver.color = waterColor;
            lockBeaverText.SetActive(true);
            lockBeaverButton.enabled = true;
            scannedBeaver = true;
            markersScannedCounterAnimator.enabled = true;
            markersScannedCounter++;
            StartCoroutine(TurnOfFScanbar());
            PlayerPrefs.SetString("BeaverScanned", "scanned");
            PlayerPrefs.Save();
        }
        if (jSONController.animalToDetect == "Giraffe" && scannedGiraffe == false)
        {
            PlayerPrefs.SetString("Giraffe", "scanned");
            scanBarImage.sprite = animalImage[2];
            lockGiraffe.sprite = animalImage[2];
            lockGiraffe.color = desertColor;
            browserOpener.pageToOpen = animalQuizUrlGiraffe;
            lockGiraffeText.SetActive(true);
            lockGiraffeButton.enabled = true;
            scannedGiraffe = true;
            markersScannedCounterAnimator.enabled = true;
            markersScannedCounter++;
            StartCoroutine(TurnOfFScanbar());
            PlayerPrefs.SetString("GiraffeScanned", "scanned");
            PlayerPrefs.Save();
        }
        if (jSONController.animalToDetect == "Ara" && scannedAra == false)
        {
            PlayerPrefs.SetString("Ara", "scanned");
            scanBarImage.sprite = animalImage[7];
            lockAra.sprite = animalImage[7];
            lockAra.color = jungleColor;
            lockAraText.SetActive(true);
            lockAraButton.enabled = true;
            scannedAra = true;
            markersScannedCounterAnimator.enabled = true;
            markersScannedCounter++;
            StartCoroutine(TurnOfFScanbar());
            PlayerPrefs.SetString("AraScanned", "scanned");
            PlayerPrefs.Save();
        }
        if (jSONController.animalToDetect == "Elephant" && scannedPenguin == false)
        {
            PlayerPrefs.SetString("Penguin", "scanned");
            scanBarImage.sprite = animalImage[5];
            lockPenguin.sprite = animalImage[5];
            lockPenguin.color = polarColor;
            browserOpener.pageToOpen = animalQuizUrlPenguin;
            lockPenguinText.SetActive(true);
            lockPenguinButton.enabled = true;
            scannedPenguin = true;
            markersScannedCounterAnimator.enabled = true;
            markersScannedCounter++;
            StartCoroutine(TurnOfFScanbar());
            PlayerPrefs.SetString("PenguinScanned", "scanned");
            PlayerPrefs.Save();
        }
        if (jSONController.animalToDetect == "Wolf" && scannedWolf == false)
        {
            scanBarImage.sprite = animalImage[9];
            lockWolf.sprite = animalImage[9];
            lockWolf.color = desertColor;
            lockWolfText.SetActive(true);
            lockWolfButton.enabled = true;
            scannedWolf = true;
            markersScannedCounterAnimator.enabled = true;
            markersScannedCounter++;
            StartCoroutine(TurnOfFScanbar());
            PlayerPrefs.SetString("WolfScanned", "scanned");
            PlayerPrefs.Save();
        }
        if (jSONController.animalToDetect == "Boa" && scannedBoa == false)
        {
            PlayerPrefs.SetString("Boa", "scanned");
            scanBarImage.sprite = animalImage[1];
            lockBoa.sprite = animalImage[1];
            lockBoa.color = jungleColor;
            lockBoaText.SetActive(true);
            lockBoaButton.enabled = true;
            scannedBoa = true;
            markersScannedCounterAnimator.enabled = true;
            markersScannedCounter++;
            StartCoroutine(TurnOfFScanbar());
            PlayerPrefs.SetString("BoaScanned", "scanned");
            PlayerPrefs.Save();
        }
    }
    /*
     * if (jSONController.animalToDetect == name)
            {
                nextButtonImg.sprite = nextButtonSpriteGreen;
                nextButtonImg.gameObject.GetComponent<Button>().enabled = true;
            }
            else
            {
                nextButtonImg.sprite = nextButtonSpriteGray;
                nextButtonImg.gameObject.GetComponent<Button>().enabled = false;
                if (wrongAnimalScannedBool == false)
                {
                    wrongAnimalScanned.SetActive(true);
                    wrongAnimalScannedBool = true;
                }
            }
     */
}

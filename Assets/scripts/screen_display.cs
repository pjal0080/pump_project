using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Net;

public class screen_display : MonoBehaviour
{   
    private string myfilePath;
    private string imgFile;
    private string base_dir;
    private int init = 0;
    private string[] images = new string[100];
    private string[] texts = new string[100];
    private string[] audios = new string[100];
    private int number_of_scene;
    private int scene_number;
    private string[] allPaths;
    private AudioClip audio_source;
    private Texture2D tex;
    private GameObject button1;
    private GameObject button2;
    private GameObject audio_button;
    //****  all public variable below  ****
    public Renderer thisRenderer;
    public GameObject buttonPrefab;
    public GameObject panelToAttachButtonsTo;
    public Text displayMessage;
    private AudioSource audio_z;
    public AudioClip clip1;
    public AudioClip clip3;
    public GameObject PrefabAudioPlay;

    // Start is called before the first frame update
    void Start()
    {
        audio_z = GetComponent<AudioSource>();
        //audio_z.transform.SetParent(panelToAttachButtonsTo.transform);        

        //Internal_Storage/android/data/YOUR_APP/files/ camera_log.txt
        myfilePath = Application.persistentDataPath + "/log.txt";
        //imgDir = Application.persistentDataPath;
        //base_dir = "E:\\suin\\codes\\Unity\\pump_project\\data_passionate_solver\\";

        if (File.Exists(myfilePath))
        {
            try
            {
                File.Delete(myfilePath);
                Debug.Log("Deleted the file....\n");
            }
            catch (System.Exception e)
            {
                Debug.Log("Not able to delete the file....\n"+e);
            }
        }


        allPaths = Directory.GetFiles(Application.dataPath, "*jpg", SearchOption.AllDirectories);
        //foreach (var path in allPaths) { Debug.Log(path); }

        //string screen_data_path = base_dir + "screen_data.csv";
        string screen_data_path = Application.persistentDataPath + "/screen_data_web.csv";
        using (var client = new WebClient())
        {
            client.DownloadFile("https://docs.google.com/spreadsheets/d/e/2PACX-1vRDEpkPPdzzeI6Au_NTtAP62ZRH38F_31kQqrZGtjtTEGQd7BWIrZfoTYffAhPDaOAzB4_pawQ_Yim-/pub?gid=1421813275&single=true&output=csv", screen_data_path);
        }
        wtf("screen_data_path : " + screen_data_path);
        int arr = 0;
        using (StreamReader rd = new StreamReader(screen_data_path))
        {            
            while (!rd.EndOfStream)
            {
                String[] value = null;
                string splits = rd.ReadLine();
                value = splits.Split(',');

                int tmp = 0;
                string img_name_tmp="xa";
                foreach (var test in value)
                {
                    //Debug.Log(test);
                    if (tmp == 1)
                    {
                        images[arr] = test;
                        img_name_tmp = images[arr];                        
                    }
                    if (tmp == 2) { texts[arr] = test; }
                    if (tmp == 3) { audios[arr] = test; }
                    if (tmp == 4 && arr != 0 && texts[arr]!="0")
                    {
                        try
                        {
                            //Debug.Log("img_name_tmp : "+img_name_tmp);
                            string img_path = Application.persistentDataPath + "/" + img_name_tmp;
                            Debug.Log("img_path : "+img_path);
                            //Debug.Log("image link : "+test);
                            if (!File.Exists(img_path))
                            {
                                Debug.Log("file doesn't exists.url = "+test+" Downloading file ....  "+ img_path);
                                using (var client = new WebClient())
                                {
                                    client.DownloadFile(test, img_path);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            wtf("\nException in getting image link : " + e);
                            Debug.Log("\nException in getting image link : " + e);
                        }

                    }
                    tmp++;
                }
                arr++;
            }
        }
        //foreach (var image in images) { Debug.Log(image); }
        //foreach (var audio in audios) { Debug.Log(audio); }
        //for (var f = 1; f < arr; f++) { Debug.Log(images[f].ToString()); }
        number_of_scene = arr-1;
        scene_number = 1;
        create_scene();
    }

    public void create_scene()
    {
        wtf("\nCreating scene....");
        Debug.Log("\nCreating scene...."+ images[scene_number]);
        try
        {
            if (init == 0)
                init = 1;
            else
            {
                Destroy(tex);
                Destroy(button1);
                Destroy(button2);
                Destroy(audio_button);
            }
            if (images[scene_number]=="0")
            {
                Debug.Log("\ninside manual screen creation ....");
                displayMessage.text = "Scene " + scene_number + " \n  " + "Create this scene manually";
                //******Manual screen creation section*******

                //****************
                if (scene_number != number_of_scene)
                {
                    button1 = (GameObject)Instantiate(buttonPrefab, new Vector3(800, 200, 0), Quaternion.identity);
                    button1.transform.SetParent(panelToAttachButtonsTo.transform);
                    button1.GetComponent<Button>().onClick.AddListener(next_scene);
                    button1.transform.GetChild(0).GetComponent<Text>().text = "Next";
                }

                if (scene_number != 1)
                {
                    button2 = (GameObject)Instantiate(buttonPrefab, new Vector3(250, 200, 0), Quaternion.identity);
                    button2.transform.SetParent(panelToAttachButtonsTo.transform);
                    button2.GetComponent<Button>().onClick.AddListener(previous_scene);
                    button2.transform.GetChild(0).GetComponent<Text>().text = "Previous";
                }
                return;
            }

            
            //scn = inputScene.text;
            //imgFile = base_dir + "image\\" +images[scene_number];
            //imgFile = allPaths[scene_number - 1];
            imgFile = Application.persistentDataPath + "/" + images[scene_number];
            wtf("image file path : " + imgFile);
            Debug.Log("\n image file : " + imgFile);
            tex = new Texture2D(1280, 720);
            byte[] imgByte = File.ReadAllBytes(imgFile);
            tex.LoadImage(imgByte);
            tex.Apply();
            thisRenderer.material.mainTexture = tex;

            displayMessage.text = "Scene "+scene_number +" \n  "+ texts[scene_number];

            if(scene_number!= number_of_scene) {
                button1 = (GameObject)Instantiate(buttonPrefab, new Vector3(800, 200, 0), Quaternion.identity);
                button1.transform.SetParent(panelToAttachButtonsTo.transform);
                button1.GetComponent<Button>().onClick.AddListener(next_scene);
                button1.transform.GetChild(0).GetComponent<Text>().text = "Next";
            }

            if (scene_number != 1)
            {
                button2 = (GameObject)Instantiate(buttonPrefab, new Vector3(250, 200, 0), Quaternion.identity);
                button2.transform.SetParent(panelToAttachButtonsTo.transform);
                button2.GetComponent<Button>().onClick.AddListener(previous_scene);
                button2.transform.GetChild(0).GetComponent<Text>().text = "Previous";
            }

        //Audio source section below
        if(scene_number == 1)
            {
                audio_source = clip1;
                audio_button = (GameObject)Instantiate(PrefabAudioPlay, new Vector3(100, 1500, 0), Quaternion.identity);
                audio_button.transform.SetParent(panelToAttachButtonsTo.transform);
                audio_button.GetComponent<Button>().onClick.AddListener(play_audio);
                audio_button.transform.GetChild(0).GetComponent<Text>().text = "Play";
            }
            if (scene_number == 3)
            {
                audio_source = clip3;
                audio_button = (GameObject)Instantiate(PrefabAudioPlay, new Vector3(100, 1500, 0), Quaternion.identity);
                audio_button.transform.SetParent(panelToAttachButtonsTo.transform);
                audio_button.GetComponent<Button>().onClick.AddListener(play_audio);
                audio_button.transform.GetChild(0).GetComponent<Text>().text = "Play";
            }

        }
        catch (Exception e)
        {
            wtf("\nException: " + e);
            Debug.Log("\nException: " + e);
        }
        
    }

    public void play_audio()
    {
        audio_z.PlayOneShot(audio_source); 
    }

    public void next_scene()
    {
        scene_number = scene_number + 1;
        create_scene();
    }

    public void previous_scene()
    {
        scene_number = scene_number - 1;
        create_scene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void wtf(string msg)
    {
        try
        {
            StreamWriter fw = new StreamWriter(myfilePath, true);
            fw.Write(msg);
            fw.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Unable to write in file...."+ ex);
        }
    }
}

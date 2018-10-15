using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniSpeech.Event
{
    public class AliciaEvent : MonoBehaviour, ISpeechRecognizer
    {
        //[SerializeField] private UniSpeechSampleUI ui;

        private AudioSource ganbatterune;
        private AudioSource itterassyai;
        private AudioSource matatsukaretara;
        private AudioSource ohayo;
        private AudioSource okite;
        private AudioSource okiteyo;
        private AudioSource oyasumi;
        private AudioSource sugoine;
        private AudioSource tsuchidayo;

        private SkinnedMeshRenderer facemoof;
        private GameObject faceM;
        private float eye_blink = 0;

        private Animator anim;

        int tranLen;
        int tranLenStart;
        string key;
        private float timeleft;
        private int okiteCnt;
        private int okiteyoCnt;
        private int okiteLim;
        private int okiteyoLim;

        private Vector3 dir = Vector3.zero;
        private Vector3 dir_old = Vector3.zero;

        SlackNotifier slackNotifier = new SlackNotifier();

        private bool getUp;

        private bool notifiFlag;

        public AnimatorStateInfo stateInfo;

        void Start()
        {
            SpeechRecognizer.CallbackGameObjectName = gameObject.name;
            SpeechRecognizer.RequestRecognizerAuthorization();

            //音源割り当て
            AudioSource[] voices = gameObject.GetComponents<AudioSource>();
            ganbatterune = voices[0];
            itterassyai = voices[1];
            matatsukaretara = voices[2];
            ohayo = voices[3];
            okite = voices[4];
            okiteyo = voices[5];
            oyasumi = voices[6];
            sugoine = voices[7];
            tsuchidayo = voices[8];

            //アニメーションフラグ初期化
            anim = GetComponent<Animator>();
            anim.SetBool("ganbatterune", false);
            anim.SetBool("ohayo", false);
            anim.SetBool("okite", false);
            anim.SetBool("okiteyo", false);
            anim.SetBool("itterassyai", false);
            anim.SetBool("oyasumi", false);
            anim.SetBool("sugoine", false);
            anim.SetBool("tsuchidayo", false);

            //瞬き
            faceM = GameObject.Find("face");
            facemoof = faceM.GetComponent<SkinnedMeshRenderer>();
            eye_blink = 0;

            StartRecord();

            //slack初期化
            StartCoroutine(slackNotifier.GetNotification());
            slackNotifier.SetBool(false);
            timeleft = 5.0f;

            dir.x = 0;
            dir.y = 0;
            dir.z = 1;

            dir_old.x = 0;
            dir_old.y = 0;
            dir_old.z = 1;

            getUp = false;
            notifiFlag = false;

            //起きて
            okiteCnt = 0;
            okiteyoCnt = 0;
            okiteLim = 2;
            okiteyoLim = 4;
        }

        void Update()
        {
            
            timeleft -= Time.deltaTime;



            if (timeleft <= 0.0)
            {
                

                dir_old.x = dir.x;
                dir_old.y = dir.y;
                dir_old.z = dir.z;

                dir.x = Input.acceleration.x;
                dir.y = Input.acceleration.y;
                dir.z = Input.acceleration.z;

                if (dir.z - dir_old.z < -0.8 && dir.y - dir_old.y < -0.5)
                    getUp = true;

                


                Debug.Log("x: " + dir.x + ", y: " + dir.y + ", z: " + dir.z);

                StartCoroutine(slackNotifier.GetNotification());
                
                timeleft = 5.0f;


                if (notifiFlag) {
                    okiteCnt++;
                    okiteyoCnt++;
                }

                //slackNotifier.SetBool(true);
            }


            if (okiteyoCnt > okiteyoLim)
            {
                anim.SetBool("okiteyo", true);
                anim.SetBool("idle", false);
                okiteyo.PlayOneShot(okiteyo.clip);
                StopRecord();
            }
            else if(okiteCnt > okiteLim){
                anim.SetBool("okite", true);
                anim.SetBool("idle", false);
                okite.PlayOneShot(okite.clip);
                StopRecord();
            }



            //blink
            eye_blink = (eye_blink > 200) ? Random.Range(-7000, -3000) : eye_blink + 10;



            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            //Debug.Log(stateInfo.nameHash);
            if (anim.GetBool("idle") && stateInfo.nameHash == Animator.StringToHash("Layer1.idle"))
            {
                anim.SetBool("ganbatterune", false);
                anim.SetBool("ohayo", false);
                anim.SetBool("okite", false);
                anim.SetBool("okiteyo", false);
                anim.SetBool("itterassyai", false);
                anim.SetBool("oyasumi", false);
                anim.SetBool("sugoine", false);
                anim.SetBool("tsuchidayo", false);

            }

            if (stateInfo.nameHash != Animator.StringToHash("Layer1.idle"))
            {
                //Debug.Log("done");
                anim.SetBool("idle", true);
            }
            else {
                facemoof.SetBlendShapeWeight(24, eye_blink);
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    anim.SetBool("ganbatterune", true);
                    anim.SetBool("idle", false);
                    ganbatterune.PlayOneShot(ganbatterune.clip);
                    StopRecord();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    anim.SetBool("itterassyai", true);
                    anim.SetBool("idle", false);
                    itterassyai.PlayOneShot(itterassyai.clip);
                    StopRecord();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    anim.SetBool("ohayo", true);
                    anim.SetBool("idle", false);
                    ohayo.PlayOneShot(ohayo.clip);
                    StopRecord();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    anim.SetBool("oyasumi", true);
                    anim.SetBool("idle", false);

                    oyasumi.PlayOneShot(oyasumi.clip);
                    StopRecord();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    anim.SetBool("sugoine", true);
                    anim.SetBool("idle", false);
                    itterassyai.PlayOneShot(sugoine.clip);
                    StopRecord();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    anim.SetBool("okite", true);
                    anim.SetBool("idle", false);
                    okite.PlayOneShot(okite.clip);
                    StopRecord();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    anim.SetBool("okiteyo", true);
                    anim.SetBool("idle", false);
                    okiteyo.PlayOneShot(okiteyo.clip);
                    StopRecord();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    anim.SetBool("tsuchidayo", true);
                    anim.SetBool("idle", false);
                    tsuchidayo.PlayOneShot(tsuchidayo.clip);
                    StopRecord();
                }

                if (slackNotifier.NewNotification)
                {
                    slackNotifier.SetBool(false);
                    anim.SetBool("tsuchidayo", true);
                    anim.SetBool("idle", false);
                    tsuchidayo.PlayOneShot(tsuchidayo.clip);
                    StopRecord();
                    notifiFlag = true;
                    okiteCnt = 0;
                    okiteyoCnt = 0;
                }

                if (getUp) {
                    anim.SetBool("itterassyai", true);
                    anim.SetBool("idle", false);
                    itterassyai.PlayOneShot(itterassyai.clip);
                    StopRecord();
                    getUp = false;
                }

                if (okiteyoCnt > okiteyoLim)
                {
                    anim.SetBool("okiteyo", true);
                    anim.SetBool("idle", false);
                    okiteyo.PlayOneShot(okiteyo.clip);
                    StopRecord();
                    okiteyoCnt = 0;
                }
                else if (okiteCnt > okiteLim)
                {
                    anim.SetBool("okite", true);
                    anim.SetBool("idle", false);
                    okite.PlayOneShot(okite.clip);
                    StopRecord();
                    okiteCnt = 0;
                }

            }

            /*
            if (Input.GetKeyDown(KeyCode.Alpha0))
                ganbatterune.PlayOneShot(ganbatterune.clip);
            if (Input.GetKeyDown(KeyCode.Alpha1))
                itterassyai.PlayOneShot(itterassyai.clip);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                matatsukaretara.PlayOneShot(matatsukaretara.clip);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                ohayo.PlayOneShot(ohayo.clip);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                okite.PlayOneShot(okite.clip);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                okiteyo.PlayOneShot(okiteyo.clip);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                oyasumi.PlayOneShot(oyasumi.clip);
            if (Input.GetKeyDown(KeyCode.Alpha7))
                sugoine.PlayOneShot(sugoine.clip);
            if (Input.GetKeyDown(KeyCode.Alpha8))
                tsuchidayo.PlayOneShot(tsuchidayo.clip);
            */


            //Debug.Log("x: "+dir.x+", y: "+dir.y+", z: "+dir.z);
        }


        public void OnRecognized(string transcription)
        {

            tranLen = transcription.Length;

            



            if ( anim.GetBool("idle") && stateInfo.nameHash == Animator.StringToHash("Layer1.idle")) {
                if (tranLen < 2) return;

                tranLenStart = tranLen - 2;
                key = transcription.Substring(tranLenStart, 2);
                if (key == "辛い")
                {
                    anim.SetBool("idle", false);
                    anim.SetBool("ganbatterune", true);
                    ganbatterune.PlayOneShot(ganbatterune.clip);
                    StopRecord();
                }
                else if (key == "研究") {
                    anim.SetBool("idle", false);
                    anim.SetBool("sugoine", true);
                    itterassyai.PlayOneShot(sugoine.clip);
                    StopRecord();
                }

                if (tranLen < 3) return;
                tranLenStart = tranLen - 3;
                key = transcription.Substring(tranLenStart, 3);

                if (key == "つらい")
                {
                    //Debug.Log("OnRecognized: " + transcription);
                    anim.SetBool("idle", false);
                    anim.SetBool("ganbatterune", true);
                    ganbatterune.PlayOneShot(ganbatterune.clip);
                    StopRecord();
                }

                if (tranLen < 4) return;
                tranLenStart = tranLen - 4;
                key = transcription.Substring(tranLenStart, 4);

                if (key == "おはよう")
                {
                    //Debug.Log("OnRecognized: " + transcription);
                    anim.SetBool("idle", false);
                    anim.SetBool("ohayo", true);
                    ohayo.PlayOneShot(ohayo.clip);
                    StopRecord();
                }
                else if (key == "おやすみ")
                {
                    //Debug.Log("OnRecognized: " + transcription);
                    anim.SetBool("idle", false);
                    anim.SetBool("oyasumi", true);
                    oyasumi.PlayOneShot(oyasumi.clip);
                    StopRecord();
                }

                if (tranLen < 5) return;
                tranLenStart = tranLen - 5;
                key = transcription.Substring(tranLenStart, 5);
                if (transcription == "けんきゅう")
                {
                    //Debug.Log("OnRecognized: " + transcription);
                    anim.SetBool("idle", false);
                    anim.SetBool("sugoine", true);
                    itterassyai.PlayOneShot(sugoine.clip);
                    StopRecord();
                }

                if (tranLen < 6) return;
                tranLenStart = tranLen - 6;
                key = transcription.Substring(tranLenStart, 6);
        
                if (transcription == "いってきます")
                {
                    //Debug.Log("OnRecognized: " + transcription);
                    anim.SetBool("idle", false);
                    anim.SetBool("itterassyai", true);
                    itterassyai.PlayOneShot(itterassyai.clip);
                    StopRecord();
                }
                /*
                else if (tranLen > 2)
                {
                    tranLenStart = tranLen - 3;
                    key = transcription.Substring(tranLenStart, 3);
                    //Debug.Log("Subtract: " + key);
                    if (key == "ハロー")
                    {
                        //anim.SetBool("nod", false);

                        StopRecord();
                    }
                }
                else
                {
                    Debug.Log("OnRecognized: " + transcription);
                    //ui.UpdateText(transcription);

                }
                */
                Debug.Log("OnRecognized: " + transcription);
             }
        }

        public void OnError(string description)
        {
            //Debug.Log("OnError: " + description);
            
            StartRecord();
        }

        public void OnAuthorized()
        {
            Debug.Log("OnAuthorized");
        }

        public void OnUnauthorized()
        {
            Debug.Log("OnUnauthorized");
        }

        public void OnAvailable()
        {
            Debug.Log("OnAvailable");
        }

        public void OnUnavailable()
        {
            Debug.Log("OnUnavailable");
        }

        private void StartRecord()
        {
    
            if (SpeechRecognizer.StartRecord())
            {
                Debug.Log("StartRecord");
            }
        }

        private void StopRecord()
        {
            if (SpeechRecognizer.StopRecord())
            {
                Debug.Log("StopRecord");
            }
        }
    }
}
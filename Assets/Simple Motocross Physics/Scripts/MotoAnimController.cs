using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using SMPScripts;
using Unity.Netcode;

    public class MotoAnimController : NetworkBehaviour
{
        MotoController motoController;
        Animator anim;
        string clipInfoCurrent, clipInfoLast;
        [HideInInspector]
        public float speed;
        [HideInInspector]
        public bool isAirborne;

        public GameObject hipIK, chestIK, leftFootIK, leftFootIdleIK, headIK;
        MotoStatus motoStatus;
        Rig rig;
        bool onOffBike;
        [Header("Character Switching")]
        [Space]
        public GameObject characterGeometry;
        public GameObject externalCharacter;
        float waitTime;
        bool isKikPerformed=false;
        void Start()
        {
            motoController = FindObjectOfType<MotoController>();
            motoStatus = FindObjectOfType<MotoStatus>();
            rig = hipIK.transform.parent.gameObject.GetComponent<Rig>();
            if (motoStatus != null)
                onOffBike = motoStatus.onBike;
            if (characterGeometry != null)
                characterGeometry.SetActive(motoStatus.onBike);
            if (externalCharacter != null)
                externalCharacter.SetActive(!motoStatus.onBike);
            anim = GetComponent<Animator>();
            leftFootIK.GetComponent<TwoBoneIKConstraint>().weight = 0;
        }

        void Update()
        {
        if (!IsOwner) return;
        if (characterGeometry != null && externalCharacter != null)
            {
                if (Input.GetKeyDown(KeyCode.Return) && motoController.transform.InverseTransformDirection(motoController.rb.velocity).z <= 0.1f && waitTime == 0)
                {
                    waitTime = 1.5f;
                    externalCharacter.transform.position = characterGeometry.transform.root.position - transform.right * 0.5f + transform.forward * 0.1f;
                    motoStatus.onBike = !motoStatus.onBike;
                    if (motoStatus.onBike)
                    {
                        anim.Play("OnBike");
                        StartCoroutine(AdjustRigWeight(0));
                    }
                    else
                    {
                        anim.Play("OffBike");
                        StartCoroutine(AdjustRigWeight(1));
                    }
                }
            }
            waitTime -= Time.deltaTime;
            waitTime = Mathf.Clamp(waitTime, 0, 1.5f);

            speed = motoController.transform.InverseTransformDirection(motoController.rb.velocity).z;
            isAirborne = motoController.isAirborne;
            anim.SetFloat("Speed", speed);
            anim.SetBool("isAirborne", isAirborne);
            if (motoStatus != null)
            {
                if (motoStatus.dislodged == false)
                {
                    if (!motoController.isAirborne && motoStatus.onBike)
                    {
                        clipInfoCurrent = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        if (clipInfoCurrent == "MotoIdleToStart" && clipInfoLast == "MotoIdle")
                            StartCoroutine(LeftFootIK(0));
                        if (clipInfoCurrent == "MotoIdle" && clipInfoLast == "MotoIdleToStart")
                            StartCoroutine(LeftFootIK(1));
                        if (clipInfoCurrent == "MotoIdle" && clipInfoLast == "MotoReverse")
                            StartCoroutine(LeftFootIdleIK(0));
                        if (clipInfoCurrent == "MotoReverse" && clipInfoLast == "MotoIdle")
                            StartCoroutine(LeftFootIdleIK(1));

                        clipInfoLast = clipInfoCurrent;
                    }
                }
                else
                {
                    characterGeometry.SetActive(false);
                    if(Input.GetKeyDown(KeyCode.R))
                    {
                        characterGeometry.SetActive(true);
                        motoStatus.dislodged = false;
                    }
                }
            }
            else
            {
                if (!motoController.isAirborne)
                {
                    clipInfoCurrent = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                    if (clipInfoCurrent == "MotoIdleToStart" && clipInfoLast == "MotoIdle")
                        StartCoroutine(LeftFootIK(0));
                    if (clipInfoCurrent == "MotoIdle" && clipInfoLast == "MotoIdleToStart")
                        StartCoroutine(LeftFootIK(1));
                    if (clipInfoCurrent == "MotoIdle" && clipInfoLast == "MotoReverse")
                        StartCoroutine(LeftFootIdleIK(0));
                    if (clipInfoCurrent == "MotoReverse" && clipInfoLast == "MotoIdle")
                        StartCoroutine(LeftFootIdleIK(1));

                    clipInfoLast = clipInfoCurrent;
                }
            }
        }

        IEnumerator LeftFootIK(int offset)
        {
            float t1 = 0f;
            while (t1 <= 1f)
            {
                t1 += Time.deltaTime*3;
                leftFootIK.GetComponent<TwoBoneIKConstraint>().weight = Mathf.Lerp(-0.05f, 1.05f, Mathf.Abs(offset - t1));
                leftFootIdleIK.GetComponent<TwoBoneIKConstraint>().weight = 1 - leftFootIK.GetComponent<TwoBoneIKConstraint>().weight;
                yield return null;
            }

        }

        public void PlayKickAnimation()
        {

            isKikPerformed = !isKikPerformed;
        if (isKikPerformed)
        {
            anim.SetBool("Kick", true);
        }
        else
        {
            anim.SetBool("Kick", false);
        }
           // StartCoroutine(ResetKickParameter());
        }

        private IEnumerator ResetKickParameter()
        {
            // Wait for the length of the kick animation
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

            // Reset Kick to false to transition back to Idle
            anim.SetBool("Kick", false);
        }
        IEnumerator LeftFootIdleIK(int offset)
        {
            float t1 = 0f;
            while (t1 <= 1f)
            {
                t1 += Time.deltaTime*3;
                leftFootIdleIK.GetComponent<TwoBoneIKConstraint>().weight = Mathf.Lerp(-0.05f, 1.05f, Mathf.Abs(offset - t1));
                yield return null;
            }

        }
        IEnumerator AdjustRigWeight(int offset)
        {
            StartCoroutine(LeftFootIK(1));
            if (offset == 0)
            {
                characterGeometry.SetActive(true);
                externalCharacter.SetActive(false);
            }
            float t1 = 0f;
            while (t1 <= 1f)
            {
                t1 += Time.deltaTime*3;
                rig.weight = Mathf.Lerp(-0.05f, 1.05f, Mathf.Abs(offset - t1));
                yield return null;
            }
            if (offset == 1)
            {
                yield return new WaitForSeconds(0.2f);
                characterGeometry.SetActive(false);
                externalCharacter.SetActive(true);
                // Matching position and rotation to the best possible transform to get a seamless transition
                externalCharacter.transform.position = characterGeometry.transform.root.position - transform.right * 0.5f + transform.forward * 0.1f;
                externalCharacter.transform.rotation = Quaternion.Euler(externalCharacter.transform.rotation.eulerAngles.x, characterGeometry.transform.root.rotation.eulerAngles.y + 80, externalCharacter.transform.rotation.eulerAngles.z);
            }

        }
    }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SMPScripts
{
    public class MotoStatus : MonoBehaviour
    {
        public bool onBike = true;
        public bool dislodged;
        public float impactThreshold;
        public GameObject ragdollPrefab;
        [HideInInspector]
        public GameObject instantiatedRagdoll;
        bool prevOnBike, prevDislodged;
        public GameObject inactiveColliders;
        MotoController motoController;
        Rigidbody rb;
        void Start()
        {
            motoController = GetComponent<MotoController>();
            rb = GetComponent<Rigidbody>();
            if (onBike)
                StartCoroutine(BikeStand(1));
            else
                StartCoroutine(BikeStand(0));

        }
        void OnCollisionEnter(Collision collision)
        {
            //Detects if there is a ragdoll to instantiate in the first place along with collsion impact detection
            if (collision.relativeVelocity.magnitude > impactThreshold && ragdollPrefab != null)
                dislodged = true;
        }
        void Update()
        {
            if (dislodged != prevDislodged)
            {
                if (dislodged)
                {
                    if (inactiveColliders != null)
                    {
                        motoController.motoGeometry.fPhysicsWheel.GetComponent<SphereCollider>().enabled = false;
                        motoController.motoGeometry.rPhysicsWheel.GetComponent<SphereCollider>().enabled = false;
                        inactiveColliders.SetActive(true);
                        motoController.enabled = false;
                    }
                    else
                        StartCoroutine(MotocontrollerToggle(false));

                    motoController.rb.centerOfMass = motoController.GetComponent<BoxCollider>().center;
                    ragdollPrefab.transform.localScale = new Vector3(1.16f,1.16f,1.16f);
                    instantiatedRagdoll = Instantiate(ragdollPrefab);
                }
                else
                {
                    if (inactiveColliders != null)
                    {
                        motoController.motoGeometry.fPhysicsWheel.GetComponent<SphereCollider>().enabled = true;
                        motoController.motoGeometry.rPhysicsWheel.GetComponent<SphereCollider>().enabled = true;
                        inactiveColliders.SetActive(false);
                        motoController.enabled = true;
                    }
                    else
                        StartCoroutine(MotocontrollerToggle(true));
                    motoController.rb.centerOfMass = motoController.centerOfMassOffset;
                    Destroy(instantiatedRagdoll);
                }
            }
            prevDislodged = dislodged;
            if (onBike != prevOnBike)
            {
                if (onBike && dislodged == false)
                    StartCoroutine(BikeStand(1));
                else
                    StartCoroutine(BikeStand(0));
            }
            prevOnBike = onBike;


        }
        IEnumerator MotocontrollerToggle(bool toggle)
        {
            yield return new WaitForSeconds(0.25f);
            motoController.enabled = toggle;
        }

        IEnumerator BikeStand(int instruction)
        {

            if (instruction == 1)
            {

                float t = 0f;
                while (t <= 1)
                {
                    t += Time.deltaTime * 5;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0), t);
                    yield return null;
                }
                motoController.enabled = true;
                rb.constraints = RigidbodyConstraints.None;


            }

            if (instruction == 0)
            {

                float t = 0f;
                while (t <= 1)
                {
                    t += Time.deltaTime * 5;
                    motoController.customSteerAxis = -Mathf.Abs(instruction - t);
                    yield return null;
                }
                motoController.enabled = false;
                yield return new WaitForSeconds(1);
                float l = 0f;
                while (l <= 1)
                {
                    l += Time.deltaTime * 5;
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, l * 5);
                    yield return null;
                }
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 5);
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

        }
    }
}

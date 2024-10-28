using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
namespace SMPScripts
{
    [System.Serializable]
    public class NoiseProperties
    {
        public bool useNoise;
        public float noiseRange;
        [Range(1, 100)]
        public float speedScale;
    }

    public class MotoProceduralIKHandler : MonoBehaviour
    {
        MotoController motoController;
        MotoAnimController motoAnimController;
        GameObject hipIKTarget, chestIKTarget, headIKTarget, leftFootIKTarget, rightFootIKTarget, rightHandIKTarget;
        public Vector2 chestIKRange, hipIKRange, headIKRange;
        TwoBoneIKConstraint chestRange;
        MultiParentConstraint hipRange;
        MultiAimConstraint headRange;
        Vector3 hipOffset, chestOffset, headOffset, leftFootInitialPos, rightFootInitialPos, rightHandInitialPos;
        Quaternion leftFootInitialRot, rightFootInitialRot, rightHandInitialRot;
        [Header("Movement Dynamics")]
        public NoiseProperties noiseProperties;
        float perlinNoise, animatedNoise, snapTime, randomTime;
        int returnToOrg;
        float initialChestRotationX, initialHipRotationX;
        Vector3 stuntModeHead;
        Vector3 impact, impactDirection;
        Vector3 velocity = Vector3.zero;
        float smoothGearRatio;
        float turnAngleX, stuntModeBody, wheelieFactor;

        void Start()
        {
            motoController = transform.root.GetComponent<MotoController>();
            motoAnimController = transform.GetComponent<MotoAnimController>();
            hipIKTarget = motoAnimController.hipIK.GetComponent<MultiParentConstraint>().data.sourceObjects[0].transform.gameObject;
            chestIKTarget = motoAnimController.chestIK.GetComponent<TwoBoneIKConstraint>().data.target.gameObject;
            headIKTarget = motoAnimController.headIK.GetComponent<MultiAimConstraint>().data.sourceObjects[0].transform.gameObject;
            leftFootIKTarget = motoAnimController.leftFootIK.GetComponent<TwoBoneIKConstraint>().data.target.gameObject;
            rightFootIKTarget = transform.Find("Rig 1/RightFootIK").GetComponent<TwoBoneIKConstraint>().data.target.gameObject;
            rightHandIKTarget = transform.Find("Rig 1/RightHandIK").GetComponent<TwoBoneIKConstraint>().data.target.gameObject;
            chestRange = motoAnimController.chestIK.GetComponent<TwoBoneIKConstraint>();
            hipRange = motoAnimController.hipIK.GetComponent<MultiParentConstraint>();
            headRange = motoAnimController.headIK.GetComponent<MultiAimConstraint>();

            hipOffset = hipIKTarget.transform.localPosition;
            chestOffset = chestIKTarget.transform.localPosition;
            headOffset = headIKTarget.transform.localPosition;
            leftFootInitialPos = leftFootIKTarget.transform.localPosition;
            rightFootInitialPos = rightFootIKTarget.transform.localPosition;
            rightHandInitialPos = rightHandIKTarget.transform.localPosition;

            leftFootInitialRot = leftFootIKTarget.transform.localRotation;
            rightFootInitialRot = rightFootIKTarget.transform.localRotation;
            rightHandInitialRot = rightHandIKTarget.transform.localRotation;

            initialChestRotationX = chestIKTarget.transform.eulerAngles.x;
            initialHipRotationX = hipIKTarget.transform.eulerAngles.x;

        }

        // Update is called once per frame
        void Update()
        {
            //Weights
            chestRange.weight = Mathf.Clamp(motoController.pickUpSpeed, chestIKRange.x, chestIKRange.y);
            hipRange.weight = Mathf.Clamp(motoController.pickUpSpeed, hipIKRange.x, hipIKRange.y);
            headRange.weight = Mathf.Clamp(motoAnimController.speed, headIKRange.x, headIKRange.y);

            //Noise
            if (noiseProperties.useNoise)
            {
                animatedNoise = Mathf.Lerp(animatedNoise, perlinNoise, Time.deltaTime * 5);
                snapTime += Time.deltaTime;
                if (snapTime > randomTime)
                {
                    randomTime = Random.Range(1 / noiseProperties.speedScale, 1.1f);
                    returnToOrg++;
                    if (returnToOrg % 2 == 0)
                        perlinNoise = noiseProperties.noiseRange * Mathf.PerlinNoise(Time.time * 10, 0) - (0.5f * noiseProperties.noiseRange);
                    else
                        perlinNoise = 0;
                    snapTime = 0;
                }
            }

            turnAngleX = motoController.transform.rotation.eulerAngles.x;
            if (turnAngleX > 180)
                turnAngleX = motoController.transform.eulerAngles.x - 360;
            
            stuntModeBody = Mathf.Lerp(stuntModeBody,System.Convert.ToInt32(motoController.isAirborne), Time.deltaTime*10);

            impact = Vector3.SmoothDamp(impact, -motoController.deceleration * 0.1f, ref velocity, 0.25f);
            impactDirection = transform.InverseTransformDirection(impact);

            //Chest IK
            chestIKTarget.transform.localPosition = new Vector3(motoController.customLeanAxis * -0.1f, wheelieFactor, 0) + chestOffset + impactDirection * 0.1f;
            chestIKTarget.transform.rotation = Quaternion.Lerp(Quaternion.Euler(initialChestRotationX + turnAngleX, motoController.transform.rotation.eulerAngles.y - motoController.customLeanAxis * 0.5f, motoController.customLeanAxis * 0.5f), chestIKTarget.transform.rotation,stuntModeBody);
            //Hip IK
            hipIKTarget.transform.localPosition = new Vector3(motoController.customLeanAxis * 0.1f, 0, 0) + hipOffset + impactDirection * 0.1f;
            hipIKTarget.transform.rotation = Quaternion.Lerp(Quaternion.Euler(initialHipRotationX + turnAngleX, motoController.transform.rotation.eulerAngles.y, motoController.customLeanAxis * 0.5f),hipIKTarget.transform.rotation,stuntModeBody);

            // //Head Target Position
            // stuntModeHead = Vector3.Lerp(stuntModeHead, Vector3.zero, Time.deltaTime * 10);
            // headIKTarget.transform.localPosition = new Vector3(motoController.customLeanAxis * 1.5f + stuntModeHead.x, 0, 0 + stuntModeHead.z) + headOffset + impactDirection;

            //Head Target Position
            if(motoController.isAirborne && motoController.airTimeSettings.freestyle)
                stuntModeHead = Vector3.Lerp(stuntModeHead, transform.InverseTransformDirection(motoController.rb.velocity),Time.deltaTime*2);
            else
                stuntModeHead = Vector3.Lerp(stuntModeHead,Vector3.zero,Time.deltaTime*2);
            wheelieFactor += motoController.wheelieInput&&motoController.wheeliePower>100?Time.deltaTime*2:-Time.deltaTime;
            wheelieFactor = Mathf.Clamp01(wheelieFactor);
            headIKTarget.transform.localPosition = new Vector3(motoController.customLeanAxis * 1.5f + stuntModeHead.x, 0, stuntModeHead.z - wheelieFactor*3) + headOffset + impactDirection;

            // //Head Target Position
            // if(motoController.isAirborne && motoController.airTimeSettings.freestyle)
            // stuntModeHead = Vector3.Lerp(stuntModeHead, transform.InverseTransformDirection(motoController.rb.velocity),Time.deltaTime*2);
            // else
            // stuntModeHead = Vector3.Lerp(stuntModeHead,Vector3.zero,Time.deltaTime*2);
            // headIKTarget.transform.localPosition = new Vector3(motoController.customLeanAxis * 1.5f + animatedNoise*motoController.pickUpSpeed + stuntModeHead.x, 1-(motoController.pickUpSpeed*1.5f)+animatedNoise,animatedNoise*3 + stuntModeHead.z) + headOffset + impactDirection;

            //Left Foot IK
            if (motoController.turnLeanAmount > 20 && !motoController.isAirborne)
            {
                leftFootIKTarget.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(-0.3f, 0, 0.3f) + impactDirection * 0.1f, ((Mathf.Abs(motoController.turnLeanAmount) - 20) * Mathf.Abs(motoController.customAccelerationAxis - 1)) * 0.5f) + leftFootInitialPos;
                leftFootIKTarget.transform.localRotation = Quaternion.Lerp(leftFootInitialRot, Quaternion.Euler(-113, -117, 289), ((Mathf.Abs(motoController.turnLeanAmount) - 20) * Mathf.Abs(motoController.customAccelerationAxis - 1)) * 0.5f);
            }

            else
            {
                leftFootIKTarget.transform.localPosition = Vector3.Lerp(leftFootIKTarget.transform.localPosition, leftFootInitialPos, Time.deltaTime * 10);
                leftFootIKTarget.transform.localRotation = Quaternion.Lerp(leftFootIKTarget.transform.localRotation, leftFootInitialRot, Time.deltaTime * 10);
            }

            //Right Foot IK
            if (motoController.turnLeanAmount < -20 && !motoController.isAirborne)
            {
                rightFootIKTarget.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0.3f, 0, 0.3f) + impactDirection * 0.1f, ((Mathf.Abs(motoController.turnLeanAmount) - 20) * Mathf.Abs(motoController.customAccelerationAxis - 1)) * 0.5f) + rightFootInitialPos;
                rightFootIKTarget.transform.localRotation = Quaternion.Lerp(rightFootInitialRot, Quaternion.Euler(-113, 117, -289), ((Mathf.Abs(motoController.turnLeanAmount) - 20) * Mathf.Abs(motoController.customAccelerationAxis - 1)) * 0.5f);

            }

            else
            {
                rightFootIKTarget.transform.localPosition = Vector3.Lerp(rightFootIKTarget.transform.localPosition, rightFootInitialPos, Time.deltaTime * 10);
                rightFootIKTarget.transform.localRotation = Quaternion.Lerp(rightFootIKTarget.transform.localRotation, rightFootInitialRot, Time.deltaTime * 10);
            }

            //Accelerator hand movement
            smoothGearRatio = Mathf.Lerp(smoothGearRatio, motoController.engineSettings.gearRatio, Time.deltaTime * 5);
            rightHandIKTarget.transform.localPosition = Vector3.Lerp(rightHandInitialPos, new Vector3(rightHandInitialPos.x, rightHandInitialPos.y - 0.075f, rightHandInitialPos.z), smoothGearRatio);
            rightHandIKTarget.transform.localRotation = Quaternion.Lerp(rightHandInitialRot, Quaternion.Euler(74.7f, 34.7f, 19.5f), smoothGearRatio);


        }
    }
}
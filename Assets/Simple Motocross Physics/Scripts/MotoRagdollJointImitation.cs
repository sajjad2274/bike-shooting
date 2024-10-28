using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMPScripts{
public class MotoRagdollJointImitation : MonoBehaviour
{

    private string limbTag = "TrackedLimb";
    GameObject CharacterCopyPosition;
    GameObject CharacterCopyAnimations;
    MotoController motoController;

    private Transform animRoot;
    public int bodyPartMass = 1;

    public float jointAngularDamping = 100f;
    public float Stiffness = 1000f;
    
    public float launchVelocityMultiplier = 1;

    public Transform[] allAnimTrans;
    public ConfigurableJoint[] confJoints;


    void Start()
    {
        motoController = FindObjectOfType<MotoController>();
        CharacterCopyPosition = motoController.GetComponentInChildren<MotoAnimController>(true).gameObject;
        CharacterCopyAnimations = GameObject.FindWithTag("AnimationToFollow");
        transform.position = new Vector3(CharacterCopyPosition.transform.position.x,motoController.transform.position.y,CharacterCopyPosition.transform.position.z);
        transform.rotation = CharacterCopyPosition.transform.rotation;
        PopulateArrays();
        AddJointFollowScript();
    }

    public void CopyInitialPositions()
    {
        foreach (Transform trans in transform.GetComponentsInChildren<Transform>())
        {
            if (trans.GetComponent<ConfigurableJoint>() != null || trans.name == "mixamorig:Hips")
            {
                foreach (Transform rider in CharacterCopyPosition.transform.GetComponentsInChildren<Transform>())
                {
                    if (trans.name == rider.name)
                        trans.localRotation = rider.localRotation;
                }
            }
        }
        foreach (Transform trans in transform.GetComponentsInChildren<Transform>())
        {
            if (trans.GetComponent<ConfigurableJoint>() != null)
            {
                foreach (Transform animRider in CharacterCopyAnimations.transform.GetComponentsInChildren<Transform>())
                {
                    if (trans.name == animRider.name)
                        animRider.localRotation = trans.localRotation;
                }
            }
        }


    }

    private void PopulateArrays()
    {
        animRoot = CharacterCopyAnimations.transform.Find("mixamorig:Hips");
        Transform[] animTransArr = animRoot.GetComponentsInChildren<Transform>();
        Transform[] ragTransArr = transform.GetComponentsInChildren<Transform>();
        List<Transform> transList = new List<Transform>();
        List<ConfigurableJoint> jointList = new List<ConfigurableJoint>();

        foreach (Transform trans in animTransArr)
        {
            if (trans.tag == limbTag)
            {
                transList.Add(trans);
            }
        }
        allAnimTrans = transList.ToArray();

        foreach (Transform trans in ragTransArr)
        {
            ConfigurableJoint cj = trans.GetComponent<ConfigurableJoint>();
            if (cj != null)
            {
                //default contact to 0.1, max depenetration to 0.1 Fixed TimeScale to  0.01
                jointList.Add(cj);
                cj.projectionMode = JointProjectionMode.PositionAndRotation;
                cj.projectionDistance = 5f;
                cj.projectionAngle = 5f;
                cj.enablePreprocessing = false;
                trans.GetComponent<Rigidbody>().solverIterations = 4;
                trans.GetComponent<Rigidbody>().mass = bodyPartMass;
                trans.GetComponent<Rigidbody>().velocity = motoController.GetComponent<Rigidbody>().velocity * launchVelocityMultiplier;
                trans.GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Infinity;
            }
        }
        confJoints = jointList.ToArray();
    }

    private void AddJointFollowScript()
    {
        foreach (ConfigurableJoint cj in confJoints)
        {
            cj.gameObject.AddComponent<MotoRagdollJointConfig>();
            //cj.connectedBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            for (int t = 0; t < allAnimTrans.Length; t++)
            {
                if (allAnimTrans[t].name == cj.gameObject.name)
                {
                    cj.GetComponent<MotoRagdollJointConfig>().torqueForce = Stiffness;
                    cj.GetComponent<MotoRagdollJointConfig>().angularDamping = jointAngularDamping;
                    cj.GetComponent<MotoRagdollJointConfig>().target = allAnimTrans[t];
                }
            }
        }
    }

}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMPScripts
{
    public class MotoTPSCamSwitch : MonoBehaviour
    {
        public GameObject target;
        public GameObject externalCharacter;
        MotoCamera motoCamera;
        MotoStatus motoStatus;
        void Start()
        {
            motoCamera = FindObjectOfType<MotoCamera>();
            motoStatus = FindObjectOfType<MotoStatus>();
        }
        void LateUpdate()
        {
            if (externalCharacter != null)
            {
                if (externalCharacter.activeInHierarchy)
                {
                    motoCamera.target = externalCharacter.transform;
                }
            }
            else if (motoStatus.dislodged && motoStatus.instantiatedRagdoll!=null)
            {
                motoCamera.target = motoStatus.instantiatedRagdoll.transform.Find("mixamorig:Hips").gameObject.transform;
            }
            else
            {
                motoCamera.target = target.transform.root.transform;
            }
        }
    }
}

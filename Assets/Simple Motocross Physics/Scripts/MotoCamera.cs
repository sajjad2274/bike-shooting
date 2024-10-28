using UnityEngine;
using System.Collections;
namespace SMPScripts
{
    public class MotoCamera : MonoBehaviour
    {
        public Transform target;
        public bool stuntCamera;
        public float distance = 20.0f;
        public float height = 5.0f;
        public float heightDamping = 2.0f;

        public float lookAtHeight = 0.0f;

        public float rotationSnapTime = 0.3F;


        private Vector3 lookAtVector;

        private float usedDistance;

        float wantedRotationAngle;
        float wantedHeight;

        float currentRotationAngle;
        float currentHeight;

        Vector3 wantedPosition;

        private float yVelocity = 0.0F;
        private float zVelocity = 0.0F;
        MotoPerfectMouseLook perfectMouseLook;



        void Start()
        {
            perfectMouseLook = GetComponent<MotoPerfectMouseLook>();
            if (stuntCamera)
            {
                var follow = new GameObject("Follow");
                var toFollow = GameObject.FindObjectOfType<MotoController>().transform;
                follow.transform.SetParent(toFollow);
                follow.transform.position = toFollow.position + toFollow.gameObject.GetComponent<BoxCollider>().center;
                target = follow.transform;

                height -= toFollow.gameObject.GetComponent<BoxCollider>().center.y;
                lookAtHeight -= toFollow.gameObject.GetComponent<BoxCollider>().center.y;
            }
            else
                target = GameObject.FindObjectOfType<MotoController>().transform;
        }

        void LateUpdate()
        {
            if (target != null)
            {
                wantedHeight = target.position.y + height;
                currentHeight = transform.position.y;

                wantedRotationAngle = target.eulerAngles.y;
                currentRotationAngle = transform.eulerAngles.y;
                if (perfectMouseLook.movement == false)
                    currentRotationAngle = Mathf.SmoothDampAngle(currentRotationAngle, wantedRotationAngle, ref yVelocity, rotationSnapTime);

                currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.fixedDeltaTime);

                wantedPosition = target.position;
                wantedPosition.y = currentHeight;

                usedDistance = Mathf.SmoothDampAngle(usedDistance, distance, ref zVelocity, 0.1f);

                wantedPosition += Quaternion.Euler(0, currentRotationAngle, 0) * new Vector3(0, 0, -usedDistance);

                transform.position = wantedPosition;

                transform.LookAt(target.position + lookAtVector);

                lookAtVector = new Vector3(0, lookAtHeight, 0);
            }


        }

    }
}
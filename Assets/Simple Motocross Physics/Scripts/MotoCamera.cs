
using UnityEngine;
using Unity.Netcode;
using SMPScripts;

public class MotoCamera : MonoBehaviour
{
    private Transform target;
    public bool stuntCamera;
    private float distance = 4.5f;
    private float height = 2.81f;
    private float heightDamping = 100f;
    private float lookAtHeight = 1.87f;
    private float rotationSnapTime = 0.5f;

    private Vector3 lookAtVector;
    private float usedDistance;
    float wantedRotationAngle;
    float wantedHeight;
    float currentRotationAngle;
    float currentHeight;
    Vector3 wantedPosition;
    private float yVelocity = 0.0F;
    private float zVelocity = 0.0F;
    private MotoPerfectMouseLook perfectMouseLook;

    private void Start()
    {
        perfectMouseLook = GetComponent<MotoPerfectMouseLook>();


        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {

        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            SetLocalPlayerCameraTarget();
        }
    }

    private void SetLocalPlayerCameraTarget()
    {

        MotoController[] players = GameObject.FindObjectsOfType<MotoController>();
        foreach (MotoController player in players)
        {
            if (MotoController.LocalInstance.IsLocalPlayer)
            {
                SetCameraTarget(player);
                break;
            }
        }
    }

    private void SetCameraTarget(MotoController player)
    {
        if (stuntCamera)
        {
            var follow = new GameObject("Follow");
            follow.transform.SetParent(player.transform);
            follow.transform.position = player.transform.position + player.gameObject.GetComponent<BoxCollider>().center;
            target = follow.transform;

            height -= player.gameObject.GetComponent<BoxCollider>().center.y;
            lookAtHeight -= player.gameObject.GetComponent<BoxCollider>().center.y;
        }
        else
        {

            target = player.transform;
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            wantedHeight = target.position.y + height;
            currentHeight = transform.position.y;

            wantedRotationAngle = target.eulerAngles.y;
            currentRotationAngle = transform.eulerAngles.y;
            if (perfectMouseLook != null && !perfectMouseLook.movement)
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

    private void OnDestroy()
    {

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}
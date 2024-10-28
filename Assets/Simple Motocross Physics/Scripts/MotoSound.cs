using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SMPScripts
{
    public class MotoSound : MonoBehaviour
    {
        AudioSource audioSource;
        AudioMixer mixer;
        MotoController motoController;
        public AnimationCurve engineRPM;
        float prevPitch;
        public float EngineFlow = 1;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponents<AudioSource>()[0];
            motoController = FindObjectOfType<MotoController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (motoController.rawCustomAccelerationAxis > 0 && !motoController.isAirborne)
            {
                audioSource.pitch = (engineRPM.Evaluate(motoController.engineSettings.gearRatio + (motoController.engineSettings.currentGear) * 0.1f) + 1 + motoController.rb.velocity.magnitude*0.05f);

                audioSource.pitch = Mathf.Lerp(prevPitch, audioSource.pitch, Time.deltaTime * EngineFlow);
                audioSource.volume = Mathf.Lerp(audioSource.volume, 1, Time.deltaTime * EngineFlow);
                prevPitch = audioSource.pitch;
            }
            else if(motoController.isAirborne)
            {
                audioSource.pitch = Mathf.Lerp(audioSource.pitch, 1.5f + motoController.engineSettings.currentGear*0.2f + engineRPM.Evaluate(1), Time.deltaTime * EngineFlow);
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0.5f, Time.deltaTime * EngineFlow);
            }
            else
            {
                audioSource.pitch = Mathf.Lerp(audioSource.pitch, 1 + motoController.engineSettings.currentGear*0.2f, Time.deltaTime * EngineFlow);
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0.5f, Time.deltaTime * EngineFlow);
            }
        }
    }
}

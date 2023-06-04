using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
namespace DefaultNamespace
{
    public class CutsceneAudioTrigger:MonoBehaviour
    {
        [SerializeField]
        public StudioEventEmitter emitter;

        public void triggerAudio()
        {
            emitter.Play();
        }
    }
}
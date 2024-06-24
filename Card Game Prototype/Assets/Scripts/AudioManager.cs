using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAudioManager {
    public class AudioManager : MonoBehaviour {
        [SerializeField] AudioClip[] audioFiles;
        [SerializeField][Range(0.1f, 0.9f)] float minPitchRange;
        [SerializeField][Range(0.5f, 1f)] float maxPitchRange;

        AudioSource aS;

        public static AudioManager Instance { get; private set; }

        void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Debug.Log($"Destroying extra {gameObject.name}");
                Destroy(gameObject);
            }
        }

        private void Start() {
            aS = GetComponentInChildren<AudioSource>();
        }

        public static void PlayDamageSound() {
            if (Instance == null) {
                Debug.Log("No instance of AudioManager found!");
                return;
            }

            float randomPitch = Random.Range(Instance.minPitchRange, Instance.maxPitchRange);
            Instance.aS.pitch = randomPitch;
            Instance.aS.PlayOneShot(Instance.audioFiles[0]);
        }
    }
}
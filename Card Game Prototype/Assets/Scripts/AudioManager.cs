using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomAudioManager {
    public class AudioManager : MonoBehaviour {
        [Header("Music")]
        [SerializeField] AudioSource mS;
        [SerializeField] Slider mSlider;
        public Slider MSlider {
            get { return mSlider; }
            set { mSlider = value; }
        }
        [SerializeField] TextMeshProUGUI mSliderValue;
        public TextMeshProUGUI MSliderValue {
            get { return mSliderValue; }
            set { mSliderValue = value; }
        }

        [Header("Sfx")]
        [SerializeField] AudioClip[] audioFiles;
        [SerializeField][Range(0.1f, 0.9f)] float minPitchRange;
        [SerializeField][Range(0.5f, 1f)] float maxPitchRange;
        [SerializeField] AudioSource aS_Sfx;
        [SerializeField] Slider sfxSlider;
        public Slider SfxSlider {
            get { return sfxSlider; }
            set { sfxSlider = value; }
        }
        [SerializeField] TextMeshProUGUI sfxSliderValue;
        public TextMeshProUGUI SfxSliderValue {
            get { return sfxSliderValue; }
            set { sfxSliderValue = value; }
        }

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

        public static void PlayDamageSound() {
            if (Instance == null) {
                Debug.Log("No instance of AudioManager found!");
                return;
            }

            float randomPitch = Random.Range(Instance.minPitchRange, Instance.maxPitchRange);
            Instance.aS_Sfx.pitch = randomPitch;
            Instance.aS_Sfx.PlayOneShot(Instance.audioFiles[0]);
        }

        public static void PlayBlockDamagedSound() {
            if (Instance == null) {
                Debug.Log("No instance of AudioManager found!");
                return;
            }

            float randomPitch = Random.Range(Instance.minPitchRange, Instance.maxPitchRange);
            Instance.aS_Sfx.pitch = randomPitch;
            Instance.aS_Sfx.PlayOneShot(Instance.audioFiles[1]);
        }

        public static void PlayBlockUpSound() {
            if (Instance == null) {
                Debug.Log("No instance of AudioManager found!");
                return;
            }

            float randomPitch = Random.Range(Instance.minPitchRange, Instance.maxPitchRange);
            Instance.aS_Sfx.pitch = randomPitch;
            Instance.aS_Sfx.PlayOneShot(Instance.audioFiles[2]);
        }

        public static void PlayBuffSound() {
            if (Instance == null) {
                Debug.Log("No instance of AudioManager found!");
                return;
            }

            float randomPitch = Random.Range(Instance.minPitchRange, Instance.maxPitchRange);
            Instance.aS_Sfx.pitch = randomPitch;
            Instance.aS_Sfx.PlayOneShot(Instance.audioFiles[3]);
        }

        public static void PlayStunSound() {
            if (Instance == null) {
                Debug.Log("No instance of AudioManager found!");
                return;
            }

            float randomPitch = Random.Range(Instance.minPitchRange, Instance.maxPitchRange);
            Instance.aS_Sfx.pitch = randomPitch;
            Instance.aS_Sfx.PlayOneShot(Instance.audioFiles[4]);
        }

        public static void PlayCardSound() {
            if (Instance == null) {
                Debug.Log("No instance of AudioManager found!");
                return;
            }

            float randomPitch = Random.Range(Instance.minPitchRange, Instance.maxPitchRange);
            Instance.aS_Sfx.pitch = randomPitch;
            Instance.aS_Sfx.PlayOneShot(Instance.audioFiles[5]);
        }

        public static void PlayGameOverSound() {
            if (Instance == null) {
                Debug.Log("No instance of AudioManager found!");
                return;
            }

            float randomPitch = Random.Range(Instance.minPitchRange, Instance.maxPitchRange);
            Instance.aS_Sfx.pitch = randomPitch;
            Instance.aS_Sfx.PlayOneShot(Instance.audioFiles[6]);
        }

        public static void PlayNotEnoughApSound() {
            if (Instance == null) {
                Debug.Log("No instance of AudioManager found!");
                return;
            }

            float randomPitch = Random.Range(Instance.minPitchRange, Instance.maxPitchRange);
            Instance.aS_Sfx.pitch = randomPitch;
            Instance.aS_Sfx.PlayOneShot(Instance.audioFiles[7]);
        }

        #region Volume slider values
        public void UpdateMusicVolume() {
            if (Instance == null) {
                return;
            }

            Instance.mS.volume = Instance.mSlider.value / 100;
            Instance.mSliderValue.text = $"{Mathf.Round(Instance.mSlider.value)}";
        }

        public void UpdateSFXVolume() {
            if (Instance == null) {
                return;
            }

            Instance.aS_Sfx.volume = Instance.sfxSlider.value / 100;
            Instance.sfxSliderValue.text = $"{Mathf.Round(Instance.sfxSlider.value)}";
        }
        #endregion
    }
}
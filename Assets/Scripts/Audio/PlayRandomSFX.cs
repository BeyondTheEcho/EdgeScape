using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Audio
{ 
    public class PlayRandomSFX : MonoBehaviour
    {
        [SerializeField] private AudioClip[] m_AudioClips;
        [SerializeField] [Range(0f, 1f)] private float m_Volume = 0.25f;
        [SerializeField] private bool m_Use3DSpatialSound = true;

        private AudioSource m_AudioSource;

        private const int c_FirstArrayIndex = 0;

        private const float c_MinPitch = 1.0f;
        private const float c_MaxPitch = 1.25f;

        private const float c_3DSpatialSound = 1.0f;
        private const float c_2DSpatialSound = 0.0f;

        private const float c_MaxSoundFalloff = 50.0f;

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            m_AudioSource.volume = m_Volume;

            m_AudioSource.rolloffMode = AudioRolloffMode.Linear;
            m_AudioSource.maxDistance = c_MaxSoundFalloff;

            if (m_Use3DSpatialSound)
            {
                m_AudioSource.spatialBlend = c_3DSpatialSound;
            }
            else
            {
                m_AudioSource.spatialBlend = c_2DSpatialSound;
            }
        }

        public void PlayRandomAudio()
        {
            if (m_AudioClips.Length == 0) return;

            m_AudioSource.pitch = Random.Range(c_MinPitch, c_MaxPitch);

            int randomClipIndex = Random.Range(c_FirstArrayIndex, m_AudioClips.Length);

            m_AudioSource.PlayOneShot(m_AudioClips[randomClipIndex]);

        }
    }
}
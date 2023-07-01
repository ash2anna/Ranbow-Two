using UnityEngine;

namespace Scripts.Weapon
{
    [CreateAssetMenu(menuName ="FirearmsAudio")]
    public class FirearmsAudio : ScriptableObject
    {
        public AudioClip ShootingAudio;
        public AudioClip ReloadAudio;
        public AudioClip ReloadAllAudio;
        public AudioClip SwitchAudio;
    }
}
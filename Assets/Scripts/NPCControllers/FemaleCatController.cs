using PlayerControllers;
using UnityEngine;

namespace NPCControllers
{
    public class FemaleCat : MonoBehaviour
    {
        [Header("Particle Effects")]
        public ParticleSystem heartParticlePrefab;
        public Vector3 heartOffset = new Vector3(0, 1f, 0);

        private Animator _animator;
        private bool _hasReacted = false;
        public GameObject heartIndicator; // Inspector'dan sürükle bırak
        public bool ReactActive { get; private set; }
    
        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            heartIndicator.SetActive(ReactActive);
        }

        // Player girince veya alandayken her frame taşıma durumunu kontrol et
        void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var pa = other.GetComponent<PlayerAttack>();
                bool carrying = (pa != null && pa.IsCarrying());
                _animator.SetBool("isInterested", true);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _animator.SetBool("isInterested", false);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // mouse yere düşüp alana girerse tepki ver
            if (!_hasReacted && other.CompareTag("Mouse") && other.transform.parent == null)
            {
                React();
            }
        }

        private void React()
        {
            _hasReacted = true;
            _animator.SetTrigger("react");
            ReactActive = true;
        }
    }
}
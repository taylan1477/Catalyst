using Save_Load_Options;
using UnityEngine;

namespace NPCControllers
{
    public class MouseController : MonoBehaviour
    {
        [SerializeField] Transform[] waypoints;
        Vector3[] _wpPositions;
        public float moveSpeed = 2f;
        public int health = 3;
    
        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] idleClips;
        [SerializeField] private float minHearDistance = 2f;
        [SerializeField] private float maxHearDistance = 20f;

        private int _currentWaypointIndex;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private bool _isDead;
        private Transform _player;

        void Awake()
        {
            // Başta waypoint Transform’lerinin World pozisyonlarını yakala
            _wpPositions = new Vector3[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++)
                _wpPositions[i] = waypoints[i].position;
        }
    
        void Start()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;

            ConfigureAudioSource();
            StartIdleSound();
        }

        void ConfigureAudioSource()
        {
            if(audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        
            // 3D Ses Ayarları
            audioSource.spatialBlend = 1f; // Tam 3D ses
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = minHearDistance;
            audioSource.maxDistance = maxHearDistance;
            audioSource.loop = true;
        }

        void Update()
        {
            if (!_isDead)
            {
                MoveAlongWaypoints();
                UpdateVolumeByDistance();
            }
        }

        void UpdateVolumeByDistance()
        {
            if(_isDead) return; // Ölü farelerde ses kontrolü yapma
    
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
    
            if(distanceToPlayer > maxHearDistance)
            {
                if(audioSource.isPlaying) 
                    audioSource.Pause();
            }
            else
            {
                if(!audioSource.isPlaying) 
                    audioSource.UnPause();
        
                audioSource.volume = Mathf.Clamp01(1 - (distanceToPlayer / maxHearDistance));
            }
        }

        void StartIdleSound()
        {
            if(_isDead) return; // Ölü farelerde ses başlatma
    
            if(idleClips.Length == 0) return;
            audioSource.clip = idleClips[Random.Range(0, idleClips.Length)];
            audioSource.Play();
        }

        void MoveAlongWaypoints()
        {
            if (_wpPositions.Length == 0) return;

            Vector3 targetPos = _wpPositions[_currentWaypointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // Yüzünü hedefe çevir
            _spriteRenderer.flipX = targetPos.x < transform.position.x;

            // Hedefe varınca sıradaki index
            if (Vector2.Distance(transform.position, targetPos) < 0.1f)
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _wpPositions.Length;

            _animator.SetBool(AnimatorHashes.Walk, true);
        }

        public void TakeDamage(int damage)
        {
            if (_isDead) return;

            health -= damage;
            AudioManager.Instance.PlayMouseHurt();

            if (health <= 0) Die();
            else _animator.SetTrigger(AnimatorHashes.Hurt);
        }

        void Die()
        {
            if(_isDead) return;
            _isDead = true;

            // SES KESME OPERASYONU
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
            audioSource.enabled = false; // AudioSource bileşenini devre dışı bırak
            Destroy(audioSource); // AudioSource'u tamamen yok et
        
            _animator.SetTrigger(AnimatorHashes.Dead);
            transform.localScale *= 0.7f;

            // TÜM KONTROLLERİ DURDUR
            this.enabled = false; // Bu script'i devre dışı bırak

            // DEBUG
            Debug.Log($"Fare öldü! Ses durumu: {audioSource.isPlaying}", gameObject);
        }
    
        public bool IsDead()
        {
            return _isDead;
        }
    }
}
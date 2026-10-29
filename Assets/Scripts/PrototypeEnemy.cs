using UnityEngine;

namespace StinyPopela
{
    public sealed class PrototypeEnemy : MonoBehaviour
    {
        public PrototypeActor target;
        public bool boss;
        public float health = 45;
        float nextHit;
        bool dead;
        void Start() { CombatFeedback.Track(this, health); }

        void Update()
        {
            if (dead || target == null || !target.Alive || Time.timeScale<=0) return;
            var delta = target.transform.position - transform.position;
            delta.y = 0;
            if (delta.magnitude > 6.5f) return;
            if (delta.magnitude > 1.5f)
            {
                transform.position += delta.normalized * (boss ? 1.6f : 2.3f) * Time.deltaTime;
                transform.forward = delta.normalized;
            }
            else if (Time.time >= nextHit)
            {
                nextHit = Time.time + (boss ? 1.5f : 1.1f);
                target.Hurt(boss ? 18 : 6);
                PrototypeWorld.Flash(transform.position + Vector3.up, 1.2f, Color.red);
            }
        }

        public void Hit(float damage)
        {
            if (dead) return;
            float actual = Mathf.Min(health, Mathf.Max(0, damage));
            health -= actual;
            CombatFeedback.Show(transform.position + Vector3.up * (boss ? 3 : 2), actual, false);
            if (health > 0) return;
            dead = true;
            int dropLevel = target.progress.level;
            target.progress.GainExperience(boss ? 160 : 25);
            PrototypeWorld.Instance.Drop(transform.position, dropLevel, boss);
            if (boss) PrototypeWorld.Instance.bossDefeated = true;
            Destroy(gameObject);
        }
    }
}

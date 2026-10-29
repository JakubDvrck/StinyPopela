using UnityEngine;
using UnityEngine.InputSystem;

namespace StinyPopela
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PrototypeActor : MonoBehaviour
    {
        public CharacterProgress progress = new CharacterProgress();
        public Vector2 touchMovement;
        public int health;
        public int slashRank;
        public int weaponBonus;
        public float attackReady, skillReady;
        CharacterController controller;
        public bool Alive => health > 0;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            health = progress.MaxHealth;
        }

        void Update()
        {
            if (!Alive) return;
            var move = touchMovement;
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                move += new Vector2((keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                    (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
                if (keyboard.spaceKey.wasPressedThisFrame) Attack(false);
                if (keyboard.qKey.wasPressedThisFrame) Attack(true);
            }
            move = Vector2.ClampMagnitude(move, 1);
            var direction = Quaternion.Euler(0, 45, 0) * new Vector3(move.x, 0, move.y);
            controller.Move((direction * 5 + Vector3.down * 4) * Time.deltaTime);
            if (direction.sqrMagnitude > .01f) transform.forward = direction;
        }

        public void Attack(bool skill)
        {
            if (!Alive || Time.time < attackReady || (skill && (slashRank == 0 || Time.time < skillReady))) return;
            attackReady = Time.time + .45f;
            if (skill) skillReady = Time.time + 4;
            float range = skill ? 4 : 2.7f;
            foreach (var enemy in FindObjectsByType<PrototypeEnemy>(FindObjectsSortMode.None))
            {
                var delta = enemy.transform.position - transform.position;
                delta.y = 0;
                if (delta.magnitude <= range && (skill || Vector3.Dot(transform.forward, delta.normalized) > -.1f))
                    enemy.Hit((8 + progress.strength * 2 + weaponBonus) * (skill ? 1.5f + slashRank * .25f : 1));
            }
            PrototypeWorld.Flash(transform.position + Vector3.up, range, skill ? Color.cyan : new Color(1,.7f,.2f));
        }

        public void Hurt(int damage)
        {
            if (Alive) health = Mathf.Max(0, health - damage);
        }

        public void UpgradeSlash()
        {
            if (progress.skillPoints <= 0 || slashRank >= 5) return;
            progress.skillPoints--;
            slashRank++;
        }
    }
}

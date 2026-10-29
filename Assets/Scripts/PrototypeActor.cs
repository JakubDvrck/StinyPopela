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
        float protectedUntil;
        public bool Alive => health > 0;
        public float MoveSpeed => 4.75f + progress.dexterity * .05f;
        public int BasicDamage => 8 + progress.strength * 2 + weaponBonus;
        public int SlashDamage => Mathf.RoundToInt(BasicDamage * (1.5f + slashRank * .25f) + progress.intelligence * 2);

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            health = progress.MaxHealth;
            protectedUntil = Time.time + 3;
        }

        void Update()
        {
            if (!Alive || Time.timeScale<=0) return;
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
            controller.Move((direction * MoveSpeed + Vector3.down * 4) * Time.deltaTime);
            if (direction.sqrMagnitude > .01f) transform.forward = direction;
        }

        public void Attack(bool skill)
        {
            if (!Alive || Time.timeScale<=0 || Time.time < attackReady || (skill && (slashRank == 0 || Time.time < skillReady))) return;
            attackReady = Time.time + .45f;
            if (skill) skillReady = Time.time + 4;
            float range = skill ? 4 : 2.7f;
            PrototypeEnemy nearest = null;
            float nearestDistance = range;
            foreach (var candidate in FindObjectsByType<PrototypeEnemy>(FindObjectsSortMode.None))
            {
                float distance = Vector3.Distance(candidate.transform.position, transform.position);
                if (candidate.health > 0 && distance <= nearestDistance) { nearest = candidate; nearestDistance = distance; }
            }
            if (nearest != null) { var aim = nearest.transform.position - transform.position; aim.y = 0; if (aim.sqrMagnitude > .001f) transform.forward = aim.normalized; }
            foreach (var enemy in FindObjectsByType<PrototypeEnemy>(FindObjectsSortMode.None))
            {
                var delta = enemy.transform.position - transform.position;
                delta.y = 0;
                if (delta.magnitude <= range && (skill || enemy==nearest))
                    enemy.Hit(skill ? SlashDamage : BasicDamage);
            }
            PrototypeWorld.Flash(transform.position + Vector3.up, range, skill ? Color.cyan : new Color(1,.7f,.2f));
        }

        public void Hurt(int damage)
        {
            if(damage<=0)return;
            if (Alive && Time.time >= protectedUntil)
            {
                int actual = Mathf.Min(health, Mathf.Max(1, damage - (PrototypeWorld.Instance != null ? PrototypeWorld.Instance.ArmorBonus : 0)));
                health -= actual;
                CombatFeedback.Show(transform.position + Vector3.up * 2, actual, true);
            }
        }

        public void Revive()
        {
            controller.enabled = false;
            transform.position = new Vector3(0,1,-10);
            controller.enabled = true;
            health = progress.MaxHealth;
            touchMovement = Vector2.zero;
            protectedUntil = Time.time + 3;
        }

        public void UpgradeSlash()
        {
            if (progress.skillPoints <= 0 || slashRank >= 5) return;
            progress.skillPoints--;
            slashRank++;
        }
    }
}

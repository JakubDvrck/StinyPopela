using System;
using UnityEngine;
namespace StinyPopela.Editor
{
    public static class PrototypeVerification
    {
        static void Check(bool value,string message){if(!value)throw new Exception(message);}
        public static string Run()
        {
            bool hadSave=PlayerPrefs.HasKey("prototype-save");string previousSave=PlayerPrefs.GetString("prototype-save","");
            try
            {
                var world=UnityEngine.Object.FindFirstObjectByType<PrototypeWorld>();Check(world!=null,"World missing");
                var player=world.player;
                Check(player.GetComponent<CharacterController>()!=null&&player.GetComponent<CapsuleCollider>()==null,"Collider conflict");
                var enemy=UnityEngine.Object.FindFirstObjectByType<PrototypeEnemy>();
                enemy.transform.position=player.transform.position+new Vector3(0,0,1.5f);enemy.health=45;
                player.transform.forward=Vector3.back;player.slashRank=0;player.attackReady=0;
                GameObject.Find("ÚTOK").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                Check(enemy.health==45-player.BasicDamage,"Initial basic attack or auto aim failed");
                typeof(PrototypeActor).GetField("protectedUntil",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(player,0f);
                int before=player.health;player.Hurt(6);
                Check(player.health==before-Mathf.Max(1,6-world.ArmorBonus),"Actual player damage failed");
                var progress=player.progress;progress.attributePoints=5;
                var screen=(InventoryScreen)typeof(PrototypeWorld).GetField("characterScreen",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(world);
                screen.Open();Check(Time.timeScale==0,"Inventory did not pause game");
                var draft=(AttributeDraft)typeof(InventoryScreen).GetField("draft",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(screen);
                int strength=progress.strength,vitality=progress.vitality;
                draft.Change(0,1);draft.Change(3,1);draft.Change(3,1);
                Check(progress.strength==strength&&progress.vitality==vitality&&progress.attributePoints==5,"Draft spent real points before confirmation");
                GameObject.Find("ZRUŠIT ZMĚNY").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                Check(draft.Remaining==5&&draft.pending[0]==0,"Cancel did not return points");
                draft.Change(0,1);draft.Change(3,1);draft.Change(3,1);
                GameObject.Find("ULOŽIT ATRIBUTY").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                Check(progress.strength==strength+1&&progress.vitality==vitality+2&&progress.attributePoints==2,"Confirmed allocation failed");
                var saved=JsonUtility.FromJson<ProgressEnvelope>(PlayerPrefs.GetString("prototype-save"));
                Check(saved.progress.strength==progress.strength&&saved.progress.attributePoints==2,"Confirmed allocation not persisted");
                Check(!draft.Change(1,-1),"Draft allowed removing committed points");
                Check(draft.Change(1,1)&&draft.Change(1,1)&&!draft.Change(1,1),"Attribute budget exceeded");draft.Cancel();
                var armor=new PrototypeWorld.Loot{title="Test armor",slot="Chest",power=4,width=2,height=2,level=1};
                var oldArmor=world.equipped.Find(x=>x.slot=="Chest");
                bool stored=(bool)typeof(PrototypeWorld).GetMethod("TryStore",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(world,new object[]{armor});
                Check(stored&&world.Equip(armor),"Equipping failed");
                Check(world.equipped.Contains(armor)&&!world.Items.Contains(armor)&&world.Items.Contains(oldArmor)&&world.ArmorBonus>=4,"Equipment swap lost items or bonuses");
                Check(world.Unequip(armor)&&world.Items.Contains(armor),"Unequip failed");
                draft.Change(0,1);screen.Close();Check(progress.strength==strength+1&&progress.attributePoints==2&&Time.timeScale==1,"Closing menu committed draft or left game paused");
                PlayerPrefs.DeleteKey("prototype-save");player.health=0;
                typeof(PrototypeWorld).GetMethod("LoadGame",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(world,null);
                Check(player.Alive&&player.health==progress.MaxHealth&&Vector3.Distance(player.transform.position,new Vector3(0,1,-10))<.1f,"Revive without save failed");
                return "PASS: initial attack button, auto aim, real damage, collider, pause, pending/cancel/commit/persistence/budget, equipment swap, unequip, revive";
            }
            finally{if(hadSave)PlayerPrefs.SetString("prototype-save",previousSave);else PlayerPrefs.DeleteKey("prototype-save");PlayerPrefs.Save();Time.timeScale=1;}
        }
        [Serializable] class ProgressEnvelope {public CharacterProgress progress;}
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace StinyPopela
{
    public sealed class PrototypeWorld : MonoBehaviour
    {
        public static PrototypeWorld Instance { get; private set; }
        public PrototypeActor player;
        public bool bossDefeated;
        Camera view;
        TextMeshProUGUI status, notice, slashLabel;
        InventoryScreen characterScreen;
        public Transform heroVisual;
        public readonly List<Loot> equipped = new List<Loot>();
        public List<Loot> Items => inventory;
        public int ArmorBonus { get { int value=0;foreach(var item in equipped) if(item.slot!="MainHand"&&item.slot!="Ring1"&&item.slot!="Ring2"&&item.slot!="Amulet")value+=item.power;return value; } }
        readonly List<Loot> inventory = new List<Loot>();
        readonly List<GameObject> drops = new List<GameObject>();
        readonly bool[,] occupied = new bool[8,4];
        [Serializable] public class Loot { public string title; public string slot="MainHand"; public int level, power, x, y, width = 1, height = 1; }
        [Serializable] class Save { public CharacterProgress progress; public int slashRank, weaponBonus; public List<Loot> items,equipped; }

        void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
            BuildWorld();
            BuildHud();
        }

        public static GameObject Shape(string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(type); go.name = name;
            go.transform.position = position; go.transform.localScale = scale;
            var template = Resources.Load<Material>("PrototypeLit");
            if (template == null) throw new InvalidOperationException("PrototypeLit material resource unavailable");
            go.GetComponent<Renderer>().material = new Material(template) { color = color };
            return go;
        }

        void BuildWorld()
        {
            Shape("Forest ground", PrimitiveType.Cube, new Vector3(0,-.3f,0), new Vector3(42,.5f,42), new Color(.13f,.23f,.15f));
            Shape("Dungeon floor", PrimitiveType.Cube, new Vector3(0,-.25f,29), new Vector3(18,.5f,18), new Color(.19f,.2f,.24f));
            Shape("Passage", PrimitiveType.Cube, new Vector3(0,-.2f,20), new Vector3(7,.4f,6), new Color(.3f,.28f,.23f));
            for (int i=0;i<18;i++)
            {
                float x = (i%2 == 0 ? -1 : 1) * (8 + i%5*2);
                float z = -15 + i*1.8f;
                Shape("Pine trunk", PrimitiveType.Cylinder, new Vector3(x,1,z), new Vector3(.5f,1,.5f), new Color(.25f,.16f,.1f));
                Shape("Pine crown", PrimitiveType.Sphere, new Vector3(x,3,z), new Vector3(2.7f,4,2.7f), new Color(.08f,.2f,.13f));
            }
            Shape("West wall", PrimitiveType.Cube, new Vector3(-9,1.5f,29), new Vector3(1,3,18), Color.gray);
            Shape("East wall", PrimitiveType.Cube, new Vector3(9,1.5f,29), new Vector3(1,3,18), Color.gray);
            Shape("North wall", PrimitiveType.Cube, new Vector3(0,1.5f,38), new Vector3(18,3,1), Color.gray);
            var hero = new GameObject("Warrior");
            hero.transform.position = new Vector3(0,1,-10);
            var body = Shape("Warrior Visual", PrimitiveType.Capsule, hero.transform.position, Vector3.one, new Color(.3f,.55f,.8f));
            body.transform.SetParent(hero.transform,true);
            body.GetComponent<Collider>().enabled = false;
            Destroy(body.GetComponent<Collider>());
            var cc = hero.AddComponent<CharacterController>(); cc.height=2; cc.radius=.45f;
            player=hero.AddComponent<PrototypeActor>();
            heroVisual=hero.transform;
            var sword = Shape("Sword", PrimitiveType.Cube, hero.transform.position+new Vector3(.7f,0,.5f),new Vector3(.15f,.15f,1.3f),new Color(.8f,.8f,.85f));
            Destroy(sword.GetComponent<Collider>()); sword.transform.SetParent(hero.transform,true);
            equipped.Add(new Loot{title="Cvičný meč",slot="MainHand",level=1,power=0,height=2});
            equipped.Add(new Loot{title="Kožená zbroj",slot="Chest",level=1,power=1,width=2,height=2});
            UpdateEquipment();
            for(int i=0;i<6;i++) SpawnEnemy(new Vector3((i%2==0?-1:1)*3,1,-3+i*3),false);
            SpawnEnemy(new Vector3(0,1.5f,32),true);
            view=Camera.main;
            if(view==null) view=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener)).GetComponent<Camera>();
            view.tag="MainCamera"; view.orthographic=true; view.orthographicSize=10;
            view.transform.rotation=Quaternion.Euler(35.264f,45,0);
            view.farClipPlane=150;
            view.cullingMask &= ~(1<<30);
        }

        void SpawnEnemy(Vector3 position,bool boss)
        {
            var go=Shape(boss?"Dungeon Warden":"Forest Raider",PrimitiveType.Capsule,position,boss?Vector3.one*1.6f:Vector3.one,new Color(.65f,boss ? .12f : .32f,.15f));
            Destroy(go.GetComponent<Collider>());
            var enemy=go.AddComponent<PrototypeEnemy>(); enemy.target=player; enemy.boss=boss; enemy.health=boss?300:45;
        }

        void LateUpdate()
        {
            view.transform.position=player.transform.position-view.transform.forward*23;
            if(slashLabel!=null)slashLabel.text=player.slashRank==0?"SEK\nZAMČENO":Time.time<player.skillReady?$"SEK\n{player.skillReady-Time.time:0.0} s":"SEK";
            status.text=$"STÍNY POPELA • VÁLEČNÍK  |  Úroveň {player.progress.level}\nŽivoty {player.health}/{player.progress.MaxHealth}  •  Zkušenosti {player.progress.experience}/{player.progress.RequiredExperience}\nSíla {player.progress.strength}  Obratnost {player.progress.dexterity}  Inteligence {player.progress.intelligence}  Vitalita {player.progress.vitality}\nVolné body {player.progress.attributePoints}  •  Skill pointy {player.progress.skillPoints}  •  Sek {player.slashRank}/5";
            notice.text=!player.Alive?"Padl jsi. Klepni na NAČÍST pro návrat do bezpečí.":bossDefeated?"Strážce dungeonu poražen! Prototyp dokončen.":"Projdi lesem na sever do dungeonu a poraz strážce.";
            for(int i=drops.Count-1;i>=0;i--)
            {
                var drop=drops[i]; if(drop==null) { drops.RemoveAt(i); continue; }
                if(Vector3.Distance(drop.transform.position,player.transform.position)<1.8f)
                {
                    var info=drop.GetComponent<LootMarker>();
                    if(TryStore(info.item)) { Destroy(drop); drops.RemoveAt(i); RefreshInventory(); }
                }
            }
        }

        public void Drop(Vector3 position,int level,bool unique)
        {
            string[] slots={"MainHand","Chest","Ring","Amulet","Head","Gloves","Legs","Boots","OffHand"};
            string[] titles={"Lesní meč","Zbroj","Prsten","Amulet","Přilba","Rukavice","Kalhoty","Boty","Štít"};
            int kind=unique?0:UnityEngine.Random.Range(0,slots.Length);
            var item=new Loot { title=unique?"Strážcův meč":titles[kind],slot=slots[kind],level=level,power=UnityEngine.Random.Range(level*2,level*3+1),height=kind<2?2:1,width=kind==1?2:1 };
            var go=Shape(item.title,PrimitiveType.Cube,new Vector3(position.x,.5f,position.z),Vector3.one*.5f,unique?Color.yellow:Color.cyan);
            Destroy(go.GetComponent<Collider>()); go.AddComponent<LootMarker>().item=item; drops.Add(go);
        }
        bool TryStore(Loot item)
        {
            for(int y=0;y<=4-item.height;y++) for(int x=0;x<=8-item.width;x++)
            {
                bool free=true;
                for(int a=0;a<item.width;a++) for(int b=0;b<item.height;b++) if(occupied[x+a,y+b]) free=false;
                if(!free) continue;
                item.x=x;item.y=y;inventory.Add(item);
                for(int a=0;a<item.width;a++) for(int b=0;b<item.height;b++) occupied[x+a,y+b]=true;
                return true;
            }
            return false;
        }

        RectTransform Panel(string name,Transform parent,Vector2 anchor,Vector2 position,Vector2 size,Color color)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(UnityEngine.UI.Image)); go.transform.SetParent(parent,false);
            var rect=(RectTransform)go.transform;rect.anchorMin=rect.anchorMax=anchor;rect.anchoredPosition=position;rect.sizeDelta=size;
            go.GetComponent<UnityEngine.UI.Image>().color=color; return rect;
        }
        TextMeshProUGUI Label(Transform parent,string text,Vector2 anchor,Vector2 position,Vector2 size,int fontSize)
        {
            var go=new GameObject("Label",typeof(RectTransform),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);
            var rect=(RectTransform)go.transform;rect.anchorMin=rect.anchorMax=anchor;rect.anchoredPosition=position;rect.sizeDelta=size;
            var label=go.GetComponent<TextMeshProUGUI>();label.text=text;label.fontSize=fontSize;label.color=Color.white;label.raycastTarget=false;return label;
        }
        void Button(Transform parent,string title,Vector2 anchor,Vector2 position,Vector2 size,UnityEngine.Events.UnityAction action)
        {
            var rect=Panel(title,parent,anchor,position,size,new Color(.12f,.16f,.21f,.95f));
            rect.gameObject.AddComponent<UnityEngine.UI.Button>().onClick.AddListener(action);
            var label=Label(rect,title,new Vector2(.5f,.5f),Vector2.zero,size-Vector2.one*8,18);label.alignment=TextAlignmentOptions.Center;
            if(title=="SEK (Q)")slashLabel=label;
        }
        void BuildHud()
        {
            var canvas=new GameObject("HUD",typeof(Canvas),typeof(UnityEngine.UI.CanvasScaler),typeof(UnityEngine.UI.GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=canvas.GetComponent<UnityEngine.UI.CanvasScaler>();scaler.uiScaleMode=UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);scaler.matchWidthOrHeight=.5f;
            var safe=new GameObject("SafeArea",typeof(RectTransform));safe.transform.SetParent(canvas.transform,false);
            var r=(RectTransform)safe.transform;var area=Screen.safeArea;r.anchorMin=area.min/new Vector2(Screen.width,Screen.height);r.anchorMax=area.max/new Vector2(Screen.width,Screen.height);r.offsetMin=r.offsetMax=Vector2.zero;
            var root=safe.transform;
            Panel("Status",root,new Vector2(0,1),new Vector2(310,-84),new Vector2(610,155),new Color(.03f,.05f,.07f,.85f));
            status=Label(root,"",new Vector2(0,1),new Vector2(310,-84),new Vector2(590,145),19);
            notice=Label(root,"",new Vector2(.5f,1),new Vector2(0,-185),new Vector2(900,40),20);notice.alignment=TextAlignmentOptions.Center;
            var stick=Panel("Joystick",root,Vector2.zero,new Vector2(110,110),new Vector2(150,150),new Color(.1f,.2f,.23f,.8f));
            var control=stick.gameObject.AddComponent<TouchStick>();control.actor=player;control.knob=Panel("Knob",stick,new Vector2(.5f,.5f),Vector2.zero,new Vector2(50,50),Color.gray);
            control.knob.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;
            Button(root,"ÚTOK",new Vector2(1,0),new Vector2(-100,100),new Vector2(130,90),()=>player.Attack(false));
            Button(root,"SEK (Q)",new Vector2(1,0),new Vector2(-250,100),new Vector2(130,90),()=>player.Attack(true));
            Button(root,"VYLEPŠIT SEK",new Vector2(1,0),new Vector2(-190,200),new Vector2(250,50),player.UpgradeSlash);
            Button(root,"POSTAVA / VÝBAVA",new Vector2(1,1),new Vector2(-110,-35),new Vector2(200,45),()=>characterScreen.Toggle());
            Button(root,"ULOŽIT",new Vector2(1,1),new Vector2(-95,-90),new Vector2(170,45),SaveGame);
            Button(root,"NAČÍST",new Vector2(1,1),new Vector2(-95,-145),new Vector2(170,45),LoadGame);
            CombatFeedback.Initialize(r);
            characterScreen=new InventoryScreen(this,r);
            if(FindFirstObjectByType<EventSystem>()==null)new GameObject("EventSystem",typeof(EventSystem),typeof(InputSystemUIInputModule));
        }
        void RefreshInventory()
        {
            characterScreen?.Refresh();
        }
        public void SaveGame()
        {
            var data=new Save {progress=player.progress,slashRank=player.slashRank,weaponBonus=player.weaponBonus,items=inventory,equipped=equipped};
            PlayerPrefs.SetString("prototype-save",JsonUtility.ToJson(data));PlayerPrefs.Save();
        }
        void LoadGame()
        {
            if(!PlayerPrefs.HasKey("prototype-save")) { player.Revive(); return; }
            var data=JsonUtility.FromJson<Save>(PlayerPrefs.GetString("prototype-save"));
            if(data?.progress==null)return;
            player.progress=data.progress;player.slashRank=data.slashRank;player.weaponBonus=data.weaponBonus;player.Revive();
            inventory.Clear();Array.Clear(occupied,0,occupied.Length);
            if(data.items!=null)foreach(var item in data.items)TryStore(item);
            if(data.equipped!=null){equipped.Clear();equipped.AddRange(data.equipped);}
            UpdateEquipment();
            RefreshInventory();
        }
        public bool Equip(Loot item)
        {
            if(!inventory.Contains(item))return false;
            string slot=string.IsNullOrEmpty(item.slot)?"MainHand":item.slot;
            if(slot=="Ring")slot=equipped.Exists(x=>x.slot=="Ring1")&&!equipped.Exists(x=>x.slot=="Ring2")?"Ring2":"Ring1";
            var old=equipped.Find(x=>x.slot==slot);
            inventory.Remove(item);for(int a=0;a<item.width;a++)for(int b=0;b<item.height;b++)occupied[item.x+a,item.y+b]=false;
            if(old!=null&&!TryStore(old)){TryStore(item);return false;}
            if(old!=null)equipped.Remove(old);item.slot=slot;equipped.Add(item);UpdateEquipment();SaveGame();RefreshInventory();return true;
        }
        public bool Unequip(Loot item)
        {
            if(!equipped.Contains(item)||!TryStore(item))return false;
            equipped.Remove(item);UpdateEquipment();SaveGame();RefreshInventory();return true;
        }
        public void UpdateEquipment()
        {
            player.weaponBonus=0;foreach(var item in equipped)if(item.slot=="MainHand"||item.slot=="Ring1"||item.slot=="Ring2"||item.slot=="Amulet")player.weaponBonus+=item.power;
            var sword=player.transform.Find("Sword");if(sword!=null)sword.gameObject.SetActive(equipped.Exists(x=>x.slot=="MainHand"));
            SetWorn("Armor visual","Chest",new Vector3(0,.05f,0),new Vector3(1.05f,.9f,1.05f),new Color(.4f,.29f,.18f));
            SetWorn("Helmet visual","Head",new Vector3(0,.95f,0),new Vector3(.65f,.4f,.65f),Color.gray);
            SetWorn("Shield visual","OffHand",new Vector3(-.65f,0,.3f),new Vector3(.12f,.85f,.6f),new Color(.35f,.3f,.23f));
            SetWorn("Gloves visual","Gloves",new Vector3(.5f,0,0),new Vector3(.2f,.3f,.3f),Color.gray);
            SetWorn("Legs visual","Legs",new Vector3(0,-.5f,0),new Vector3(.65f,.4f,.5f),new Color(.23f,.25f,.3f));
            SetWorn("Boots visual","Boots",new Vector3(0,-.85f,.1f),new Vector3(.7f,.25f,.55f),new Color(.2f,.16f,.12f));
        }
        void SetWorn(string name,string slot,Vector3 position,Vector3 scale,Color color)
        {
            var existing=player.transform.Find(name);
            if(existing==null)
            {
                var obj=Shape(name,PrimitiveType.Cube,Vector3.zero,scale,color);obj.GetComponent<Collider>().enabled=false;Destroy(obj.GetComponent<Collider>());
                obj.transform.SetParent(player.transform,false);obj.transform.localPosition=position;existing=obj.transform;
            }
            existing.gameObject.SetActive(equipped.Exists(x=>x.slot==slot));
        }
        void OnDestroy(){Time.timeScale=1;characterScreen?.Dispose();if(Instance==this)Instance=null;}
        public static void Flash(Vector3 position,float radius,Color color)
        {
            var go=new GameObject("Attack ring");var line=go.AddComponent<LineRenderer>();
            var material=new Material(Resources.Load<Material>("PrototypeLit")){color=color};line.sharedMaterial=material;
            line.loop=true;line.positionCount=40;line.widthMultiplier=.07f;line.useWorldSpace=true;
            line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;line.receiveShadows=false;
            for(int i=0;i<40;i++){float angle=i*Mathf.PI*2/40;line.SetPosition(i,new Vector3(position.x+Mathf.Cos(angle)*radius,.08f,position.z+Mathf.Sin(angle)*radius));}
            Destroy(go,.18f);Destroy(material,.2f);
        }
    }
    public sealed class LootMarker : MonoBehaviour { public PrototypeWorld.Loot item; }
}

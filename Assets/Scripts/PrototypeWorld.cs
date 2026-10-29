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
        TextMeshProUGUI status, notice;
        GameObject inventoryPanel;
        readonly List<Loot> inventory = new List<Loot>();
        readonly List<GameObject> drops = new List<GameObject>();
        readonly bool[,] occupied = new bool[8,4];
        [Serializable] public class Loot { public string title; public int level, power, x, y, width = 1, height = 1; }
        [Serializable] class Save { public CharacterProgress progress; public int slashRank, weaponBonus; public List<Loot> items; }

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
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("URP Lit shader unavailable");
            go.GetComponent<Renderer>().material = new Material(shader) { color = color };
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
            var hero = Shape("Warrior", PrimitiveType.Capsule, new Vector3(0,1,-10), Vector3.one, new Color(.3f,.55f,.8f));
            Destroy(hero.GetComponent<Collider>());
            var cc = hero.AddComponent<CharacterController>(); cc.height=2; cc.radius=.45f;
            player=hero.AddComponent<PrototypeActor>();
            var sword = Shape("Sword", PrimitiveType.Cube, hero.transform.position+new Vector3(.7f,0,.5f),new Vector3(.15f,.15f,1.3f),new Color(.8f,.8f,.85f));
            Destroy(sword.GetComponent<Collider>()); sword.transform.SetParent(hero.transform,true);
            for(int i=0;i<6;i++) SpawnEnemy(new Vector3((i%2==0?-1:1)*3,1,-3+i*3),false);
            SpawnEnemy(new Vector3(0,1.5f,32),true);
            view=Camera.main;
            if(view==null) view=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener)).GetComponent<Camera>();
            view.tag="MainCamera"; view.orthographic=true; view.orthographicSize=10;
            view.transform.rotation=Quaternion.Euler(35.264f,45,0);
            view.farClipPlane=150;
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
            status.text=$"STÍNY POPELA • VÁLEČNÍK  |  Úroveň {player.progress.level}\nŽivoty {player.health}/{player.progress.MaxHealth}  •  Zkušenosti {player.progress.experience}/{player.progress.RequiredExperience}\nSíla {player.progress.strength}  Obratnost {player.progress.dexterity}  Inteligence {player.progress.intelligence}  Vitalita {player.progress.vitality}\nVolné body {player.progress.attributePoints}  •  Skill pointy {player.progress.skillPoints}  •  Sek {player.slashRank}/5";
            notice.text=!player.Alive?"Padl jsi. Obnov hru tlačítkem NAČÍST nebo restartuj scénu.":bossDefeated?"Strážce dungeonu poražen! Prototyp dokončen.":"Projdi lesem na sever do dungeonu a poraz strážce.";
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
            var item=new Loot { title=unique?"Strážcův meč":"Lesní meč",level=level,power=UnityEngine.Random.Range(level*2,level*3+1),height=2 };
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
            Button(root,"INVENTÁŘ",new Vector2(1,1),new Vector2(-95,-35),new Vector2(170,45),()=>inventoryPanel.SetActive(!inventoryPanel.activeSelf));
            Button(root,"ULOŽIT",new Vector2(1,1),new Vector2(-95,-90),new Vector2(170,45),SaveGame);
            Button(root,"NAČÍST",new Vector2(1,1),new Vector2(-95,-145),new Vector2(170,45),LoadGame);
            string[] names={"+ SÍLA","+ OBRATNOST","+ INTELIGENCE","+ VITALITA"};
            for(int i=0;i<4;i++) {int index=i;Button(root,names[i],new Vector2(.5f,0),new Vector2(-225+i*150,35),new Vector2(140,45),()=>{player.progress.SpendAttribute(index);});}
            inventoryPanel=Panel("Inventory",root,new Vector2(.5f,.5f),Vector2.zero,new Vector2(520,330),new Color(.04f,.06f,.09f,.98f)).gameObject;
            RefreshInventory();inventoryPanel.SetActive(false);
            if(FindFirstObjectByType<EventSystem>()==null)new GameObject("EventSystem",typeof(EventSystem),typeof(InputSystemUIInputModule));
        }
        void RefreshInventory()
        {
            if(inventoryPanel==null)return;
            foreach(Transform child in inventoryPanel.transform)Destroy(child.gameObject);
            Label(inventoryPanel.transform,"INVENTÁŘ • klepnutím vybavíš zbraň",new Vector2(.5f,1),new Vector2(0,-25),new Vector2(490,40),20);
            for(int y=0;y<4;y++)for(int x=0;x<8;x++)Panel("Cell",inventoryPanel.transform,new Vector2(0,1),new Vector2(43+x*61,-83-y*61),new Vector2(57,57),new Color(.14f,.18f,.22f));
            foreach(var item in inventory)
            {
                var captured=item;
                Button(inventoryPanel.transform,$"{item.title}\nL{item.level} +{item.power}",new Vector2(0,1),new Vector2(43+item.x*61+(item.width-1)*30.5f,-83-item.y*61-(item.height-1)*30.5f),new Vector2(item.width*61-4,item.height*61-4),()=>player.weaponBonus=captured.power);
            }
        }
        void SaveGame()
        {
            var data=new Save {progress=player.progress,slashRank=player.slashRank,weaponBonus=player.weaponBonus,items=inventory};
            PlayerPrefs.SetString("prototype-save",JsonUtility.ToJson(data));PlayerPrefs.Save();
        }
        void LoadGame()
        {
            if(!PlayerPrefs.HasKey("prototype-save"))return;
            var data=JsonUtility.FromJson<Save>(PlayerPrefs.GetString("prototype-save"));
            if(data?.progress==null)return;
            player.progress=data.progress;player.slashRank=data.slashRank;player.weaponBonus=data.weaponBonus;player.health=player.progress.MaxHealth;
            inventory.Clear();Array.Clear(occupied,0,occupied.Length);
            if(data.items!=null)foreach(var item in data.items)TryStore(item);
            RefreshInventory();
        }
        public static void Flash(Vector3 position,float radius,Color color)
        {
            var go=Shape("Attack flash",PrimitiveType.Cylinder,position,new Vector3(radius*2,.025f,radius*2),color);
            Destroy(go.GetComponent<Collider>());Destroy(go,.12f);
        }
    }
    public sealed class LootMarker : MonoBehaviour { public PrototypeWorld.Loot item; }
}

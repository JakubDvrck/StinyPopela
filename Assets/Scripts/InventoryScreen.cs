using System;
using UnityEngine;
using TMPro;
namespace StinyPopela
{
    public sealed class InventoryScreen
    {
        readonly PrototypeWorld world;
        readonly RectTransform panel,grid,paper;
        readonly TextMeshProUGUI detail,points;
        readonly TextMeshProUGUI[] attributes=new TextMeshProUGUI[4];
        AttributeDraft draft;
        Camera previewCamera;
        RenderTexture texture;
        GameObject preview;
        float previousTimeScale=1;
        readonly string[] slots={"Head","Chest","Gloves","Legs","Boots","MainHand","OffHand","Ring1","Ring2","Amulet"};
        readonly string[] names={"Hlava","Zbroj","Rukavice","Kalhoty","Boty","Zbraň","Druhá ruka","Prsten 1","Prsten 2","Amulet"};
        public InventoryScreen(PrototypeWorld world,RectTransform root)
        {
            this.world=world;
            panel=Box(root,"Character menu",Vector2.zero,new Vector2(1100,660),new Color(.035f,.05f,.075f,.99f));
            panel.GetComponent<UnityEngine.UI.Image>().raycastTarget=true;
            Text(panel,"POSTAVA • VÝBAVA • INVENTÁŘ",new Vector2(-80,300),new Vector2(800,40),24);
            Button(panel,"ZAVŘÍT",new Vector2(455,300),new Vector2(140,38),Close);
            paper=Box(panel,"Equipment",new Vector2(-280,110),new Vector2(500,310),new Color(.07f,.09f,.12f));
            grid=Box(panel,"Grid inventory",new Vector2(270,110),new Vector2(500,310),new Color(.07f,.09f,.12f));
            detail=Text(panel,"Klepni na předmět v mřížce pro porovnání a vybavení.",new Vector2(0,-66),new Vector2(1040,40),17);
            Box(panel,"Attributes",new Vector2(0,-203),new Vector2(1060,230),new Color(.055f,.075f,.1f));
            points=Text(panel,"",new Vector2(0,-104),new Vector2(1030,30),18);
            string[] titles={"SÍLA","OBRATNOST","INTELIGENCE","VITALITA"};
            for(int i=0;i<4;i++)
            {
                int index=i;float x=-390+i*260;
                Text(panel,titles[i],new Vector2(x,-140),new Vector2(230,28),18);
                attributes[i]=Text(panel,"",new Vector2(x,-201),new Vector2(240,100),16);
                Button(panel,"−",new Vector2(x-75,-168),new Vector2(40,30),()=>Change(index,-1));
                Button(panel,"+",new Vector2(x+75,-168),new Vector2(40,30),()=>Change(index,1));
            }
            Button(panel,"ULOŽIT ATRIBUTY",new Vector2(-155,-287),new Vector2(280,40),()=>Apply());
            Button(panel,"ZRUŠIT ZMĚNY",new Vector2(155,-287),new Vector2(280,40),()=>{draft.Cancel();RefreshAttributes();});
            CreatePreview();panel.gameObject.SetActive(false);
        }
        public static RectTransform Box(Transform parent,string name,Vector2 position,Vector2 size,Color color)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(UnityEngine.UI.Image));go.transform.SetParent(parent,false);
            var rect=(RectTransform)go.transform;rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.sizeDelta=size;rect.anchoredPosition=position;
            var image=go.GetComponent<UnityEngine.UI.Image>();image.color=color;image.raycastTarget=false;return rect;
        }
        public static TextMeshProUGUI Text(Transform parent,string value,Vector2 position,Vector2 size,int fontSize)
        {
            var go=new GameObject("Text",typeof(RectTransform),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);
            var rect=(RectTransform)go.transform;rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.sizeDelta=size;rect.anchoredPosition=position;
            var text=go.GetComponent<TextMeshProUGUI>();text.text=value;text.fontSize=fontSize;text.color=Color.white;text.raycastTarget=false;text.alignment=TextAlignmentOptions.Center;return text;
        }
        public static void Button(Transform parent,string title,Vector2 position,Vector2 size,UnityEngine.Events.UnityAction callback)
        {
            var rect=Box(parent,title,position,size,new Color(.16f,.22f,.29f));rect.GetComponent<UnityEngine.UI.Image>().raycastTarget=true;
            rect.gameObject.AddComponent<UnityEngine.UI.Button>().onClick.AddListener(callback);
            Text(rect,title,Vector2.zero,size-new Vector2(6,2),15);
        }
        public void Toggle(){if(panel.gameObject.activeSelf)Close();else Open();}
        public void Open()
        {
            if(panel.gameObject.activeSelf)return;
            draft=new AttributeDraft(world.player.progress);previousTimeScale=Time.timeScale;Time.timeScale=0;
            world.player.touchMovement=Vector2.zero;panel.gameObject.SetActive(true);previewCamera.enabled=true;Refresh();
        }
        public void Close()
        {
            if(!panel.gameObject.activeSelf)return;
            draft?.Cancel();panel.gameObject.SetActive(false);previewCamera.enabled=false;Time.timeScale=previousTimeScale;
        }
        void Change(int i,int delta){draft.Change(i,delta);RefreshAttributes();}
        public bool Apply()
        {
            int oldMaximum=world.player.progress.MaxHealth;
            if(!draft.Commit())return false;
            if(world.player.Alive)world.player.health=Mathf.Min(world.player.progress.MaxHealth,world.player.health+world.player.progress.MaxHealth-oldMaximum);
            world.SaveGame();RefreshAttributes();detail.text="Atributy potvrzeny a uloženy.";return true;
        }
        void RefreshAttributes()
        {
            if(draft==null)return;
            points.text=$"VOLNÉ BODY: {draft.Remaining}   •   Změny se použijí až po ULOŽIT ATRIBUTY";
            var p=world.player.progress;
            int proposedSlash=Mathf.RoundToInt((world.player.BasicDamage+draft.pending[0]*2)*(1.5f+world.player.slashRank*.25f)+draft.Proposed(2)*2);
            attributes[0].text=$"{p.strength} → {draft.Proposed(0)} (+{draft.pending[0]})\n\n+2 poškození / bod\nÚtok {world.player.BasicDamage} → {world.player.BasicDamage+draft.pending[0]*2} (+{draft.pending[0]*2})";
            attributes[1].text=$"{p.dexterity} → {draft.Proposed(1)} (+{draft.pending[1]})\n\n+0,05 rychlosti / bod\nPohyb {world.player.MoveSpeed:0.00} → {world.player.MoveSpeed+draft.pending[1]*.05f:0.00}";
            attributes[2].text=$"{p.intelligence} → {draft.Proposed(2)} (+{draft.pending[2]})\n\n+2 poškození seku / bod\nSek {world.player.SlashDamage} → {proposedSlash} (+{proposedSlash-world.player.SlashDamage})";
            attributes[3].text=$"{p.vitality} → {draft.Proposed(3)} (+{draft.pending[3]})\n\n+10 životů / bod\nŽivoty {p.MaxHealth} → {p.MaxHealth+draft.pending[3]*10} (+{draft.pending[3]*10})";
        }
        public void Refresh()
        {
            if(!panel.gameObject.activeSelf)return;
            if(draft==null||!ReferenceEquals(draft.Source,world.player.progress))draft=new AttributeDraft(world.player.progress);
            foreach(Transform child in paper)UnityEngine.Object.Destroy(child.gameObject);
            foreach(Transform child in grid)UnityEngine.Object.Destroy(child.gameObject);
            var image=new GameObject("Character preview",typeof(RectTransform),typeof(UnityEngine.UI.RawImage));image.transform.SetParent(paper,false);
            var rect=(RectTransform)image.transform;rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.sizeDelta=new Vector2(170,280);
            image.GetComponent<UnityEngine.UI.RawImage>().texture=texture;image.GetComponent<UnityEngine.UI.RawImage>().raycastTarget=false;
            for(int i=0;i<slots.Length;i++)
            {
                string slot=slots[i];var equipped=world.equipped.Find(x=>x.slot==slot);
                Button(paper,names[i]+"\n"+(equipped==null?"—":equipped.title+" +"+equipped.power),new Vector2(i<5?-170:170,120-(i%5)*58),new Vector2(145,52),()=>
                {
                    if(equipped==null){detail.text="Prázdný slot: "+SlotName(slot);return;}
                    if(!world.Unequip(equipped))detail.text="Pro sundání předmětu není místo v inventáři.";
                    else detail.text="Předmět přesunut do inventáře.";
                });
            }
            Text(grid,"INVENTÁŘ 8 × 4 • klepnutím vybavíš",new Vector2(0,134),new Vector2(480,30),17);
            for(int y=0;y<4;y++)for(int x=0;x<8;x++)Box(grid,"Cell",new Vector2(-196+x*56,82-y*56),new Vector2(52,52),new Color(.13f,.17f,.22f));
            foreach(var item in world.Items)
            {
                var captured=item;
                Button(grid,item.title+"\n+"+item.power,new Vector2(-196+item.x*56+(item.width-1)*28,82-item.y*56-(item.height-1)*28),new Vector2(item.width*56-4,item.height*56-4),()=>
                {
                    string slot=captured.slot??"MainHand";
                    string targetSlot=slot=="Ring"?(world.equipped.Exists(x=>x.slot=="Ring1")&&!world.equipped.Exists(x=>x.slot=="Ring2")?"Ring2":"Ring1"):slot;
                    var old=world.equipped.Find(x=>x.slot==targetSlot);
                    string comparison=$"{captured.title} L{captured.level}: {Effect(slot)} +{captured.power}"+(old==null?"":$" • dříve +{old.power} • rozdíl {captured.power-old.power:+0;-0;0}");
                    bool success=world.Equip(captured);detail.text=success?comparison+" • VYBAVENO":comparison+" • V inventáři není místo.";
                });
            }
            RebuildPreview();RefreshAttributes();
        }
        string SlotName(string slot) {int index=Array.IndexOf(slots,slot);return index>=0?names[index]:slot;}
        static string Effect(string slot) => slot=="MainHand"||slot.StartsWith("Ring")||slot=="Amulet"?"útok":"obrana";
        void CreatePreview()
        {
            texture=new RenderTexture(256,384,16);texture.Create();
            var go=new GameObject("Inventory Preview Camera");previewCamera=go.AddComponent<Camera>();previewCamera.cullingMask=1<<30;
            previewCamera.clearFlags=CameraClearFlags.SolidColor;previewCamera.backgroundColor=new Color(.055f,.075f,.1f);previewCamera.orthographic=true;previewCamera.orthographicSize=1.55f;previewCamera.targetTexture=texture;
            go.transform.position=new Vector3(1003,1001.5f,1005);go.transform.LookAt(new Vector3(1000,1000,1000));previewCamera.nearClipPlane=.1f;previewCamera.farClipPlane=20;previewCamera.enabled=false;
            var light=go.AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.5f;light.cullingMask=1<<30;
        }
        void RebuildPreview()
        {
            if(preview!=null)UnityEngine.Object.Destroy(preview);
            preview=new GameObject("Equipped character preview");preview.transform.position=new Vector3(1000,1000,1000);
            foreach(Transform child in world.heroVisual)
            {
                var clone=UnityEngine.Object.Instantiate(child.gameObject,preview.transform,false);
                foreach(var transform in clone.GetComponentsInChildren<Transform>(true))transform.gameObject.layer=30;
                foreach(var collider in clone.GetComponentsInChildren<Collider>(true))collider.enabled=false;
            }
            preview.transform.rotation=Quaternion.Euler(0,180,0);
        }
        public void Dispose()
        {
            if(preview!=null)UnityEngine.Object.Destroy(preview);
            if(previewCamera!=null)UnityEngine.Object.Destroy(previewCamera.gameObject);
            if(texture!=null){texture.Release();UnityEngine.Object.Destroy(texture);}
        }
    }
}

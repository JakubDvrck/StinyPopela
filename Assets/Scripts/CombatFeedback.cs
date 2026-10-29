using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace StinyPopela
{
    public sealed class CombatFeedback : MonoBehaviour
    {
        static CombatFeedback instance;
        RectTransform root;
        int sequence;
        class Bar { public PrototypeEnemy enemy; public float maximum; public RectTransform rect,fill; public TextMeshProUGUI text; }
        class Number { public Vector3 position; public float age; public TextMeshProUGUI text; }
        readonly List<Bar> bars=new List<Bar>();
        readonly List<Number> numbers=new List<Number>();
        public static void Initialize(RectTransform root)
        {
            var go=new GameObject("Combat overlay",typeof(RectTransform));go.transform.SetParent(root,false);
            go.transform.SetAsFirstSibling();
            var rect=(RectTransform)go.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
            instance=go.AddComponent<CombatFeedback>();instance.root=rect;
        }
        public static void Track(PrototypeEnemy enemy,float maximum)
        {
            if(instance==null)return;
            var rect=InventoryScreen.Box(instance.root,"Enemy health",Vector2.zero,new Vector2(enemy.boss?150:90,9),new Color(.03f,.03f,.03f,.9f));
            rect.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;
            var fill=InventoryScreen.Box(rect,"Fill",Vector2.zero,Vector2.zero,new Color(.75f,.12f,.12f));
            fill.anchorMin=Vector2.zero;fill.anchorMax=Vector2.one;fill.offsetMin=new Vector2(1,1);fill.offsetMax=new Vector2(-1,-1);fill.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;
            var text=InventoryScreen.Text(rect,"",new Vector2(0,15),new Vector2(160,20),13);
            instance.bars.Add(new Bar{enemy=enemy,maximum=maximum,rect=rect,fill=fill,text=text});
        }
        public static void Show(Vector3 position,float damage,bool playerDamage)
        {
            if(instance==null||damage<=0)return;
            var text=InventoryScreen.Text(instance.root,(playerDamage?"−":"")+damage.ToString("0.#"),Vector2.zero,new Vector2(140,40),26);
            text.color=playerDamage?new Color(1,.25f,.25f):new Color(1,.9f,.45f);text.fontStyle=FontStyles.Bold;
            var camera=Camera.main;
            if(camera!=null)position+=camera.transform.right*((instance.sequence++%3-1)*.5f);
            if(playerDamage)position+=Vector3.up*.45f;
            instance.numbers.Add(new Number{position=position,text=text});
        }
        bool Place(RectTransform rect,Vector3 position)
        {
            var camera=Camera.main;if(camera==null)return false;
            var screen=camera.WorldToScreenPoint(position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root,screen,null,out var point);rect.anchoredPosition=point;
            return screen.z>0 && screen.x>=0&&screen.x<=Screen.width&&screen.y>=0&&screen.y<=Screen.height;
        }
        void LateUpdate()
        {
            if(Time.timeScale<=0)
            {
                foreach(var bar in bars)if(bar.rect!=null)bar.rect.gameObject.SetActive(false);
                foreach(var number in numbers)if(number.text!=null)number.text.gameObject.SetActive(false);
                return;
            }
            for(int i=bars.Count-1;i>=0;i--)
            {
                var bar=bars[i];if(bar.enemy==null||bar.enemy.health<=0){Destroy(bar.rect.gameObject);bars.RemoveAt(i);continue;}
                bar.rect.gameObject.SetActive(Place(bar.rect,bar.enemy.transform.position+Vector3.up*(bar.enemy.boss?2.7f:1.5f)));
                bar.fill.anchorMax=new Vector2(Mathf.Clamp01(bar.enemy.health/bar.maximum),1);
                bar.text.text=$"{Mathf.CeilToInt(bar.enemy.health)}/{bar.maximum:0}";
            }
            for(int i=numbers.Count-1;i>=0;i--)
            {
                var n=numbers[i];n.age+=Time.deltaTime;if(n.age>1){Destroy(n.text.gameObject);numbers.RemoveAt(i);continue;}
                n.text.gameObject.SetActive(Place((RectTransform)n.text.transform,n.position+Vector3.up*n.age));
                var color=n.text.color;color.a=1-n.age;n.text.color=color;
            }
        }
        void OnDestroy() { if(instance==this)instance=null; }
    }
}

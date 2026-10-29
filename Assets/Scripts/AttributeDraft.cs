using System;
namespace StinyPopela
{
    public sealed class AttributeDraft
    {
        readonly CharacterProgress source;
        public CharacterProgress Source => source;
        public readonly int[] pending = new int[4];
        public AttributeDraft(CharacterProgress source) { this.source = source; }
        public int Remaining { get { int used=0; foreach(int n in pending) used+=n; return source.attributePoints-used; } }
        public int Current(int i) => i==0?source.strength:i==1?source.dexterity:i==2?source.intelligence:source.vitality;
        public int Proposed(int i) => Current(i)+pending[i];
        public bool Change(int i,int delta)
        {
            if(i<0||i>3||Math.Abs(delta)!=1||pending[i]+delta<0||(delta>0&&Remaining<=0))return false;
            pending[i]+=delta;return true;
        }
        public void Cancel() { Array.Clear(pending,0,pending.Length); }
        public bool Commit()
        {
            if(Remaining<0)return false;
            int used=0;foreach(int n in pending) { if(n<0)return false;used+=n; }
            source.strength+=pending[0];source.dexterity+=pending[1];source.intelligence+=pending[2];source.vitality+=pending[3];
            source.attributePoints-=used;Cancel();return true;
        }
    }
}

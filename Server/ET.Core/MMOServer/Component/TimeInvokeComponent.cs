using System.Collections.Generic;
using System;
using UnityEngine;
namespace ETModel
{
    [ObjectSystem]
    public class TimeInvokeComponentUpdateComponent : UpdateSystem<TimeInvokeComponent>
    {
        public override void Update(TimeInvokeComponent self)
        {
            self.Update();
        }
    }

    public class Inv
    {
        public float rate;
        public float timing;
        public string name;
        public Action a;
        public Inv(Action a,float rate)
        {
            // .Method.Name 获取方法名
            this.rate = rate;
            this.timing = 0;
            this.a = a;
            this.name = a.Method.Name;
        }
    }

	public  class TimeInvokeComponent : Component
	{   
        List<Inv> timeDict = new List<Inv>();

        public bool IsInvoking(Action a){
            bool has = false;
            foreach (Inv inv in timeDict){
                if(inv.name == a.Method.Name) return true;
            }
			return has;   
        }
        public void CancelInvoke(Action a){
            RemoveInvoke(a.Method.Name);
        }

        public void InvokeRepeating(Action a, float repeatRate){
            if(IsInvoking(a))
            {
                Log.Error($"已经存在此Repeating方法:{a.Method.Name}");
                return;
            } 

            Inv inv = new Inv(a,repeatRate);
            timeDict.Add(inv);
        }

        public void Update()
        {
            // Run Invoke执行时，可能inv已经移除，这里要判断
            if(timeDict.Count > 0) {
                for(int k = 0;k<timeDict.Count;++k){
                    Inv inv = timeDict[k];
                    if(inv!=null) Run(inv);
                }
            }
        }

        void RemoveInvoke(string name){
            // foreach 迭代变量无法赋值,用for
            for (int i = timeDict.Count-1; i>=0; i--){
                Inv inv = timeDict[i];
                timeDict.Remove(inv);
                inv = null;
            }
        }

        void Run(Inv inv){
            inv.timing += Time.fixedDeltaTime;
            if (inv.timing > inv.rate)
            {
                inv.timing = 0;
                inv.a.Invoke();
            }
        }

    }
}
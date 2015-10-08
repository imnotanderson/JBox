using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class World  {

    List<Box> boxList = new List<Box>();
    List<Box> triggerList = new List<Box>();

    float g = 1;

    public void Upt(float deltaTime)
    {
        foreach (Box box in boxList)
        {
            box.AddSpeed(-Vector2.up * g);
            box.Move(deltaTime);   
        }
    }

    public void AddBox(Box box,bool isTrigger=false)
    {
        box.world = this;
        var list = isTrigger ? triggerList : boxList;
        list.Add(box);
    }

    public void Draw()
    {
        Gizmos.color = Color.white;
        foreach (var box in boxList)
        {
            box.Draw();
        }
        Gizmos.color = Color.red;
        foreach (var box in triggerList)
        {
            box.Draw();
        }
    }

    public bool CheckHit(Box box,out Box hitBox)
    {
        hitBox = null;
        var ps = box.GetPoints();
        foreach (var b in triggerList)
        {
            foreach (var p in ps)
            {
                if (b.PointInBox(p))
                {
                    hitBox = b;
                    return true;
                }
            }
        }
        return false;
    }

}

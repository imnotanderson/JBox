using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class World  {
    
    public Box testBox = null;
    public List<Box> boxList = new List<Box>();
    public List<Box> triggerList = new List<Box>();

    float g = 1;

    public void Upt(float deltaTime)
    {
        foreach (Box box in boxList)
        {
            box.AddSpeed(-Vector2.up * g);
            box.speed.x = 10;
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
        Gizmos.color = Color.green;
        if(testBox!=null)
        {
            testBox.Draw();
        }
    }

    public void MoveBox(Box box, Vector2 speed)
    {
        Vector2 xSpeed = new Vector2(speed.x, 0);
        Vector2 ySpeed = new Vector2(0, speed.y);
        
        bool xInBox = false;
        bool yInBox = false;
        foreach (var b in triggerList)
        {
            bool tmXInBox = false;
            bool tmYInBox = false;
            tmXInBox = (box.CheckMoveBoxX(b, speed));
            tmYInBox = (box.CheckMoveBoxY(b, speed));
            if(tmXInBox || tmYInBox)
            box.GetPivotPos(b, box.pos + speed, speed);
            if(tmXInBox)xInBox = true;
            if(tmYInBox)yInBox = true;
        }
        if (xInBox || yInBox)
        {
            if (xInBox)
            {
                box.speed.x = 0;
            }
            else
            {
                box.pos.x+=speed.x;
            }
            if (yInBox)
            {
                box.speed.y = 0;
            }
            else
            {
                box.pos.y+=speed.y;
            }
            return;
        }
        box.pos += speed;

    }

    public void MoveBoxX()
    { 
    
    }

    public void MoveBoxY()
    { 
        
    }

}

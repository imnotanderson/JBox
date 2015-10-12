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

        foreach (var b in triggerList)
        {
            bool xInBox = (box.CheckMoveBoxX(b, speed));
            bool yInBox = (box.CheckMoveBoxY(b, speed));
            //logic bug:--
            if (xInBox || yInBox)
            {
                box.GetPivotPos(b, box.pos + speed, speed);
                if (xInBox)
                {
                    box.speed.x = 0;
                }
                if (yInBox)
                {
                    box.speed.y = 0;
                }
                return;
            }
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

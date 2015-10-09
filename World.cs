using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class World  {
    
    public List<Box> boxList = new List<Box>();
    public List<Box> triggerList = new List<Box>();

    float g = 1;

    public void Upt(float deltaTime)
    {
        foreach (Box box in boxList)
        {
            box.AddSpeed(-Vector2.up * g+Vector2.right*0.05f);
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

    public void MoveBox(Box box,Vector2 speed)
    {
        Vector2 xSpeed = new Vector2(speed.x, 0);
        Vector2 ySpeed = new Vector2(0, speed.y);

        foreach (var b in triggerList)
        {
            if (box.CheckMoveBoxX(b, speed.x))
                break;
            //if (box.CheckMoveBox(b, xSpeed))
            //    break ;
        }
        foreach (var b in triggerList)
        {
            if (box.CheckMoveBoxY(b, speed.y))
                break;
            //if (box.CheckMoveBox(b, ySpeed))
            //    break;
        }

    }

    public void MoveBoxX()
    { 
    
    }

    public void MoveBoxY()
    { 
        
    }

}

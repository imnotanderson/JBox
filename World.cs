using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class World
{

    #region debug
    public Box testBox = null;
    public List<Box> BoxList { get { return boxList; } }
    public List<Box> TriggerList { get { return triggerList; } }
    #endregion

    List<Box> boxList = new List<Box>();
    List<Box> triggerList = new List<Box>();
    float g = 1;

    #region life cycle

    public void Upt(float deltaTime)
    {
        foreach (Box box in boxList)
        {
            box.ApplyForce(-Vector2.up * g);
            box.Move(deltaTime);
        }
    }
    #endregion

    #region function
    public void AddBox(Box box, bool isTrigger = false)
    {
        box.world = this;
        var list = isTrigger ? triggerList : boxList;
        list.Add(box);
    }

    public void MoveBox(Box box, Vector2 speed)
    {
        Vector2 xSpeed = new Vector2(speed.x, 0);
        Vector2 ySpeed = new Vector2(0, speed.y);

        bool xInBox = false;
        bool yInBox = false;
        List<Box> enterBoxList = new List<Box>();
        foreach (var b in triggerList)
        {
            Box.BoxCheckResult tmXInBoxCheck ;
            Box.BoxCheckResult tmYInBoxCheck ;
            tmXInBoxCheck = (box.CheckMoveBoxX(b, speed));
            tmYInBoxCheck = (box.CheckMoveBoxY(b, speed));
            if (tmXInBoxCheck != Box.BoxCheckResult.OutBox || tmYInBoxCheck != Box.BoxCheckResult.OutBox)
            {
                enterBoxList.Add(b);
            }
			if (tmXInBoxCheck == Box.BoxCheckResult.InBox || tmYInBoxCheck == Box.BoxCheckResult.InBox)
			{
				box.MoveToPivotPos(b, box.pos + speed, speed);
			}
			if (tmXInBoxCheck== Box.BoxCheckResult.InBox) xInBox = true;
            if (tmYInBoxCheck== Box.BoxCheckResult.InBox) yInBox = true;
        }
        box.CheckEnterBox(enterBoxList);
        if (xInBox || yInBox)
        {
            if (xInBox)
            {
                box.SetXSpeed(0);
            }
            else
            {

                box.SetPosXAdd(speed.x);
            }
            if (yInBox)
            {
                box.SetYSpeed(0);
            }
            else
            {
                box.SetPosYAdd(speed.y);
            }
            return;
        }
        box.pos += speed;
    }

    #endregion

    #region debug
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
        if (testBox != null)
        {
            testBox.Draw();
        }
    }
    #endregion

}
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
    float g = 1.3f;

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
            tmXInBoxCheck = (box.CheckMoveBoxX(b, xSpeed));
            tmYInBoxCheck = (box.CheckMoveBoxY(b, ySpeed));
            if (tmXInBoxCheck == Box.BoxCheckResult.OutBox && tmYInBoxCheck == Box.BoxCheckResult.OutBox)
            {
                //enterBoxList.Add(b);
            }
            else if (tmXInBoxCheck == Box.BoxCheckResult.InBox || tmYInBoxCheck == Box.BoxCheckResult.InBox)
            {
                var oldPos = box.pos;
                if (tmXInBoxCheck == Box.BoxCheckResult.InBox)
                {
                    tmXInBoxCheck = (box.CheckMoveBoxX(b, xSpeed));
                    var tmPos = box.pos;
                    box.MoveToPivotPos(b, oldPos + xSpeed, speed);
                    tmPos.x = box.pos.x;
                    box.pos = tmPos;
                }
                if (tmYInBoxCheck == Box.BoxCheckResult.InBox)
                {
                    var tmPos = box.pos;
                    box.MoveToPivotPos(b, oldPos + ySpeed, speed);
//                    tmPos.y = Mathf.RoundToInt( box.pos.y);
                    if (tmPos.y < 1)
                    {
                        //here <1 when bug
                        box.MoveToPivotPos(b, oldPos + ySpeed, speed);
                    }
                    box.pos = tmPos;
                }
                enterBoxList.Add(b);
            }
            else
            {
                enterBoxList.Add(b);
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
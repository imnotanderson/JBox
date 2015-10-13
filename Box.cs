using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum Dir{
none,up,down,left,right,
}

public class Box
{
    const float IGNORE_RANGE = 0.0001f;
    public float mass = 1;
    List<Box> enterBoxList = new List<Box>();
    public Vector2 pos;
    /// <summary>
    /// half width and height--
    /// </summary>
    public float hwidth, hheight;
    public bool lockSpeedX = false;
    public bool lockSpeedY = false;

    public Vector2 speed
    {
        set
        {
            var oldVal = _speed;
            _speed = value;
            if (lockSpeedX) _speed.x = oldVal.y;
            if (lockSpeedY) _speed.y = oldVal.y;
        }
        get
        {
            return _speed;
        }
    }
    Vector2 _speed;
    Vector2 addSpeed = Vector2.zero;
    public World world;


    #region life cycle

    public Box(float x, float y, float width, float height)
    {
        this.pos = new Vector2(x, y);
        this.hwidth = width / 2f;
        this.hheight = height / 2f;
    }

    public Box SetMass(float mass)
    {
        if (mass < 0) return this;
        this.mass = mass;
        return this;
    }


    public virtual void OnOtherEnter(Box other)
    {
        Debug.Log("other box enter");
    }

    public virtual void OnOtherExit(Box other)
    {
        Debug.Log("other box exit");
    }


    #endregion

    #region function
    void AddSpeed()
    {
        this.speed += addSpeed;
        addSpeed = Vector2.zero;
    }
    public void Move(float stepTime)
    {
        AddSpeed();
        world.MoveBox(this, speed * stepTime);
    }
    public void MoveToPivotPos(Box staticBox, Vector2 newPos, Vector2 moveDir)
    {
        pos = newPos;
        float h = this.hheight + staticBox.hheight;
        float w = this.hwidth + staticBox.hwidth;
        Vector2 ravPos = this.pos - staticBox.pos;
        Vector2[] p = new Vector2[0];
        var dir = -moveDir;
        if (dir.x != 0)
        {
            float k = dir.y / dir.x;
            float c = -ravPos.x * k + ravPos.y;
            p = new Vector2[]
            {
                new Vector2(-w,k*-w+c),new Vector2(w,k*w+c),
                new Vector2((h-c)/k,h),new Vector2((-h-c)/k,-h)
            };
        }
        else
        {
            ravPos.y = ravPos.y > 0 ? h : -h;
            pos = staticBox.pos + ravPos;
        }
        foreach (var v in p)
        {
            if (CheckPointInRayAndBox(ravPos, dir, w, h, v))
            {
                pos = staticBox.pos + v;
                return;
            }
        }
    }
    public Box Clone()
    {
        Box cloneBox = new Box(pos.x, pos.y, hwidth * 2, hheight * 2);
        return cloneBox;
    }

    public void ApplyForce(Vector2 force)
    {
        if (mass == 0) return;
        addSpeed += force / mass;
    }

    public void SetXSpeed(float x)
    {
        var val = this.speed;
        val.x = x;
        this.speed = val;
    }
    public void SetYSpeed(float y)
    {
        var val = this.speed;
        val.y = y;
        this.speed = val;
    }


    #endregion

    #region calc
    public void CheckEnterBox(List<Box> newEnterBoxList)
    {
        //calc
        List<Box> oldEnterBoxList = this.enterBoxList;
        List<Box> resultExitBoxList = new List<Box>();
        List<Box> resultEnterBoxList = new List<Box>();
        foreach (var b in oldEnterBoxList)
        {
            if (newEnterBoxList.Contains(b) == false)
            {
                resultEnterBoxList.Add(b);
                b.OnOtherEnter(this);
            }
        }
        foreach (var b in newEnterBoxList)
        {
            if (oldEnterBoxList.Contains(b) == false)
            {
                resultExitBoxList.Add(b);
                b.OnOtherExit(this);
            }
        }
        this.enterBoxList = newEnterBoxList;
    }

    public bool CheckMoveBoxX(Box staticBox, Vector2 speed)
    {
        float xSpeed = speed.x;
        var newPos = pos + new Vector2(xSpeed, 0);
        bool isInStaticBox = NewPosInStaticBox(newPos, staticBox);
        if (isInStaticBox)
        {
            return true;
        }
        return false;
    }
    public bool CheckMoveBoxY(Box staticBox, Vector2 speed)
    {
        float ySpeed = speed.y;
        var newPos = pos + new Vector2(0, ySpeed);
        bool isInStaticBox = NewPosInStaticBox(newPos, staticBox);
        if (isInStaticBox)
        {
            return true;
        }
        return false;
    }
    bool CheckPointInRayAndBox(Vector2 rayPoint, Vector2 rayDir, float boxHWidth, float boxHHeight, Vector2 point)
    {
        if (Mathf.Abs(point.x) > boxHWidth) return false;
        if (Mathf.Abs(point.y) > boxHHeight) return false;
        if (rayDir.x >= 0 && rayPoint.x > point.x) return false;
        if (rayDir.y >= 0 && rayPoint.y > point.y) return false;
        if (rayDir.x <= 0 && rayPoint.x < point.x) return false;
        if (rayDir.y <= 0 && rayPoint.y < point.y) return false;
        return true;
    }

    public static Vector2 GetPosInBoxLineByDir(float w, float h, Vector2 pos, Vector2 dir)
    {
        Vector2 result = Vector2.zero;
        if (dir == Vector2.zero)
        {
            dir = Vector2.up;
        }
        Vector2 offset = Vector2.zero;
        if (dir.x != 0)
        {
            offset = (dir / Mathf.Abs(dir.x)) * w;
            if (Mathf.Abs(offset.y) < h)
            {
                result = pos + offset;
                return result;
            }
        }
        offset = (dir / Mathf.Abs(dir.y) * h);
        result = pos + offset;
        return result;
    }

    bool NewPosInStaticBox(Vector2 newPos, Box staticBox)
    {
        bool isInStaticBox = false;
        float newMinY = newPos.y - this.hheight;
        float newMaxY = newPos.y + this.hheight;
        float staticMinY = staticBox.pos.y - staticBox.hheight;
        float staticMaxY = staticBox.pos.y + staticBox.hheight;

        float newMinX = newPos.x - this.hwidth;
        float newMaxX = newPos.x + this.hwidth;
        float staticMinX = staticBox.pos.x - staticBox.hwidth;
        float staticMaxX = staticBox.pos.x + staticBox.hwidth;
        if ((newMinY > staticMaxY - IGNORE_RANGE || newMaxY - IGNORE_RANGE < staticMinY) ||
            (newMinX > staticMaxX - IGNORE_RANGE || newMaxX - IGNORE_RANGE < staticMinX))
        {
            isInStaticBox = false;
        }
        else
        {
            isInStaticBox = true;
        }
        return isInStaticBox;
    }

    public bool PointInBox(Vector2 p)
    {
        var ps = GetPoints();
        if (ps[0].x <= p.x && p.x <= ps[2].x &&
            ps[2].y <= p.y && p.y <= ps[0].y)
            return true;
        return false;
    }
    public Vector2[] GetPoints(Dir dir = Dir.none)
    {
        Vector2[] p = new Vector2[]{
            pos+new Vector2(-hwidth,hheight),pos+new Vector2(hwidth,hheight),
            pos+new Vector2(hwidth,-hheight), pos+new Vector2(-hwidth,-hheight),
        };
        return p;
    }
    #endregion

    #region debug
    public void Draw()
    {
        Vector2[] p = new Vector2[]{
            pos+new Vector2(-hwidth,hheight),pos+new Vector2(hwidth,hheight),
            pos+new Vector2(hwidth,-hheight), pos+new Vector2(-hwidth,-hheight),
            pos+new Vector2(-hwidth,hheight)        
        };
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(p[i], p[i + 1]);
        }
    }
    #endregion
}
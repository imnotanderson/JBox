using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum Dir
{
    none, up, down, left, right,
}

public class Box
{
	public object data;
    public string name = "";
    const float IGNORE_RANGE = 0f;
	const float ON_BOX_CHECK_PARAM = 0.02f;
    public float mass = 1;
	public Vector2 force = Vector2.zero;
    List<Box> enterBoxList = new List<Box>();
    public Vector2 pos
    {
        set
        {
            _pos = value;
            if (onPosChange != null)
                onPosChange(_pos);
        }
        get { return this._pos; }
    }
    Vector2 _pos;
    /// <summary>
    /// half width and height--
    /// </summary>
    public float hwidth, hheight;
    public bool lockSpeedX = false;
    public bool lockSpeedY = false;
    Action<Box> onEnterOtherBox;
    Action<Box> onExitOtherBox;
    Action<Vector2> onPosChange;
    public Vector2 speed
    {
        set
        {
            var oldVal = _speed;
            if (lockSpeedX) value.x = oldVal.x;
            if (lockSpeedY) value.y = oldVal.y;
            _speed = value;
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

    public Box(float x, float y, float width, float height, string name = "")
    {
        this.name = name;
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

    public Box SetEnterOtherBoxCallback(Action<Box> callback)
    {
        this.onEnterOtherBox = callback;
        return this;
    }

    public Box SetExitOtherBoxCallback(Action<Box> callback)
    {
        this.onExitOtherBox = callback;
        return this;
    }

    public Box SetOnPosChange(Action<Vector2> onPosChange)
    {
        this.onPosChange = onPosChange;
        return this;
    }
    public virtual void OnEnterOther(Box other)
    {
        if (onEnterOtherBox != null)
            onEnterOtherBox(other);
    }

    public virtual void OnExitOther(Box other)
    {
        if (onExitOtherBox != null)
            onExitOtherBox(other);
    }


    #endregion

    #region function
    public void SetPosX(float x)
    {
        var pos = this.pos;
        pos.x = x;
        this.pos = pos;
    }

    public void SetPosXAdd(float x)
    {
        var pos = this.pos;
        pos.x += x;
        this.pos = pos;
    }

    public void SetPosYAdd(float y)
    {
        var pos = this.pos;
        pos.y += y;
        this.pos = pos;
    }

    public void SetPosY(float y)
    {
        var pos = this.pos;
        pos.y = y;
        this.pos = pos;
    }

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
    
    public Box Clone()
    {
        Box cloneBox = new Box(pos.x, pos.y, hwidth * 2, hheight * 2);
        return cloneBox;
    }

    public void ApplyForce(Vector2 force)
    {
		this.force += force;
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
				pos += moveDir.normalized * IGNORE_RANGE*1f;
                return;
            }
        }
		//pos += moveDir.normalized * 0.001f;
    }
    
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
                this.OnExitOther(b);
            }
        }
        foreach (var b in newEnterBoxList)
        {
            if (oldEnterBoxList.Contains(b) == false)
            {
                resultExitBoxList.Add(b);
                this.OnEnterOther(b);
            }
        }
        this.enterBoxList = newEnterBoxList;
    }

	public enum BoxCheckResult
	{
		OutBox,OnBox,InBox
	}

	//return 3 state: out box need move,on box dont need pupop, in box need pipop
	public BoxCheckResult CheckMoveBoxX(Box staticBox, Vector2 speed)
    {
        float xSpeed = speed.x;
        var newPos = pos + new Vector2(xSpeed, 0);
        BoxCheckResult checkResult = NewPosInStaticBox(newPos, staticBox);

		return checkResult;
    }
	public BoxCheckResult CheckMoveBoxY(Box staticBox, Vector2 speed)
    {
        float ySpeed = speed.y;
        var newPos = pos + new Vector2(0, ySpeed);
		BoxCheckResult checkResult = NewPosInStaticBox(newPos, staticBox);
		return checkResult;
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

    BoxCheckResult NewPosInStaticBoxX(Vector2 newPos, Box staticBox)
    {
        BoxCheckResult result;
        float newMinY = newPos.y - this.hheight;
        float newMaxY = newPos.y + this.hheight;
        float staticMinY = staticBox.pos.y - staticBox.hheight;
        float staticMaxY = staticBox.pos.y + staticBox.hheight;

        float newMinX = newPos.x - this.hwidth;
        float newMaxX = newPos.x + this.hwidth;
        float staticMinX = staticBox.pos.x - staticBox.hwidth;
        float staticMaxX = staticBox.pos.x + staticBox.hwidth;
                   //up                     //down
        if (
                    //right                 //left
			(newMinX > staticMaxX || newMaxX < staticMinX)) {
			result = BoxCheckResult.OutBox;
		} else if (
			(newMinX > staticMaxX - ON_BOX_CHECK_PARAM || newMaxX < staticMinX + ON_BOX_CHECK_PARAM)) 
		{
			result = BoxCheckResult.OnBox;
		}
        else
        {
            result = BoxCheckResult.InBox;
        }
        return result;
    }

    BoxCheckResult NewPosInStaticBoxY(Vector2 newPos, Box staticBox)
    {
        BoxCheckResult result;
        float newMinY = newPos.y - this.hheight;
        float newMaxY = newPos.y + this.hheight;
        float staticMinY = staticBox.pos.y - staticBox.hheight;
        float staticMaxY = staticBox.pos.y + staticBox.hheight;

        float newMinX = newPos.x - this.hwidth;
        float newMaxX = newPos.x + this.hwidth;
        float staticMinX = staticBox.pos.x - staticBox.hwidth;
        float staticMaxX = staticBox.pos.x + staticBox.hwidth;
        //up                     //down
        if ((newMinY > staticMaxY || newMaxY < staticMinY))
        {
            result = BoxCheckResult.OutBox;
        }
        else if ((newMinY > staticMaxY - ON_BOX_CHECK_PARAM || newMaxY < staticMinY + ON_BOX_CHECK_PARAM) )
        {
            result = BoxCheckResult.OnBox;
        }
        else
        {
            result = BoxCheckResult.InBox;
        }
        return result;
    }

    BoxCheckResult NewPosInStaticBox(Vector2 newPos, Box staticBox)
    {
        BoxCheckResult result;
        float newMinY = newPos.y - this.hheight;
        float newMaxY = newPos.y + this.hheight;
        float staticMinY = staticBox.pos.y - staticBox.hheight;
        float staticMaxY = staticBox.pos.y + staticBox.hheight;

        float newMinX = newPos.x - this.hwidth;
        float newMaxX = newPos.x + this.hwidth;
        float staticMinX = staticBox.pos.x - staticBox.hwidth;
        float staticMaxX = staticBox.pos.x + staticBox.hwidth;
        //up                     //down
        if ((newMinY > staticMaxY || newMaxY < staticMinY) ||
            //right                 //left
            (newMinX > staticMaxX || newMaxX < staticMinX))
        {
            result = BoxCheckResult.OutBox;
        }
        else if ((newMinY > staticMaxY - ON_BOX_CHECK_PARAM || newMaxY < staticMinY + ON_BOX_CHECK_PARAM) ||
          (newMinX > staticMaxX - ON_BOX_CHECK_PARAM || newMaxX < staticMinX + ON_BOX_CHECK_PARAM))
        {
            result = BoxCheckResult.OnBox;
        }
        else
        {
            result = BoxCheckResult.InBox;
        }
        return result;
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
#if UNITY_EDITOR
        Vector2[] p = new Vector2[]{
            pos+new Vector2(-hwidth,hheight),pos+new Vector2(hwidth,hheight),
            pos+new Vector2(hwidth,-hheight), pos+new Vector2(-hwidth,-hheight),
            pos+new Vector2(-hwidth,hheight)
        };
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(p[i], p[i + 1]);
        }
        Handles.Label(pos, name);
#endif
    }
    #endregion
}
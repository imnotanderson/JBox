using UnityEngine;
using System.Collections;

public enum Dir{
none,up,down,left,right,
}

public class Box
{
	const float IGNORE_RANGE = 0.0001f;
    public float mass = 0;
    public Vector2 pos;
    public float hwidth, hheight;
    public Vector2 speed;
    public World world;

    public Box(float x, float y, float width, float height)
    {
        this.pos = new Vector2(x, y);
        this.hwidth = width / 2f;
        this.hheight = height / 2f;
    }

    public void AddSpeed(Vector2 addSpeed)
    {
        this.speed += addSpeed;
    }

    public void Move(float stepTime)
    {
        world.MoveBox(this, speed * stepTime);
    }

    public bool CheckMoveBoxX(Box staticBox, Vector2 speed)
    {
        float xSpeed = speed.x;
		var newPos = pos + new Vector2(xSpeed, 0);
		bool isInStaticBox = NewPosInStaticBox(newPos,staticBox);
		//isInStaticBox = false;
		if (isInStaticBox)
        {
            //**
            pos = GetPivotPos(staticBox, newPos,speed);
            //**
			this.speed.x = 0;
            return true;
        }
        pos = newPos;
        return false;
    }


    public bool CheckMoveBoxY(Box staticBox, Vector2 speed)
    {
        float ySpeed = speed.y;
        var newPos = pos + new Vector2(0, ySpeed);
        bool isInStaticBox = NewPosInStaticBox(newPos, staticBox);
        if (isInStaticBox)
        {
            //**
            pos = GetPivotPos(staticBox, newPos, speed);
            //**
            this.speed.y = 0;
            return true;
        }
        pos = newPos;
        return false;
    }

    //Get Pivot Pos--
    Vector2 GetPivotPos(Box staticBox, Vector2 newPos,Vector2 moveDir)
    {
        if (staticBox.hwidth <= 0) return newPos;
        Vector2 tmCenter = newPos;
        Vector2 p = staticBox.pos - newPos;
        Vector2 dir = -moveDir;
        dir = dir.normalized;
        float absX = Mathf.Abs(dir.x);
        float absY = Mathf.Abs(dir.y);
        float heightSum = (staticBox.hheight + this.hheight);
        float widthSum = (staticBox.hwidth + this.hwidth) ;
        //up and down--
        //err:
        if (absX==0 || absY / absX > Mathf.Abs(staticBox.hheight / staticBox.hwidth))
        {
            float y = staticBox.pos.y + (dir.y >= 0 ? heightSum : -heightSum);
            tmCenter.y = y;
        }
        else
        //left and right--
        {
            float x = staticBox.pos.x + (dir.x >= 0 ? widthSum : -widthSum);
            tmCenter.x = x;
        }
        return tmCenter;
        return moveDir;
    }

	bool NewPosInStaticBox(Vector2 newPos,Box staticBox)
	{
		bool isInStaticBox = false;
		float newMinY = newPos.y-this.hheight;
		float newMaxY = newPos.y+this.hheight;
		float staticMinY = staticBox.pos.y-staticBox.hheight;
		float staticMaxY = staticBox.pos.y+staticBox.hheight;
		
		float newMinX = newPos.x - this.hwidth;
		float newMaxX = newPos.x + this.hwidth;
		float staticMinX = staticBox.pos.x - staticBox.hwidth;
		float staticMaxX = staticBox.pos.x + staticBox.hwidth;
		if ((newMinY > staticMaxY-IGNORE_RANGE || newMaxY-IGNORE_RANGE < staticMinY) ||
		    (newMinX>staticMaxX-IGNORE_RANGE || newMaxX-IGNORE_RANGE<staticMinX)) {
			isInStaticBox = false;
		} else {
			isInStaticBox = true;
		}
		return isInStaticBox;
	}


    public Box Clone()
    {
        Box cloneBox = new Box(pos.x, pos.y, hwidth * 2, hheight * 2);
        return cloneBox;
    }

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

    public Vector2[] GetPoints(Dir dir = Dir.none)
    {
        Vector2[] p = new Vector2[]{
            pos+new Vector2(-hwidth,hheight),pos+new Vector2(hwidth,hheight),
            pos+new Vector2(hwidth,-hheight), pos+new Vector2(-hwidth,-hheight),
        };
        return p;
    }

    public bool PointInBox(Vector2 p)
    {
        var ps = GetPoints();
        if (ps[0].x <= p.x && p.x <= ps[2].x &&
            ps[2].y <= p.y && p.y <= ps[0].y)
            return true;
        return false;
    }

}
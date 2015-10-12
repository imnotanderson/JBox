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

    //Get Pivot Pos--
    public void GetPivotPos(Box staticBox, Vector2 newPos, Vector2 moveDir)
    {
        pos = newPos;
        float h = this.hheight+staticBox.hheight;
        float w = this.hwidth + staticBox.hwidth;
        Vector2 ravPos = this.pos - staticBox.pos;
        Vector2[]p = new Vector2[0];
        var dir = -moveDir;
        if(dir.x!=0)
        {
            float k = dir.y/dir.x;
            float c = -ravPos.x*k+ravPos.y;
            p = new Vector2[]
            {
                new Vector2(-w,k*-w+c),new Vector2(w,k*w+c),
                new Vector2((h-c)/k,h),new Vector2((-h-c)/k,-h)
            };
        }
        else
        {
            ravPos.y = ravPos.y>0?h:-h;
            pos = staticBox.pos+ravPos;
        }
        foreach (var v in p)
        {
            if(CheckPointInRayAndBox(ravPos,dir,w,h,v))
            {
                pos = staticBox.pos + v;
                return;   
            }
        }
    }
    
    bool CheckPointInRayAndBox(Vector2 rayPoint,Vector2 rayDir,float boxHWidth,float boxHHeight,Vector2 point)
    {
        if(Mathf.Abs(point.x)>boxHWidth)return false;
        if(Mathf.Abs(point.y)>boxHHeight)return false;
        if(rayDir.x>=0 && rayPoint.x>point.x)return false;
        if(rayDir.y>=0 && rayPoint.y>point.y)return false;
        if(rayDir.x<=0 && rayPoint.x<point.x)return false;
        if(rayDir.y<=0 && rayPoint.y<point.y)return false;
        return true;
    }

   public static Vector2 GetPosInBoxLineByDir(float w,float h,Vector2 pos,Vector2 dir)
    {
        Vector2 result = Vector2.zero;
        if(dir==Vector2.zero)
        {
            dir = Vector2.up;
        }
                Vector2 offset = Vector2.zero;
        if(dir.x!=0)
        {
            offset = (dir/ Mathf.Abs(dir.x))*w;
            if(Mathf.Abs(offset.y)<h)
            {
                result = pos+offset;
                return result;
            }   
        }
        offset = (dir/Mathf.Abs(dir.y)*h);
        result = pos+offset;
        return result;
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
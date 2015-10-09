using UnityEngine;
using System.Collections;

public enum Dir{
none,up,down,left,right,
}

public class Box
{
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

    /// <summary>
    /// check hit and return hit pos;
    /// </summary>
    /// <param name="staticBox"></param>
    /// <param name="pos"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public bool CheckMoveBox(Box staticBox, Vector2 speed)
    {
        if (speed.Equals(Vector2.zero))
        {
            return false;
        }
        if (Mathf.Abs(this.pos.x - staticBox.pos.x) - staticBox.hwidth - this.hwidth < Mathf.Abs(speed.x) &&
            Mathf.Abs(this.pos.y - staticBox.pos.y) - staticBox.hheight - this.hheight < Mathf.Abs(speed.y))
        {
            if (speed.normalized.x != 0)
            {
                speed = staticBox.pos - this.pos;
                speed = speed.normalized * (staticBox.hwidth + this.hwidth) / speed.normalized.x;
                this.speed.x = 0;
            }
            else
            {
                speed = staticBox.pos - this.pos;
                speed = speed.normalized * (staticBox.hheight + this.hheight) / speed.normalized.y;
                this.speed.y = 0;
            }
            pos = staticBox.pos + speed;
            return true;
        }
        pos += speed;
        return false;
    }

    public bool CheckMoveBoxX(Box staticBox, float xSpeed)
    {
        var newPos = pos + new Vector2(xSpeed, 0);
        if (staticBox.PointInBox(newPos))
        {
            //**
            speed = staticBox.pos - this.pos;
            speed = speed.normalized * (staticBox.hwidth + this.hwidth) / speed.normalized.x;
            this.speed.x = 0;
            speed.y = 0;
            pos = staticBox.pos + speed;
            //**
            return true;
        }
        pos = newPos;
        return false;
    }
    public bool CheckMoveBoxY(Box staticBox, float ySpeed)
    {
        var newPos = pos + new Vector2(0, ySpeed);
        if (staticBox.PointInBox(newPos))
        {
            //**
            //**
            return true;
        }
        pos = newPos;
        return false;
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
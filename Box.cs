using UnityEngine;
using System.Collections;



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
        Box hitBox;
        if(world.CheckHit(this,out hitBox)==false)
        pos += speed * stepTime;
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

    public Vector2[] GetPoints()
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
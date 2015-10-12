using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    World world = null;
    Box testBox;
    void Start()
    {
        world = new World();
        world.AddBox(new Box(0.1f, 5, 1, 1));
        world.AddBox(new Box(0, -2, 50, 3), true);
        testBox = new Box(0,0,2,2);
        world.testBox = testBox;
        world.AddBox(new Box(20, -9, 30, 1f), true);
        world.AddBox(new Box(35, -15, 30, 1f), true);
        world.AddBox(new Box(40, -9, 1, 13), true);
    }

    public void OnDrawGizmos()
    {
        if (world == null) return;
        world.Draw();
    }

    void Update()
    {
        if (world == null) return;
        world.Upt(Time.deltaTime);
        if(Input.GetKey(KeyCode.W))TestMove(Vector2.up);
        if(Input.GetKey(KeyCode.S))TestMove(-Vector2.up);
        if(Input.GetKey(KeyCode.A))TestMove(-Vector2.right);
        if(Input.GetKey(KeyCode.D))TestMove(Vector2.right);
        
    }

    void TestMove(Vector2 speed)
    {
        world.MoveBox(testBox,speed*Time.deltaTime);
    }
    
    void OnGUI()
    {
        if (world == null) return;
        var b1 = world.boxList.Count > 0 ? world.boxList[0] : null;
        var b2 = world.triggerList.Count > 0 ? world.triggerList[0] : null;
        GUILayout.Label(string.Format("test:({0},{1})", testBox.pos.x, testBox.pos.y));
        GUILayout.Label(string.Format("b2:({0},{1})", b2.pos.x, b2.pos.y));
        if(GUILayout.Button("Test"))
        {
            Box staticBox = world.triggerList[0];
            var pos = Box.GetPosInBoxLineByDir(staticBox.hwidth+testBox.hwidth,staticBox.hheight+testBox.hheight,staticBox.pos,testBox.pos-staticBox.pos);
           testBox.pos = pos;
           // testBox.GetPivotPos(world.triggerList[0],new Vector2(0,0),Vector2.right);
        }
    }
}

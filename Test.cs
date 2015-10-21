using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
	public float testSpeed = 10;
    World world = null;
    Box testBox;
    void Start()
    {
        world = new World();
        var box = new Box(0, 5, 1, 1, "test");
        //box.SetXSpeed(2);
		box.SetEnterOtherBoxCallback ((b) => {
			Debug.Log("Enter "+b.name);
		});
        box.SetExitOtherBoxCallback((b) =>
        {
            Debug.Log("Exit " + b.name);
        }); 
        testBox = box;
		world.AddBox (box);
//        box.lockSpeedX = true;
		for (int i = 0; i<100; i++) {
			world.AddBox(new Box(0+i, 0, 1, 1, ""+i), true);
		}
//		world.AddBox(box);
//        world.AddBox(new Box(0, -2, 50, 3, "2"), true);
//        testBox = new Box(0, 0, 2, 2, "3");
//        world.testBox = testBox;
//        world.AddBox(new Box(5, 0, 3, 3, "4"), true);
//        world.AddBox(new Box(35, -15, 30, 1f, "5"), true);
//        world.AddBox(new Box(40, -9, 1, 13, "6"), true);
    }

    public void OnDrawGizmos()
    {
        if (world == null) return;
        world.Draw();
    }

    void Update()
    {
        testBox.SetXSpeed(testBox.speed.x + 0.1f);
        if (world == null) return;
        world.Upt(Time.deltaTime);
        if (Input.GetKey(KeyCode.W)) TestMove(Vector2.up);
        if (Input.GetKey(KeyCode.S)) TestMove(-Vector2.up);
        if (Input.GetKey(KeyCode.A)) TestMove(-Vector2.right);
        if (Input.GetKey(KeyCode.D)) TestMove(Vector2.right);

        if (Input.GetKeyUp(KeyCode.Q)) ApplyForceTest();


    }
    public Vector2 force;
    void ApplyForceTest()
    {
        world.BoxList[0].ApplyForce(force);
    }

    void TestMove(Vector2 speed)
    {
		world.MoveBox(testBox, testSpeed* speed * Time.deltaTime);
    }

    void OnGUI()
    {
        GUILayout.Label(testBox.speed.x.ToString());
        if (world == null) return;
        var b1 = world.BoxList.Count > 0 ? world.BoxList[0] : null;
        var b2 = world.TriggerList.Count > 0 ? world.TriggerList[0] : null;
//        GUILayout.Label(string.Format("test:({0},{1})", testBox.pos.x, testBox.pos.y));
        GUILayout.Label(string.Format("b2:({0},{1})", b2.pos.x, b2.pos.y));
        if (GUILayout.Button("Test"))
        {
            Box staticBox = world.TriggerList[0];
            var pos = Box.GetPosInBoxLineByDir(staticBox.hwidth + testBox.hwidth, staticBox.hheight + testBox.hheight, staticBox.pos, testBox.pos - staticBox.pos);
            testBox.pos = pos;
        }
    }
}

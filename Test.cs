using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    World world = null;

    void Start()
    {
        world = new World();
		world.AddBox(new Box(0, 5, 1, 1));
        world.AddBox(new Box(0, -2, 50, 1f), true);
        world.AddBox(new Box(20, -9, 30, 1f), true);
        world.AddBox(new Box(35, -15, 30, 1f), true);
        world.AddBox(new Box(40, 35, 1, 110f), true);
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
    }

    void OnGUI()
    {
        if (world == null) return;
        var b1 = world.boxList.Count > 0 ? world.boxList[0] : null;
        var b2 = world.triggerList.Count > 0 ? world.triggerList[0] : null;
        GUILayout.Label(string.Format("b1:({0},{1})", b1.pos.x, b1.pos.y));
        GUILayout.Label(string.Format("b2:({0},{1})", b2.pos.x, b2.pos.y));
    }
}

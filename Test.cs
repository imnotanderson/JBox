using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    World world = null;

    void Start()
    {
        world = new World();
        world.AddBox(new Box(0, 5, 1, 1));
        world.AddBox(new Box(0, -2, 10, 1f),true);
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
}

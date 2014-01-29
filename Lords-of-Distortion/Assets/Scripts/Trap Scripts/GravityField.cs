using UnityEngine;
using System.Collections;

public class GravityField : Power {

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
        player.rigidbody2D.gravityScale = -1;
    }

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {
    }

    public override void PowerActionExit(GameObject player, Controller2D controller)
    {
        player.rigidbody2D.gravityScale = 1;
    }
  
    void OnDestroy()
    {
        Debug.Log("Destroyed");
        GameObject user = GameObject.FindGameObjectWithTag("Player");
        if (user != null)
            user.rigidbody2D.gravityScale = 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death
{
    private GameObject player;
    private Animator fadeAnimator;

    private Vector2 checkPoint = Vector2.zero;


    public Death(GameObject _p)
    {
        player = _p;
    }
    
    public void Respawn()
    {
        player.transform.position = checkPoint;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void NewCheckPoint(Vector2 newCheckPoint)
    {
        checkPoint = newCheckPoint;
    }
}

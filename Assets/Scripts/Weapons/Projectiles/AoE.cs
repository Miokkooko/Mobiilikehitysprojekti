using System.Collections.Generic;
using UnityEngine;

public class AoE : Projectile
{
    public override void Start()
    {
        Destroy(gameObject, 0.5f);
    }

}

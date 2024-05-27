using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationControls : MonoBehaviour
{
    public Enemy Enemy;
    
    public void AnimationEnd()
    {
        Enemy.AnimationEnd();
    }
}

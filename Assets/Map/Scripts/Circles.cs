using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circles {

    public float[] circle_now;
    public float[] circle_bef;
    public float shrinkTime;

    public Circles(float[] now,float[] bef,float time) {
        circle_now = now;
        circle_bef = bef;
        shrinkTime = time;
    }
}

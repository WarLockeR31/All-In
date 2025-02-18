using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollTheBall : MonoBehaviour
{

    public float StartingSpeed = 0;
    public float currentSpeed;
    public Transform pivot;
    public Transform ball;

    private Vector3 newPoint;

    public bool stopped;



    private Func<double, double> dx = x => .2*Math.Sin(Math.Pow(6f * Math.Sin(2 * x), 2f));
    private Func<double, double> dy = x => .2*Math.Sin(Math.Tan(x));
    void Start()
    {
        stopped = true;
        
    }


    // Update is called once per frame
    void Update()
    {
        if (stopped) return;
        ball.position = ball.position+new Vector3((float)dx(currentSpeed), (float)dy(currentSpeed));
        ball.RotateAround(pivot.position, Vector3.forward, currentSpeed * Time.deltaTime);
        ball.RotateAround(ball.position, Vector3.forward, currentSpeed/2 * Time.deltaTime);
        pivot.RotateAround(pivot.position, Vector3.forward, -currentSpeed * Time.deltaTime);
    }


    private void FixedUpdate()
    {
        if (currentSpeed>0)
        {
            currentSpeed-=0.2f;
        }
        else
        {
            currentSpeed = 0;
            stopped = true;
        }
    }
}

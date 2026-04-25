using System;

public struct Transform
{
    public float x;
    public float y;

    public float scaleX;
    public float scaleY;

    public float angle;

    //  Constructor

    public Transform(float x, float y)
    {
        this.x = x;
        this.y = y;

        this.scaleX = 1f;
        this.scaleY = 1f;
        this.angle = 0f;
    }
}

using System;

public class BulletPool
{
    private List<Bullet> bullets = new List<Bullet>();

    public Bullet GetBullet(Vector2f position, float angle, Vector2f shipVelocity)
    {
        foreach (var bullet in bullets)
        {
            if (!bullet.Active)
            {
                bullet.Init(position, angle, shipVelocity);
                return bullet;
            }
        }

        Bullet newBullet = new Bullet(position, angle, shipVelocity);
        bullets.Add(newBullet);
        return newBullet;
    }

    public void Update(float deltaTime)
    {
        foreach (var bullet in bullets)
        {
            bullet.Update(deltaTime);
        }
    }

    public void Draw()
    {
        foreach (var bullet in bullets)
        {
            if (bullet.Active)
                bullet.Draw();
        }
    }
}

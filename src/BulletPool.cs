using EngineGDI;
using System;
using System.Collections.Generic;

public class BulletPool
{
    public List<Bullet> availableBullets = new List<Bullet>();
    public List<Bullet> activeBullets = new List<Bullet>();

    public List<Bullet> ActiveBullets => activeBullets;

    public BulletPool(int MaxDesiredSize = 100)
    {
        for (int i = 0; i < MaxDesiredSize; i++)
        {
            Bullet newBullet = new Bullet(); //Esto afecta al script bullet y hay que cambiar cosas
            availableBullets.Add(new Bullet());
            

        }
    }

    public Bullet GetBullet(Vector2f position, float angle, Vector2f shipVelocity)
    {
        Bullet bulletToUse; 

        if (availableBullets.Count > 0)
        {
            bulletToUse = availableBullets[0];
            availableBullets.RemoveAt(0);
        }
        else 
        {
            bulletToUse = new Bullet(); 
        }

        bulletToUse.OnDeactivate -= OnBulletDeactivated;
        bulletToUse.OnDeactivate += OnBulletDeactivated;

        bulletToUse.Init(position, angle, shipVelocity);

        activeBullets.Add(bulletToUse);
        return bulletToUse;
    }

    private void OnBulletDeactivated(Bullet bullet)
    {
        if (activeBullets.Contains(bullet))
        {
            activeBullets.Remove(bullet);
        }
        if (!availableBullets.Contains(bullet))
        {
            availableBullets.Add(bullet);
        }
    }

    public void Update(float deltaTime)
    {
        for (int i = activeBullets.Count - 1; i >= 0; i--)
        {
            activeBullets[i].Update(deltaTime);
        }
    }

    public void Draw()
    {
        foreach (var bullet in activeBullets)
        {
                bullet.Draw();
        }
    }
}

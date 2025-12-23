using UnityEngine;

public class SampleGun : Gun
{
    public void Start()
    {
        maxAmmo = 12;
        ammo = maxAmmo;
        baseCoolDown = 0.2f;
    }
    public override void Shoot() => base.Shoot();


}

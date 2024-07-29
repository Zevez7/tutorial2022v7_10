using System;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class WandControl : XRGrabInteractable
{
    // Start is called before the first frame update
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnPoint;
    private bool isFiring;

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab,
            projectileSpawnPoint.position,
            projectileSpawnPoint.rotation);
            // projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * 200);
        }
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
    }
}

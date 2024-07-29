
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
public class ExplosiveDevice : XRGrabInteractable

{
    public UnityEvent OnDetonated;
    private bool isActived;
    // Start is called before the first frame update
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (args.interactableObject.transform.GetComponent<XRGrabInteractable>() != null)
        {
            isActived = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isActived && other.gameObject.GetComponent<WandProjectile>() != null)
        {
            OnDetonated?.Invoke();
        }
    }


}

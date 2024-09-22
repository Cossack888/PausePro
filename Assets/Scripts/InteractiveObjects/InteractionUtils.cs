using UnityEngine;

public static class InteractionUtils
{
    public static bool ObjectProneToInteraction(GameObject gameObject)
    {
        if (gameObject.GetComponent<BreakableObject>() != null || gameObject.GetComponent<InteractionObject>() != null || gameObject.GetComponent<TurnOff>() != null)
            return true;
        else return false;
    }
}
using UnityEngine;

public static class InteractionUtils
{
    public static bool ObjectProneToInteraction(GameObject obj)
    {

        //     gameObject.GetComponent<InteractionObject>() != null ||
        //     gameObject.GetComponent<TurnOff>() != null ||
        //     gameObject.GetComponent<Glyph>() != null
        //     )
        //     return true;
        // else return false;

        return obj.GetComponent<IProneToInteraction>() != null;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class GlyphFinalInteractionEffect : IInteractionEffect
{
    List<GameObject> collidingObjects;
    public GlyphFinalInteractionEffect(List<GameObject> collidingObjects)
    {
        this.collidingObjects = collidingObjects;
    }

    public void ApplyEffect()
    {
        Debug.Log("Glyph final action on " + collidingObjects);
        foreach (GameObject collidingObject in collidingObjects)
        {
            Debug.Log("victim(final): " + collidingObject.name);
        }
    }
}
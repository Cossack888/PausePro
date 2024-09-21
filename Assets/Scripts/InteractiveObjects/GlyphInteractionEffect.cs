using System.Collections.Generic;
using UnityEngine;

public class GlyphInitialInteractionEffect : IInteractionEffect
{
    List<GameObject> collidingObjects;
    public GlyphInitialInteractionEffect(List<GameObject> collidingObjects)
    {
        this.collidingObjects = collidingObjects;
    }

    public void ApplyEffect()
    {

        Debug.Log("Glyph initial action on " + collidingObjects);
        foreach (GameObject collidingObject in collidingObjects)
        {
            Debug.Log("victim: " + collidingObject.name);
        }
    }
}
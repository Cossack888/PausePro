using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glyph : MonoBehaviour, IProneToInteraction
{
    private Boolean active = false;
    private List<GameObject> collidingObjects = new List<GameObject>();

    public bool Active { get => active; set => active = value; }

    void OnCollisionEnter(Collision collision)
    {
        //if (InteractionUtils.ObjectProneToInteraction(collision.gameObject))
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collidingObjects.Add(collision.gameObject);
            Debug.Log("Glyph: Colliding object added: " + collision.gameObject.name);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //collidingObjects.Remove(collision.gameObject);
            Debug.Log("Glyph: Colliding object removed: " + collision.gameObject.name);
        }
    }

    public IInteractionEffect InitialAction()
    {
        List<GameObject> collidingObjectsCopy = new();
        collidingObjects.ForEach(collidingObject => collidingObjectsCopy.Add(collidingObject));

        return new GlyphInitialInteractionEffect(collidingObjectsCopy);
    }

    public IInteractionEffect FinalAction()
    {
        List<GameObject> collidingObjectsCopy = new();
        collidingObjects.ForEach(collidingObject => collidingObjectsCopy.Add(collidingObject));

        return new GlyphFinalInteractionEffect(collidingObjectsCopy);
    }

}

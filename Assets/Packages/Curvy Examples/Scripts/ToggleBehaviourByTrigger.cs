// =====================================================================
// Copyright 2013-2015 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================

using UnityEngine;
using System.Collections;

public class ToggleBehaviourByTrigger : MonoBehaviour
{
    public Behaviour UIElement;

    void OnTriggerEnter()
    {
        if (UIElement)
            UIElement.enabled = !UIElement.enabled;
    }
}
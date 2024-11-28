using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableEventCall : MonoBehaviour
{
    public UnityEvent ScriptOn;

    private void OnEnable()
    {
        ScriptOn.Invoke();
    }
}

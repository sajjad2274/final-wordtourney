using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorListener : MonoBehaviour
{
    public MainMenuHandler mainMenuHandler;
    void OnSpinWheelAnimationFinished()
    {
        mainMenuHandler.SpinFinishedAnimatorWheel();
    }

    
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.WordConnect.Demo
{
    /// <summary>
    /// This script retrieves input data based on the active input platform.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        /// <summary>
        /// Enum for the different input platforms available.
        /// For WebGL use Desktop option.
        /// </summary>
        public enum Platform 
        { 
            MOBILE, 
            DESKTOP 
        }

        /// <summary>
        /// Current active input platform.
        /// </summary>
        public Platform inputPlatform = Platform.DESKTOP;

        /// <summary>
        /// Dictionary containing the platform enum and script controlling the input retrieval for its platform.
        /// </summary>
        private Dictionary<Platform, InputPlatform> _platformInput = new Dictionary<Platform, InputPlatform>();

        /// <summary>
        /// Event to be invoked when the input is removed from the screen
        /// </summary>
        public Action PointerUp;

        /// <summary>
        /// On awake add the platform handlers to the dictionary
        /// </summary>
        private void Awake()
        {
            _platformInput.Add(Platform.MOBILE, new MobileInput());
            _platformInput.Add(Platform.DESKTOP, new DesktopInput());
        }

        /// <summary>
        /// Check each frame if input is detected.
        /// </summary>
        private void Update()
        {
            if (GetPointerUp()) 
                PointerUp?.Invoke();
        }

        /// <summary>
        /// Checks based on the current input platform whether a beginning input has been registered.
        /// </summary>
        /// <returns>Bool whether the current input platform has registered a beginning input.</returns>
        public bool GetPointerDown() => _platformInput[inputPlatform].GetPointerDown();

        /// <summary>
        /// Checks based on the current input platform whether continuous input has been registered.
        /// </summary>
        /// <returns>Bool whether the current input platform has registered a continuous input.</returns>
        public bool GetPointer() => _platformInput[inputPlatform].GetPointer();

        /// <summary>
        /// Checks based on the current input platform whether ending input has been registered.
        /// </summary>
        /// <returns>Bool whether the current input platform has registered an ending input.</returns>
        public bool GetPointerUp() => _platformInput[inputPlatform].GetPointerUp();

        /// <summary>
        /// Gets the pointer position based on the currently active input platform.
        /// </summary>
        /// <returns>The current position of the pointer on the screen.</returns>
        public Vector2 GetPointerPosition() => _platformInput[inputPlatform].GetPointerPosition();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace AliveCell
{
    public class ButtonInputWriter : OnScreenControl
    {
        [InputControl(layout = "Button")]
        [SerializeField]
        private string m_ControlPath;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }

        public void OnButtonUp()
        {
            SendValueToControl(0.0f);
        }

        public void OnButtonDown()
        {
            SendValueToControl(1.0f);
        }
    }
}
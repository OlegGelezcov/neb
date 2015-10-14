using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game
{
    public class ShiftKeyState
    {
        public bool keyPressed { get; private set; }
        public float keyPressTime { get; private set; }


        public void OnKeyDown(float time)
        {
            if(false == this.keyPressed)
            {
                this.keyPressed = true;
                this.keyPressTime = time;
            }
        }

        public void OnKeyUp()
        {
            this.keyPressed = false;
        }
    }
}

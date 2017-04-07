using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Objects
{
    public class Transform
    {
        public int x = 0;
        public int y = 0;
        public float scale_x = 1;
        public float scale_y = 1;

        public Transform()
        { }

        public Transform(int posX, int posY, float scaleX, float scaleY)
        {
            x = posX;
            y = posY;

            scale_x = scaleX;
            scale_y = scaleY;
        }
    }
}

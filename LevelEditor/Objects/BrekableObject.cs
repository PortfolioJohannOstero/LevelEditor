using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    public class BrekableObject : MoveableObject
    {
        protected int mHealth = 0;

        public BrekableObject() { }

        public BrekableObject(string objectName, string sprite, float mass, int health)
        {
            mName = objectName;
            mSpriteFilename = sprite;
            mMass = mass;
            mHealth = health;
        }
    }
}

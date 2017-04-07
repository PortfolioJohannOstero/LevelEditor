using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Events
{
    public class ObjectEvent : EventArgs
    {
        Object mObject;
        public Object getObject
        {
            get { return mObject; }
        }

        public ObjectEvent(Object obj)
        {
            mObject = obj;
        }

        public ObjectEvent(MoveableObject obj)
        {
            mObject = obj;
        }
        public ObjectEvent(BreakableObject obj)
        {
            mObject = obj;
        }

        public ObjectEvent(ExplosiveObject obj)
        {
            mObject = obj;
        }

    }
}

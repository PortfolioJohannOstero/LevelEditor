using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using System.Xml;

namespace LevelEditor
{
    public class BreakableObject : MoveableObject
    {
        protected int mHealth = 0;

        public int Health
        {
            get { return mHealth; }
            set { mHealth = value < 0 ? 0 : value; }
        }

        public BreakableObject()
        {
            mType = Objects.ObjectDictionary_Moveable.BREAKABLE;
        }

        public BreakableObject(string objectName, string sprite, float mass, int health)
            : base(objectName, sprite, mass)
        {
            mType = Objects.ObjectDictionary_Moveable.BREAKABLE;
            mHealth = health;
        }

        public BreakableObject(BreakableObject cpy)
            : base(cpy.Name, cpy.Sprite_filename, cpy.Mass)
        {
            mType = Objects.ObjectDictionary_Moveable.BREAKABLE;
            mHealth = cpy.Health;
        }

        public override string Print(int maxLength)
        {
            return base.Print(maxLength) + "\n" + mHealth;
        }

        public override void AppendAdditionalValues(XmlNode parentNode, XmlDocument document)
        {
            base.AppendAdditionalValues(parentNode, document);

            XmlElement healthNode = document.CreateElement("Health");
            healthNode.InnerText = mHealth.ToString();

            parentNode.AppendChild(healthNode);
        }

        public override bool GetAdditionalAttributes(XmlElement node)
        {
            if (!base.GetAdditionalAttributes(node))
                return false;

            try
            {
                mHealth = int.Parse(node["Health"].InnerXml);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }
    }
}

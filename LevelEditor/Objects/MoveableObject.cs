using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace LevelEditor
{
    public class MoveableObject : Object
    {
        protected float mMass = 0;

        public float Mass
        {
            get { return mMass; }
        }

        public MoveableObject() 
        {
            mType = Objects.ObjectDictionary_Moveable.UNBREAKABLE;
        }

        public MoveableObject(string objectName, string sprite, float mass) : base(objectName, sprite)
        {
            mType = Objects.ObjectDictionary_Moveable.UNBREAKABLE;
            mMass = mass;
        }

        public MoveableObject(MoveableObject cpy)
            : base(cpy.Name, cpy.Sprite_filename)
        {
            mType = Objects.ObjectDictionary_Moveable.UNBREAKABLE;
            mMass = cpy.Mass;
        }


        public override string Print(int maxLength)
        {
            return base.Print(maxLength) + "\n" + mMass;
        }

        public override void AppendAdditionalValues(XmlNode parentNode, XmlDocument document)
        {
            XmlElement massNode = document.CreateElement("Mass");
            massNode.InnerText = mMass.ToString();

            parentNode.AppendChild(massNode);
        }

        public override bool GetAdditionalAttributes(XmlElement node)
        {
            if (!base.GetAdditionalAttributes(node))
                return false;

            try
            {
                mMass = float.Parse(node["Mass"].InnerXml);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }
    }
}

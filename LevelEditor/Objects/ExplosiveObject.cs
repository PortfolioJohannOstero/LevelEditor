using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using System.Xml;

namespace LevelEditor
{
    public class ExplosiveObject : BreakableObject
    {
        protected int mExplosiveDamage = 0;
        protected float mBlasRadius = 0;

        public int ExplosiveDamage
        {
            get { return mExplosiveDamage; }
            set { mExplosiveDamage = value < 0 ? 0 : value; }
        }
        public float BlastRadius
        {
            get { return mBlasRadius; }
            set { mBlasRadius = value < 0 ? 0 : value; }
        }

        public ExplosiveObject()
        {
            mType = Objects.ObjectDictionary_Moveable.EXPLOSIVE;
        }

        public ExplosiveObject(ExplosiveObject cpy)
            : base(cpy.Name, cpy.Sprite_filename, cpy.Mass, cpy.Health)
        {
            mType = Objects.ObjectDictionary_Moveable.EXPLOSIVE;
            mExplosiveDamage = cpy.ExplosiveDamage;
            mBlasRadius = cpy.BlastRadius;
        }

        public ExplosiveObject(string objectName, string sprite, float mass, int health, int explosiveDamage, float blasRadius) 
            : base(objectName, sprite, mass, health)
        {
            mType = Objects.ObjectDictionary_Moveable.EXPLOSIVE;
            mExplosiveDamage = explosiveDamage;
            mBlasRadius = blasRadius;
        }

        public override string Print(int maxLength)
        {
            return base.Print(maxLength) + "\n" + mExplosiveDamage + "\n" + mBlasRadius;
        }

        public override void AppendAdditionalValues(XmlNode parentNode, XmlDocument document)
        {
            base.AppendAdditionalValues(parentNode, document);

            XmlElement damageNode = document.CreateElement("Explosive_Damage");
            damageNode.InnerText = mExplosiveDamage.ToString();

            XmlElement blastRadiusNode = document.CreateElement("Blast_Radius");
            blastRadiusNode.InnerText = mBlasRadius.ToString();

            parentNode.AppendChild(damageNode);
            parentNode.AppendChild(blastRadiusNode);
        }

        public override bool GetAdditionalAttributes(XmlElement node)
        {
            if (!base.GetAdditionalAttributes(node))
                return false;

            try
            {
                mExplosiveDamage = int.Parse(node["Explosive_Damage"].InnerXml);
                mBlasRadius = float.Parse(node["Blast_Radius"].InnerXml);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}

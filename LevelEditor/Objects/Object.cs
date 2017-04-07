    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace LevelEditor
{
    public class Object : Image // <-- Inherits from Image to make manipulating it in the canvas easier
    {
        /* +==== Interface ====+ */   
        public string mName;
        public string mSpritefilename;

        public string Name
        {
            get { return mName; }
        }

        public string Sprite_filename
        {
            get { return mSpritefilename; }
        }

        public int layer = 5;

        public Objects.Transform transform;


        protected Objects.ObjectDictionary_Moveable mType;

        public Objects.ObjectDictionary_Moveable Type
        {
            get { return mType; }
        }

        /* +==============================================+ */
        public Object() 
        {
            mType = Objects.ObjectDictionary_Moveable.OBJECT;
            transform = new Objects.Transform();
        }

        public void UpdateSource()
        {
            Source = new BitmapImage(new Uri(mSpritefilename));
        }

        public Object(string objectName, string spriteFilename)
        {
            mName = objectName;
            mType = Objects.ObjectDictionary_Moveable.OBJECT;
            mSpritefilename = spriteFilename;

            transform = new Objects.Transform();
            Source = new BitmapImage(new Uri(mSpritefilename));
        }

        public Object(Object cpy)
        {
            mName = cpy.Name;
            mType = Objects.ObjectDictionary_Moveable.OBJECT;
            mSpritefilename = cpy.Sprite_filename;
            transform = new Objects.Transform();

            Source = new BitmapImage(new Uri(mSpritefilename));
        }

        public virtual string Print(int maxLength)
        {
            return Utility.TruncateString(mName, maxLength) + "\n" + mType;
        }

        public override string ToString()
        {
            return mName;
        }

        public virtual void AppendAdditionalValues(XmlNode parentNode, XmlDocument document)
        {

        }

        public virtual bool GetAdditionalAttributes(XmlElement node)
        {
            try
            {
                mName = node["Name"].InnerXml;
                mSpritefilename = node["SpriteSource"].InnerXml;
                layer = int.Parse(node["Layer"].InnerXml);

                XmlNodeList tNode = node["Transform"].ChildNodes;

                int x = int.Parse(tNode[0].Attributes["PosX"].Value);
                int y = int.Parse(tNode[0].Attributes["PosY"].Value);

                int scaleX = int.Parse(tNode[1].Attributes["ScaleX"].Value);
                int scaleY = int.Parse(tNode[1].Attributes["ScaleY"].Value);

                transform = new Objects.Transform(x, y, scaleX, scaleY);
            }
            catch(Exception)
            {
                return false;
            }
               
            return true;
        }
    }
}

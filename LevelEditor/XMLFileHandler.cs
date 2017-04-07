using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;

namespace LevelEditor
{
    class XMLFileHandler
    {
        XmlDocument mDoc;
        string mFilename;

        string mCoreContainerName = "StoredObjects"; // <-- Default

        XmlWriterSettings settings;

        bool active = false;

        /* +==== Getters ====+ */
        #region Getters
        public bool IndentTags
        {
            get { return settings.OmitXmlDeclaration; }
            set { settings.OmitXmlDeclaration = value; }
        }
        #endregion

        /* +==== Constructor and Initializer ====+ */
        #region Constructor and Initializer
        public XMLFileHandler()
        {}

        public bool Intialize(string filename)
        {
            /* +=== Error check ===+ */
            if(!File.Exists(filename))
                return false;

            active = true;


            /* Initial xml setup check */
            mDoc = new XmlDocument();
            mFilename = filename;
            settings = new XmlWriterSettings();

            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.IndentChars = "\t";

            XmlElement root = mDoc.DocumentElement;
            if (!File.ReadAllText(mFilename).Contains(mCoreContainerName)) // if root node cannot be found, then it loads it in
            {
                XmlNode initNode = mDoc.CreateNode(XmlNodeType.Element, mCoreContainerName, null);
                mDoc.InsertAfter(initNode, root);

                XmlDeclaration xmlDecleration = mDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                mDoc.InsertAfter(xmlDecleration, root);

                mDoc.Save(filename);
            }
            return active;
        }
        #endregion

        /* +==== Add to the XML file ====+ */
        #region Add to the XML file
        public bool add(Object obj, string group)
        {
            if (!active || obj == null)
                return false;

            mDoc.Load(mFilename);

            XmlNode groupNode = establishGroupNode(group);

            /* +=== Default values ===+ */
            XmlNode parentNode = mDoc.CreateNode(XmlNodeType.Element, obj.Type.ToString(), null);
            XmlNode nameNode = newElement<string>("Name", obj.Name);
            XmlNode spriteNode = newElement<String>("SpriteSource", obj.Sprite_filename);
            XmlElement layerNode = newElement<int>("Layer", obj.layer);

            XmlNode transformNode = mDoc.CreateNode(XmlNodeType.Element, "Transform", null);
                XmlElement positionNode = mDoc.CreateElement("Position");
                    XmlAttribute xPos = newAttribute<int>("PosX", obj.transform.x);
                    XmlAttribute yPos = newAttribute<int>("PosY", obj.transform.y);

                XmlElement scaleNode = mDoc.CreateElement("Scale");
                    XmlAttribute xScale = newAttribute<float>("ScaleX", obj.transform.scale_x);
                    XmlAttribute yScale = newAttribute<float>("ScaleY", obj.transform.scale_y);
            
            groupNode.AppendChild(parentNode);
                parentNode.AppendChild(nameNode);
                parentNode.AppendChild(spriteNode);
                parentNode.AppendChild(layerNode);
                    parentNode.AppendChild(transformNode);
                        transformNode.AppendChild(positionNode);
                            positionNode.Attributes.Append(xPos);
                            positionNode.Attributes.Append(yPos);
                        transformNode.AppendChild(scaleNode);
                            scaleNode.Attributes.Append(xScale);
                            scaleNode.Attributes.Append(yScale);


            /* +=== Additional values ===+ */
            if(obj.Type != Objects.ObjectDictionary_Moveable.OBJECT)
                obj.AppendAdditionalValues(parentNode, mDoc);

            return true;
        }

        public bool add(EditorMapEvent map, string group)
        {
            if (!active || map == null)
                return false;

            mDoc.Load(mFilename);

            XmlNode groupNode = establishGroupNode(group);

            XmlNode nameNode = newElement<string>("Name", map.Name);
            XmlNode bgSrcNode = newElement<string>("Background_Source", map.BackgroundSource);
            XmlNode widthNode = newElement<int>("Width", map.Width);
            XmlNode heightNode = newElement<int>("Height", map.Height);

            groupNode.AppendChild(nameNode);
            groupNode.AppendChild(bgSrcNode);
            groupNode.AppendChild(widthNode);
            groupNode.AppendChild(heightNode);
            return true;
        }
        #endregion

        /* +==== Helper methods ====+ */
        #region Helper Methods
        private XmlNode establishGroupNode(string group)
        {
            XmlNode groupNode = mDoc.DocumentElement.GetElementsByTagName(group)[0];
            if (groupNode == null)
            {
                groupNode = mDoc.CreateNode(XmlNodeType.Element, group, group);
                mDoc.DocumentElement.PrependChild(groupNode);
            }

            return groupNode;
        }
        private XmlElement newElement<T>(string tagName, T value)
        {
            XmlElement element = mDoc.CreateElement(tagName);
            element.InnerText = value.ToString();

            return element;
        }
        private XmlAttribute newAttribute<T>(string attributeName, T value)
        {
            XmlAttribute attribute = mDoc.CreateAttribute(attributeName);
            attribute.Value = value.ToString();

            return attribute;
        }
        #endregion

        /* +==== Get information from the XML file ====+ */
        #region Get information from the XML file
        public EditorMapEvent getMap(string xmlContainer)
        {
            if (!active)
                return null;

            mDoc.Load(mFilename);

            XmlNode rootNode = mDoc.DocumentElement.GetElementsByTagName(xmlContainer)[0];
            if (rootNode == null)
                return null;

            EditorMapEvent map;
            try
            {
                string name = rootNode["Name"].InnerXml;
                string bgSrc = rootNode["Background_Source"].InnerXml;
                int width = int.Parse(rootNode["Width"].InnerXml);
                int height = int.Parse(rootNode["Height"].InnerXml);

                map = new EditorMapEvent(name, bgSrc, width, height);
                
            } 
            catch(Exception)
            {
                return null;
            }

            return map;
        }

        public List<Object> get(string xmlContainer)
        {
            if (!active)
                return null;

            mDoc.Load(mFilename);

            XmlNode nodeRoot = mDoc.DocumentElement.GetElementsByTagName(xmlContainer)[0];
            if (nodeRoot == null)
                return null;

            XmlNodeList nodeList = nodeRoot.ChildNodes;
            if (nodeList.Count <= 0)
                return null;

            List<Object> objects = new List<Object>();
            foreach(XmlElement node in nodeList)
            {
                Object obj = new Object();

                if (node.LocalName == Objects.ObjectDictionary_Moveable.UNBREAKABLE.ToString())
                    obj = new MoveableObject();
                else if (node.LocalName == Objects.ObjectDictionary_Moveable.BREAKABLE.ToString())
                    obj = new BreakableObject();
                else if (node.LocalName == Objects.ObjectDictionary_Moveable.EXPLOSIVE.ToString())
                    obj = new ExplosiveObject();


                if (!obj.GetAdditionalAttributes(node))
                    continue;

                objects.Add(obj);
            }

            return objects;
        }
        #endregion

        public bool remove(string name, string xmlContainer)
        {
            if (!active)
                return false;

            mDoc.Load(mFilename);
            XmlNode nodeRoot = mDoc.DocumentElement.GetElementsByTagName(xmlContainer)[0];
            if (nodeRoot == null)
                return false;

            XmlNodeList nodeList = nodeRoot.ChildNodes;
            if(nodeList.Count <= 0)
                return false;

            foreach(XmlNode node in nodeList)
            {
                if(node["Name"].InnerText == name)
                {
                    nodeRoot.RemoveChild(node);
                    return true;
                }
            }

            return false;
        }

        public bool save()
        {
            if (!active)
                return false;

            mDoc.Save(mFilename);
            return true;
        }
    }
}

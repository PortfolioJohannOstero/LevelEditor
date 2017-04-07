using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LevelEditor
{
    public class EditorMapEvent : EventArgs
    {
        string mName;
        string mBackgroundSource;
        int mWidth, mHeight;
        Color mBackgroundColour;

        public string Name
        { get { return mName; } }

        public string BackgroundSource
        { get { return mBackgroundSource; } }

        public int Width
        { get { return mWidth; } }

        public int Height
        { get { return mHeight; } }

        public Color BackgroundColour
        { get { return mBackgroundColour; } }

        public EditorMapEvent(string name, string backgroundImage, int mapWidth, int mapHeight)
        {
            mName = name;
            mBackgroundSource = backgroundImage;
            mWidth = mapWidth;
            mHeight = mapHeight;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//using System.IO;
using Microsoft.Win32;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for CreateMapWindow.xaml
    /// </summary>
    public partial class CreateMapWindow : Window
    {
        public event EventHandler<EditorMapEvent> RaiseCreateMapEvent;

        private string mBackgroundImage = "";

        private bool createdMap = false;

        public CreateMapWindow()
        {
            InitializeComponent();
        }


        private void cb_mapSizeEqualsBackgroundSize_Clicked(object sender, RoutedEventArgs e)
        {
            gb_customDimensons.IsEnabled = !cb_mapSizeEqualsBackgroundSize.IsChecked.Value;
        }
        private void btn_loadBackground_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a background Image";
            openFileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            if(openFileDialog.ShowDialog().Value)
            {
                label_backgroundName.Content = openFileDialog.FileName;
                mBackgroundImage = openFileDialog.FileName;
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            if (CreateMap())
            {
                createdMap = true;
                Close();
            }
        }

        public bool CreateMap()
        {
            if (RaiseCreateMapEvent == null)
                return false;
            
            if(tb_mapName.Text == "")
            {
                errMessage("No map name was found!");
                return false;
            }

            int width = 0;
            int height = 0;
            if (!cb_mapSizeEqualsBackgroundSize.IsChecked.Value)
            {
                if(tb_screenWidth.Text != "" && tb_screenHeight.Text != "" &&
                   (Utility.isPostiveRealNumber(tb_screenWidth.Text) && Utility.isPostiveRealNumber(tb_screenHeight.Text)) &&
                   (width = Int32.Parse(tb_screenWidth.Text)) >= 0 && (height = Int32.Parse(tb_screenHeight.Text)) >= 0);
                else
                {
                    errMessage("Invalid Width and/or Height input(s)!");
                    return false;
                }
                
            }
            else if (mBackgroundImage != null)
            {
                BitmapImage img = new BitmapImage(new Uri(mBackgroundImage));

                width = img.PixelWidth;
                height = img.PixelHeight;
            }
            else
            {
                errMessage("No Image was found!");
                return false;
            }


            RaiseCreateMapEvent(this, new EditorMapEvent(tb_mapName.Text, mBackgroundImage, width, height));
            return true;
        }



        void errMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK);
        }

        private void window_closed(object sender, EventArgs e)
        {
            if(!createdMap)
                RaiseCreateMapEvent(this, null);
        }
    }
}

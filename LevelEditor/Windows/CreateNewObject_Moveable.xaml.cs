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

using Microsoft.Win32;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for CreateNewObject_Moveable.xaml
    /// </summary>
    public partial class CreateNewObject_Moveable : Window
    {
        public event EventHandler<Events.ObjectEvent> RaiseCreateObjectEvent;

        Object newObject;
        string spriteFilename = "";
        bool created = false;

        public CreateNewObject_Moveable()
        {
            InitializeComponent();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_loadSprite_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a Sprite Image";
            openFileDialog.Filter = "PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|GIF Files (*.gif)|*.gif";

            if (openFileDialog.ShowDialog().Value)
            {
                BitmapImage spriteImage = new BitmapImage(new Uri(openFileDialog.FileName));
                spriteFilename = openFileDialog.FileName;

                image_spriteDisplay.Source = spriteImage;
            }
        }

        private void cb_isExplosive_Click(object sender, RoutedEventArgs e)
        {
            tb_explosiveRadius.IsEnabled = cb_isExplosive.IsChecked.Value;
            tb_explosiveDamage.IsEnabled = cb_isExplosive.IsChecked.Value;
        }

        private void dropdown_itemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool state = (dropdown_itemType.SelectedIndex == 0);

            if(cb_isExplosive != null)
            {
                cb_isExplosive.IsEnabled = state;
                tb_health.IsEnabled = state;
                
                if(state)
                {
                    tb_explosiveRadius.IsEnabled = cb_isExplosive.IsChecked.Value;
                    tb_explosiveDamage.IsEnabled = cb_isExplosive.IsChecked.Value;
                }
                else
                {
                    tb_explosiveRadius.IsEnabled = state;
                    tb_explosiveDamage.IsEnabled = state;
                }
                    
            }
            
        }

        private void btn_whiteBlackToggle_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush currColour = ((SolidColorBrush)btn_whiteBlackToggle.Background);
            border_spriteBackground.Background = currColour;


            if (currColour.Color == Brushes.White.Color)
                btn_whiteBlackToggle.Background = Brushes.Black;
            else
                btn_whiteBlackToggle.Background = Brushes.White;
        }

        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            if (tb_name.Text == "" || tb_mass.Text == "" || spriteFilename == "")
            {
                MessageBox.Show("Invalid name, mass or missing sprite!", "Sprite Creation Error", MessageBoxButton.OK);
            }
            else if(dropdown_itemType.SelectedItem == comboItem_breakable)
            {
                if(tb_health.Text == "")
                    MessageBox.Show("Invalid health!", "Sprite Creation Error", MessageBoxButton.OK);
                else if(!cb_isExplosive.IsChecked.Value)
                {
                    newObject = new BreakableObject(tb_name.Text, spriteFilename, int.Parse(tb_mass.Text), int.Parse(tb_health.Text));  // PASS!
                }
                else if(cb_isExplosive.IsChecked.Value)
                {
                    if (tb_explosiveRadius.Text == "" || tb_explosiveDamage.Text == "")
                        MessageBox.Show("Invalid Explosive parameters", "Sprite Creation Error", MessageBoxButton.OK);
                    else
                        // PASS
                        newObject = new ExplosiveObject(tb_name.Text, spriteFilename, int.Parse(tb_mass.Text), int.Parse(tb_health.Text), int.Parse(tb_explosiveDamage.Text), int.Parse(tb_explosiveRadius.Text));
                }
            }
            else if(dropdown_itemType.SelectedItem == comboItem_unbreakable)
            {
                newObject = new MoveableObject(tb_name.Text, spriteFilename, int.Parse(tb_mass.Text)); // PASS
            }

            if (newObject != null && CreateObject())
            {
                created = true;
                Close();
            }
        }

        /* +==== Create Object ====+ */
        public bool CreateObject()
        {
            if (RaiseCreateObjectEvent == null)
                return false;

            RaiseCreateObjectEvent(this, new Events.ObjectEvent(newObject));
            return true;
        }

        private void window_closed(object sender, EventArgs e)
        {
            if (!created)
                RaiseCreateObjectEvent(this, null);
        }


        /* +==== Handling numbers only text boxes ====+ */
        private void tb_numbersOnly_focusOut(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;

            if (!Utility.isPostiveRealNumber(textbox.Text))
            {
                textbox.Text = "";
                textbox.Background = Brushes.Red;
            }
                
        }

        private void tb_numbersOnly_focusIn(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;
            textbox.Background = Brushes.White;
        }


    }
}

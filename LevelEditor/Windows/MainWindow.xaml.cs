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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;

namespace LevelEditor
{

    public partial class MainWindow : Window
    {
        String currentLayerValue = "";

        bool isDragging = false;
        Point mouseOffset;

        private List<Object> mapObject;
        EditorMapEvent mapDetails;

        Object currentObject;

        string currentFilename = "";

        // XML config members
        string xmlconfig_storedObjectContainer = "";
        string[] xmlconfig_storageGroups = new string[]{"BACKGROUND", "MOVEABLE"};

        /* +==== New Window Forms: Used to make sure that only one window is open at a time */
        CreateMapWindow createWindow = null;
        CreateNewObject_Moveable createMoveableWindow = null;

        XMLFileHandler xmlFileHandler; // <--- the objects xml file handler

        int maxDescriptionLength = 15; // <--- Used for printing out in the description box

        /* +==== Init ====+ */
        #region Init
        public MainWindow()
        {
            InitializeComponent();
            currentLayerValue = tb_layerValue.Text;
            grid_objectDescription.Visibility = Visibility.Hidden;

            xmlFileHandler = new XMLFileHandler();
            if(xmlFileHandler.Intialize(@"..\..\Resources\StoredObjects.xml"))
                LoadInStoredObjects();

            collectFromXmlConfig(@"..\..\Resources\XMLConfig.txt");

            mapObject = new List<Object>();
        }

        void collectFromXmlConfig(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("Missing " + filename, "Missing file", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
                StreamReader sr = new StreamReader(File.OpenRead(filename));
                sr.ReadLine(); // <- Skip the firstline
                
                // Special case
                string[] s = sr.ReadLine().Split(':');
                xmlconfig_storedObjectContainer = s[1];

                sr.ReadLine();
                int groupSize = int.Parse(sr.ReadLine());

                xmlconfig_storageGroups = new string[groupSize];
                for(int i = 0; i < groupSize || !sr.EndOfStream; i++)
                    xmlconfig_storageGroups[i] = sr.ReadLine().Split(':')[1];
        }
        #endregion


        /* +==== Layer Controllers ====+ */
        /* Allows the user to only set the value to what the slider is capable off
         * Uses the slider and textbox 
         */
        #region Layer Controllers
        private void slider_layerController_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (tb_layerValue != null)
                tb_layerValue.Text = ((int)slider_layerController.Value).ToString();

            if (currentObject != null)
            {
                currentObject.layer = (int)e.NewValue;
                Canvas.SetZIndex(currentObject, currentObject.layer);
            }
        }

        /* +=== Textbox layer value events ===+ */
        private void tb_layerValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(tb_layerValue.Text != "")
            {
                int value;
                if (Utility.isPostiveRealNumber(tb_layerValue.Text) && (value = int.Parse(tb_layerValue.Text)) <= slider_layerController.Maximum)
                {
                    currentLayerValue = tb_layerValue.Text;
                    slider_layerController.Value = value;
                }
                else
                    tb_layerValue.Text = currentLayerValue;
            }
        }

        private void tb_layerValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tb_layerValue.Text == "")
                tb_layerValue.Text = currentLayerValue;
        }
        #endregion


        /* +==== Creating a new map ====+ */
        /* Using the init new map button and the file/button tab to open the CreateMapWindow
         * Sends a custom EditorMapEvent that retrieves the map details from the CreateMapWindow
         * And removes it when the CreateMapWindow has been terminated (closed)
         *
         * Also, it removes the init buttons when the event was succesful.
         */
        #region  Creating a new Map
        private void btn_newMap_init_Click(object sender, RoutedEventArgs e)
        {
            if(createWindow == null)
            {
                createWindow = new CreateMapWindow();
                createWindow.RaiseCreateMapEvent += new EventHandler<EditorMapEvent>(collectMapDetails);
                createWindow.Show();
            }
        }

       public void collectMapDetails(object sender, EditorMapEvent e)
       {
           currentFilename = "";
           updateCanvas(e);
           mapDetails = e;
           createWindow.RaiseCreateMapEvent -= new EventHandler<EditorMapEvent>(collectMapDetails);
           createWindow = null;

           
       }
        #endregion


        /* +=== Terminate the program ====+*/
       private void menuItem_closeWindow(object sender, RoutedEventArgs e)
       {
           Close();
       }

        /* +==== Creating/Removing (new) Moveable Object(s) ====+ */
       /* Using the new button on the Moveable tab to open the CreateNewObject_Moveable
        * Sends a custom ObjectEvent that retrieves the Object details from the CreateNewObject_Moveable
        * And removes it when the CreateNewObject_Moveable has been terminated (closed)
        */
       #region Creating/Removing Moveable Object(s)
        #region Create a new Moveable Object
        private void btn_newObject_moveable_Click(object sender, RoutedEventArgs e)
        {
            if (createMoveableWindow == null)
            {
                createMoveableWindow = new CreateNewObject_Moveable();
                createMoveableWindow.RaiseCreateObjectEvent += new EventHandler<Events.ObjectEvent>(collectMoveableObject);
                createMoveableWindow.Show();
            }
        }

        public void collectMoveableObject(object sender, Events.ObjectEvent e)
        {
            if (e != null)
            {
                listview_moveableObjectList.Items.Add(e.getObject);
                listview_moveableObjectList.SelectedItem = e.getObject; /* <-- sets the index to be the newely created object */

                xmlFileHandler.add(e.getObject, "MOVEABLE");
                xmlFileHandler.save();
            }

            createMoveableWindow.RaiseCreateObjectEvent -= new EventHandler<Events.ObjectEvent>(collectMoveableObject);
            createMoveableWindow = null;
        }
        #endregion

        #region Remove Moveable Object
        private void btn_removeMoveableObject_Click(object sender, RoutedEventArgs e)
        {
            if (listview_moveableObjectList.SelectedItem != null)
            {
                if(MessageBox.Show("Are you sure you want to delete \"" + (listview_moveableObjectList.SelectedItem as Object).Name + "\"?", "Deleting Moveable Object", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {

                    bool derp = xmlFileHandler.remove((listview_moveableObjectList.SelectedItem as Object).Name, xmlconfig_storageGroups[1]);
                    MessageBox.Show(derp.ToString());
                    
                    xmlFileHandler.save();

                    /* Moves the index to the previous one if there is a previous one, otherwise, leave it at index 0 */
                    int previousIndex;
                    if ((previousIndex = listview_moveableObjectList.SelectedIndex) > 0)
                        previousIndex--;

                    listview_moveableObjectList.Items.Remove(listview_moveableObjectList.SelectedItem);
                    listview_moveableObjectList.SelectedIndex = previousIndex;

                    btn_removeMoveableObject.IsEnabled = !listview_moveableObjectList.Items.IsEmpty;
                }
            }
        }

        private void listview_moveableObjectList_LostFocus(object sender, RoutedEventArgs e)
        {
            btn_removeMoveableObject.IsEnabled = false;
        }

        private void listview_moveableObjectList_GotFocus(object sender, RoutedEventArgs e)
        {
            btn_removeMoveableObject.IsEnabled = listview_moveableObjectList.SelectedItem != null;
        }
        #endregion
       #endregion

        

        /* Updates the object description box appropriately */
        private void listview_moveableObjectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listview_moveableObjectList.SelectedItem != null)
            {
                grid_objectDescription.Visibility = Visibility.Visible;

                Object typeCollector = listview_moveableObjectList.SelectedItem as Object;
                string description;


                if (typeCollector.Type == Objects.ObjectDictionary_Moveable.UNBREAKABLE)
                {
                    typeCollector = new MoveableObject(listview_moveableObjectList.SelectedItem as MoveableObject);
                    description = (listview_moveableObjectList.SelectedItem as MoveableObject).Print(maxDescriptionLength);
                }
                    
                else if (typeCollector.Type == Objects.ObjectDictionary_Moveable.BREAKABLE)
                {
                    typeCollector = new BreakableObject(listview_moveableObjectList.SelectedItem as BreakableObject);
                    description = (listview_moveableObjectList.SelectedItem as BreakableObject).Print(maxDescriptionLength);
                }
                    
                else if (typeCollector.Type == Objects.ObjectDictionary_Moveable.EXPLOSIVE)
                {
                    typeCollector = new ExplosiveObject(listview_moveableObjectList.SelectedItem as ExplosiveObject);
                    description = (listview_moveableObjectList.SelectedItem as ExplosiveObject).Print(maxDescriptionLength);
                }
                else
                {
                    typeCollector = new Object(listview_moveableObjectList.SelectedItem as Object);
                    description = typeCollector.Print(maxDescriptionLength);
                }
                    

                label_descriptionList.Text = description;

                if(map_canvas.IsEnabled)
                    AddObjectToMap(typeCollector);
                
            }
            else
                grid_objectDescription.Visibility = Visibility.Hidden;
        }


        /* +=== Map Handling  ==== */
        #region Map Handling
        private void AddObjectToMap(Object obj)
        {
            obj.MouseDown += CanvasObject_MouseDown;
            obj.UpdateSource();
            obj.AllowDrop = true;
            map_canvas.Children.Add(obj);
            Canvas.SetZIndex(obj, obj.layer);
        }

        public void updateCanvas(EditorMapEvent map)
        {
            if (map != null)
            {
                map_canvas.Children.Clear();

                border_init_container.Visibility = Visibility.Hidden;
                border_layerHandler.IsEnabled = true;

                if (map.BackgroundSource != "")
                    map_canvas.Background = new ImageBrush(new BitmapImage(new Uri(map.BackgroundSource)));
                else
                    map_canvas.Background = new SolidColorBrush(Colors.Black);

                map_canvas.Width = map.Width;
                map_canvas.Height = map.Height;
                map_canvas.IsEnabled = true;

                label_mapSize.Content = map.Width + " x " + map.Height;
                label_mapSize.Visibility = Visibility.Visible;

                label_xPos.Visibility = Visibility.Visible;
                label_yPos.Visibility = Visibility.Visible;
                label_layer.Visibility = Visibility.Visible;

                btn_file_save.IsEnabled = true;
                btn_file_saveAss.IsEnabled = true;
            }
        }

        /* +=== Event Handling ===+ */
        #region Draggin Handling
        private void CanvasObject_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            Object img = (Object)sender;

            mouseOffset = e.GetPosition(img);

            img.MouseMove += CanvasObject_MouseMove;
            img.MouseUp += CanvasObject_MouseUp;
            img.CaptureMouse();
            

            updatePositionLabels(img.transform.x, img.transform.y);
            updateLayerLabel(img.layer);

            // Selected item
            currentObject = img;
        }

        private void CanvasObject_MouseUp(object sender, MouseEventArgs e)
        {
            Object img = (Object)sender;
            img.MouseMove -= CanvasObject_MouseMove;
            img.MouseUp -= CanvasObject_MouseUp;
            img.ReleaseMouseCapture();

            if (img.transform.x < 0 || img.transform.y < 0 || img.transform.x > mapDetails.Width || img.transform.y > mapDetails.Height)
            {
                map_canvas.Children.Remove(img);
                currentObject = null;
                updatePositionLabels(0, 0);
                updateLayerLabel(0);
            }
        }

        private void CanvasObject_MouseMove(object sender, MouseEventArgs e)
        {
            if(isDragging)
            {
                Object img = (Object)sender;

                // Get the position of the ellipse relative to the Canvas
                Point point = e.GetPosition(map_canvas);

                img.SetValue(Canvas.TopProperty, point.Y - mouseOffset.Y);
                img.SetValue(Canvas.LeftProperty, point.X - mouseOffset.X);

                img.transform.x = (int)Canvas.GetLeft(img);
                img.transform.y = (int)Canvas.GetTop(img);

                updatePositionLabels(img.transform.x, img.transform.y);
            }
        }
        #endregion
        private void updatePositionLabels(int x, int y)
        {
            label_xPos.Content = "x: " + x;
            label_yPos.Content = "y: " + y;
        }

        private void updateLayerLabel(int layer)
        {
            label_layer.Content = "Layer: " + layer;
        }
        #endregion

        /* +==== Saving and Loading ====+ */
        #region Saving & Loading
        #region Save
        private void btn_saveAss_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();  // http://msdn.microsoft.com/en-us/library/microsoft.win32.savefiledialog%28v=vs.110%29.aspx
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "Config Files (.xml)|*.xml"; // Filter files by extension 

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results 
            if (result == true)
            {
                saveToXmlFile(dlg.FileName);
                currentFilename = dlg.FileName;
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if(currentFilename == "")
                btn_saveAss_Click(sender, e);
            else
                saveToXmlFile(currentFilename);
        }

        private void saveToXmlFile(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            File.Create(filename).Close();

            XMLFileHandler newDocument = new XMLFileHandler();
            newDocument.Intialize(filename);


            foreach(Object obj in map_canvas.Children)
            {
                newDocument.add(obj, "MOVEABLE");
                newDocument.save();
            }

            newDocument.add(mapDetails, "BACKGROUND");

            newDocument.save();
        }

        #endregion

        #region Load
        private void btn_loadMap_init_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();  // http://msdn.microsoft.com/en-us/library/microsoft.win32.openfiledialog%28v=vs.110%29.aspx
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "Config Files (.xml)|*.xml"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                if(dlg.FileName != "")
                {
                    currentFilename = dlg.FileName;
                    XMLFileHandler newDocument = new XMLFileHandler();
                    newDocument.Intialize(dlg.FileName);

                    mapDetails = newDocument.getMap(xmlconfig_storageGroups[0]);
                    updateCanvas(mapDetails);

                    List<Object> objList = newDocument.get(xmlconfig_storageGroups[1]);
                    if(objList != null && objList.Count > 0)
                    {
                        foreach (Object obj in objList)
                        {
                            obj.UpdateSource();

                            // Get the position of the ellipse relative to the Canvas
                            Canvas.SetLeft(obj, obj.transform.x);
                            Canvas.SetTop(obj, obj.transform.y);
                            AddObjectToMap(obj);
                        }
                    }
                }
            }
        }

        private void LoadInStoredObjects()
        {
            List<Object> tempObjList = xmlFileHandler.get(xmlconfig_storageGroups[1]);
            if (tempObjList == null)
                return;

            foreach (Object obj in tempObjList)
            {
                if (File.Exists(obj.Sprite_filename))
                {
                    listview_moveableObjectList.Items.Add(obj);
                    obj.UpdateSource();
                }
            }
        }
        #endregion
        #endregion

        private void btn_file_about_Click(object sender, RoutedEventArgs e)
        {
            Windows.AboutWindow aboutWindow = new Windows.AboutWindow();
            aboutWindow.Show();
        }
    }
}
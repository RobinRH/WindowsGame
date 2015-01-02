// Copyright Robin A. Reynolds-Haertle, 2015

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FourRivers
{
    public sealed partial class Tile : UserControl, IComparable
    {

        public Tile()
        {
            this.InitializeComponent();
            m_imageControl = new Image();
            this.grid1.Children.Add(m_imageControl);
        }

        public Image ImageControl
        {
            get { return m_imageControl; }
        }

        private Image m_imageControl;
        private const int c_Spacing = 38;
        private int m_Column = 0;
        private int m_Row = 0;
//        public Colors Color = Colors.Red;
        public int Number = 0;
        public int RandomIndex = 0;
        //public bool Clear = false;
        private bool m_Clear = false;
        private bool m_selected = false;
        public string fileName = "";
        public BitmapImage m_image = null;
        //private Vector2 m_position;
        //public Vector2 Position;
        Windows.Foundation.Point m_position;

        //public Tile(Colors newColor, int newNumber)
        public Tile(int newNumber)
        {
            this.InitializeComponent();
            Number = newNumber;
            this.Width = 50;
            this.Height = 50;
            m_imageControl = new Image();
            this.grid1.Children.Add(m_imageControl);
        }

        public int Row
        {
            get { return m_Row; }
            set
            {
                m_Row = value;
                m_position = new Windows.Foundation.Point(Left, Top);
            }
        }

        public int Column
        {
            get { return m_Column; }
            set {
                m_Column = value;
                m_position = new Point(Left, Top);
            }
        }

        public bool Clear
        {
            get { return m_Clear; }
            set
            {
                m_Clear = value;
                if (m_Clear)
                {
                    //Canvas parent = (Canvas)this.Parent;
                    //if (parent != null) parent.Children.Remove(this);
                    this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    this.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
        }

        public bool Selected
        {
            get { return m_selected; }
            set
            {
                m_selected = value;
                this.Opacity = m_selected ? .50 : 1;
            }
        }

        public BitmapImage Image
        {
            get { return m_image; }
            set { m_image = value; }
        }


        public Point Position
        {
            get { return m_position; }
        }

        public int Top
        {
            get
            {
                //return m_Row * c_Spacing + Game1.c_TopMargin;
                return m_Row;
            }
        }

        public int Left
        {
            get
            {
                //return m_Column * c_Spacing + Game1.c_Edge;
                return m_Column;
            }
        }

        #region Implementation of IComparable
        public int CompareTo(object obj)
        {
            Tile tile = (Tile)obj;
            return this.RandomIndex - tile.RandomIndex;
        }
        #endregion
    }


  
    
    }


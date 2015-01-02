// Copyright Robin A. Reynolds-Haertle, 2015

// TODO Add undo
// TODO Add theme


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
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Shapes;
using Windows.UI;

namespace FourRivers
{


    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : FourRivers.Common.LayoutAwarePage
    {
        int c_Rows = 12;
        int c_Columns = 12;
        TileGrid m_grid = new TileGrid();
        bool g_testmode = false;
        bool g_losergame = false;
        bool m_animate = true;
        bool m_sound = true;
        bool m_ignoreClicks = false;

        Rect _windowBounds;
        double _settingsWidth = 346;
        Popup _settingsPopup;

        BitmapImage[] m_images = new BitmapImage[36];
        BitmapImage[][] images = new BitmapImage[6][];

        // used to animate tiles
        private System.Collections.Generic.List<Point> m_animationPoints;
        private Tile tileA;
        private Tile tileB;


        public MainPage()
        {
            this.InitializeComponent();
            _windowBounds = Window.Current.Bounds;
            SettingsPane.GetForCurrentView().CommandsRequested += BlankPage_CommandsRequested;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            string assetName;
            for (int tileCount = 0; tileCount < 36; tileCount++)
            {
                assetName = "ms-appx:///Assets/SedateTheme/tile" + tileCount.ToString() + ".png";
                m_images[tileCount] = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(assetName));
            }

            int[] rows = Enumerable.Range(0, 12).ToArray<int>();
            int[] columns = Enumerable.Range(0, 12).ToArray<int>();

            this.NewGame(true);
        }


        private void tileClick(object sender, RoutedEventArgs e)
        {
            
            if (m_ignoreClicks)
            {
                return;
            }

            // make clicking sound
            if (m_sound)
            {
                soundElement.Play();
            }

            // find the tile clicked
            Tile tile = (Tile)sender;
            Grid.GetRow(tile);

            if (tile == null)
            {
                return;
            }

            if (!tile.Clear)
            {
                tile.Selected = !tile.Selected;
            }

            Tile[] selectedTiles = new Tile[2];
            int selected = m_grid.HowManySelected(selectedTiles);
            Path path = new Path();

            Tile tile1 = null;
            Tile tile2 = null;
            if (selected == 2)
            {
                if (tile == selectedTiles[0])
                {
                    tile1 = selectedTiles[0];
                    tile2 = selectedTiles[1];
                }
                else
                {
                    tile1 = selectedTiles[1];
                    tile2 = selectedTiles[0];
                }

                if (m_grid.CanRemove(tile1, tile2, ref path))
                {
                    if (m_animate)
                    {
                        m_ignoreClicks = true;
                        CreateAnimationSteps(tile1, tile2, path);
                        AnimateLine1();
                        // animation will check for end of game
                    }
                    else
                    {
                        tile1.Clear = true;
                        tile2.Clear = true;
                        tile1.Selected = false;
                        tile2.Selected = false;
                        // check for end of game
                        CheckForEndOfGame();
                        //bool result = PlaySoundW("bigexpl.wav", null, 0);
                    }

                }
                else
                {
                    //they aren't a match
                    tile1.Selected = false;
                    tile2.Selected = false;
                }
            }
            
            //if (!m_grid.AnyPairsLeft(ref tile1, ref tile2))
            //{
            //    // game is over
            //    m_grid.ClearAll();
            //    txtGameOver.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //}
            //else
            //{
            //    txtGameOver.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            //}
        }

        private void CheckForEndOfGame()
        {
            Tile tile1 = null;
            Tile tile2 = null;
            if (!m_grid.AnyPairsLeft(ref tile1, ref tile2))
            {
                // game is over
                //m_grid.ClearAll();
                if (m_grid.IsGameWon())
                {
                    WinningGameAnimation();
                }
                else
                {
                    GameLostAnimation();
                }
            }
            else
            {
                //txtGameOver.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //GameOverAnimation();
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }


        List<Tile> m_tiles = null;
        private void NewGame(bool firstGame)
        {
            txtGameOver1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            if (firstGame)
            {
                // initialize a new set of tiles
                m_tiles = new List<Tile>();
                for (int tileCount = 0; tileCount < 36; tileCount++)
                {
                    for (int copies = 1; copies <= 4; copies++)
                    {
                        Tile newTile = new Tile(tileCount);
                        m_tiles.Add(newTile);
                        newTile.Tapped += new TappedEventHandler(tileClick);
                        newTile.Image = m_images[tileCount];
                        newTile.ImageControl.Source = newTile.Image;
                    }
                }
            }

            do
            {
                // remove all the tiles from the Grid
                gameBoard.Children.Clear();

                // assign a random number between 1 and 1,000,000 to each tile, then sort the tiles by number
                System.Random rgen = new System.Random();
                for (int t = 0; t < m_tiles.Count; t++)
                {
                    (m_tiles[t]).RandomIndex = rgen.Next(0, 1000000);
                }
                m_tiles.Sort();

                // assign the tiles to rows and columns
                int nTiles = 0;
                Tile at;

                for (int row = 0; row < c_Rows; row++)
                {
                    for (int column = 0; column < c_Columns; column++)
                    {
                        at = m_tiles[nTiles];
                        at.Row = row;
                        at.Column = column;
                        at.Clear = false;
                        at.Selected = false;
                        m_grid[row, column] = at;
                        Canvas.SetLeft(at, column * 55 + 55);
                        Canvas.SetTop(at, row * 55 + 55);
                        gameBoard.Children.Add(at);
                        nTiles++;
                    }
                }
            } while (!m_grid.IsWinnable());

            if (g_testmode)
            {
                foreach (Tile sometile in m_tiles)
                {
                    if (!((sometile.Number == 6)))
                        {
                        sometile.Clear = true;
                    }
                }
                if (g_losergame)
                {
                    m_grid[0, 0].Clear = false;
                }
            }


        }

        async private void hintButton_Click(object sender, RoutedEventArgs e)
        {
            //Scenario1Storyboard.Begin();
            Tile tile1 = null;
            Tile tile2 = null;
            if (m_grid.AnyPairsLeft(ref tile1, ref tile2))
            {
                m_grid.UnSelectAll();
                tile1.Selected = true;
                tile2.Selected = true;
                m_ignoreClicks = true;
                await System.Threading.Tasks.Task.Delay(1000);
                tile1.Selected = false;
                tile2.Selected = false;
                m_ignoreClicks = false;
            }

        }

        private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
            NewGame(false);
        }


        void BlankPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand cmd = new SettingsCommand("sample", "Options", (x) =>
                {
                    _settingsPopup = new Popup();
                    _settingsPopup.Closed += OnPopupClosed;
                    Window.Current.Activated += OnWindowActivated;
                    _settingsPopup.IsLightDismissEnabled = true;
                    _settingsPopup.Width = _settingsWidth;
                    _settingsPopup.Height = _windowBounds.Height;

                    ThemeSettingFlyout mypane = new ThemeSettingFlyout();
                    mypane.Width = _settingsWidth;
                    mypane.Height = _windowBounds.Height;
                    mypane.Animation = m_animate;
                    mypane.Sound = m_sound;

                    _settingsPopup.Child = mypane;
                    _settingsPopup.SetValue(Canvas.LeftProperty, _windowBounds.Width - _settingsWidth);
                    _settingsPopup.SetValue(Canvas.TopProperty, 0);
                    _settingsPopup.IsOpen = true;
                });

            args.Request.ApplicationCommands.Add(cmd);

            SettingsCommand help = new SettingsCommand("sample", "Rules", (x) =>
            {
                _settingsPopup = new Popup();
                _settingsPopup.Closed += OnPopupClosed;
                Window.Current.Activated += OnWindowActivated;
                _settingsPopup.IsLightDismissEnabled = true;
                _settingsPopup.Width = _settingsWidth;
                _settingsPopup.Height = _windowBounds.Height;

                HelpFlyout helpFlyout = new HelpFlyout();
                helpFlyout.Width = _settingsWidth;
                helpFlyout.Height = _windowBounds.Height;

                _settingsPopup.Child = helpFlyout;
                _settingsPopup.SetValue(Canvas.LeftProperty, _windowBounds.Width - _settingsWidth);
                _settingsPopup.SetValue(Canvas.TopProperty, 0);
                _settingsPopup.IsOpen = true;
            });

            args.Request.ApplicationCommands.Add(help);
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }
        }

        void OnPopupClosed(object sender, object e)
        {
            if (((Popup)sender).Child is ThemeSettingFlyout)
            {
                Window.Current.Activated -= OnWindowActivated;
                ThemeSettingFlyout mypane = (ThemeSettingFlyout)((Popup)sender).Child;
                // txtTheme.Text = (mypane.Theme == Theme.Woodblock) ? "Woodblock" : "Candy";
                m_animate = mypane.Animation;
                m_sound = mypane.Sound;
            }
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SettingsPane.Show();
        }

        Tile m_animationTile;
        int m_id = 1;
        private void AnimateLine1()
        {
            tileA.Selected = false;
            tileA.Clear = true;
            m_id++;
            m_animationTile = new Tile();
            m_animationTile.Image = tileA.Image;
            m_animationTile.ImageControl.Source = tileA.Image;
            m_animationTile.Clear = false;

            Canvas.SetTop(m_animationTile, m_animationPoints[0].Y * 55 + 55);
            Canvas.SetLeft(m_animationTile, m_animationPoints[0].X * 55 + 55);
            gameBoard.Children.Add(m_animationTile);

            // Create a duration of 2 seconds.
            // Duration duration = new Duration(TimeSpan.FromSeconds(2));
            Point point1 = m_animationPoints[0];
            Point point2 = m_animationPoints[1];
            double distance = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
            Duration duration = new Duration(TimeSpan.FromSeconds(distance / 6));


            // Create two DoubleAnimations and set their properties.
            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
            DoubleAnimation myDoubleAnimation2 = new DoubleAnimation();

            myDoubleAnimation1.Duration = duration;
            myDoubleAnimation2.Duration = duration;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;

            sb.Children.Add(myDoubleAnimation1);
            sb.Children.Add(myDoubleAnimation2);

            Storyboard.SetTarget(myDoubleAnimation1, m_animationTile);
            Storyboard.SetTarget(myDoubleAnimation2, m_animationTile);

            // Set the attached properties of Canvas.Left and Canvas.Top
            // to be the target properties of the two respective DoubleAnimations.
            Storyboard.SetTargetProperty(myDoubleAnimation1, "(Canvas.Left)");
            Storyboard.SetTargetProperty(myDoubleAnimation2, "(Canvas.Top)");

            myDoubleAnimation1.To = m_animationPoints[1].X * 55 + 55;
            myDoubleAnimation2.To = m_animationPoints[1].Y * 55 + 55;

            // Make the Storyboard a resource.
            gameBoard.Resources.Add(m_id.ToString(), sb);

            // Begin the animation.
            sb.Completed += new EventHandler<object>(AnimateLine2);

            sb.Begin();
        }



        private void AnimateLine2<EventArgs>(object sender, EventArgs e)
        {
            gameBoard.Resources.Remove(m_id.ToString());
            m_id++;

            // Create a duration of 2 seconds.
            //Duration duration = new Duration(TimeSpan.FromSeconds(2));
            Point point1 = m_animationPoints[1];
            Point point2 = m_animationPoints[2];
            double distance = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
            Duration duration = new Duration(TimeSpan.FromSeconds(distance / 6));

            // Create two DoubleAnimations and set their properties.
            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
            DoubleAnimation myDoubleAnimation2 = new DoubleAnimation();

            myDoubleAnimation1.Duration = duration;
            myDoubleAnimation2.Duration = duration;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;

            sb.Children.Add(myDoubleAnimation1);
            sb.Children.Add(myDoubleAnimation2);

            Storyboard.SetTarget(myDoubleAnimation1, m_animationTile);
            Storyboard.SetTarget(myDoubleAnimation2, m_animationTile);

            // Set the attached properties of Canvas.Left and Canvas.Top
            // to be the target properties of the two respective DoubleAnimations.
            Storyboard.SetTargetProperty(myDoubleAnimation1, "(Canvas.Left)");
            Storyboard.SetTargetProperty(myDoubleAnimation2, "(Canvas.Top)");

            myDoubleAnimation1.To = m_animationPoints[2].X * 55 + 55;
            myDoubleAnimation2.To = m_animationPoints[2].Y * 55 + 55;

            // Make the Storyboard a resource.
            gameBoard.Resources.Add(m_id.ToString(), sb);

            // Begin the animation.
            sb.Completed += new EventHandler<object>(AnimateLine3);

            sb.Begin();
        }



        private void AnimateLine3<EventArgs>(object sender, EventArgs e)
        {
            gameBoard.Resources.Remove(m_id.ToString());
            m_id++;

            // Create a duration of 2 seconds.
            //Duration duration = new Duration(TimeSpan.FromSeconds(2));
            Point point1 = m_animationPoints[2];
            Point point2 = m_animationPoints[3];
            double distance = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
            Duration duration = new Duration(TimeSpan.FromSeconds(distance / 6));

            // Create two DoubleAnimations and set their properties.
            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
            DoubleAnimation myDoubleAnimation2 = new DoubleAnimation();

            myDoubleAnimation1.Duration = duration;
            myDoubleAnimation2.Duration = duration;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;

            sb.Children.Add(myDoubleAnimation1);
            sb.Children.Add(myDoubleAnimation2);

            Storyboard.SetTarget(myDoubleAnimation1, m_animationTile);
            Storyboard.SetTarget(myDoubleAnimation2, m_animationTile);

            Storyboard.SetTargetProperty(myDoubleAnimation1, "(Canvas.Left)");
            Storyboard.SetTargetProperty(myDoubleAnimation2, "(Canvas.Top)");

            myDoubleAnimation1.To = m_animationPoints[3].X * 55 + 55;
            myDoubleAnimation2.To = m_animationPoints[3].Y * 55 + 55;

            // Make the Storyboard a resource.
            gameBoard.Resources.Add(m_id.ToString(), sb);

            // Begin the animation.
            sb.Completed += new EventHandler<object>(AnimateLine4);

            sb.Begin();
        }

        private void AnimateLine4<EventArgs>(object sender, EventArgs e)
        {
            gameBoard.Resources.Remove(m_id.ToString());
            gameBoard.Children.Remove(m_animationTile);
            tileB.Selected = false;
            tileB.Clear = true;
            m_ignoreClicks = false;
            CheckForEndOfGame();
        }


        private void CreateAnimationSteps(Tile tile1, Tile tile2, Path path)
        {
            tileA = tile1;
            tileB = tile2;

            m_animationPoints = new List<Point>();

            if (path.Direction == Direction.Horizontal)
            {
                // start position is location of tile1
                m_animationPoints.Add(new Point(tile1.Column, tile1.Row));

                // next location is horizontal move to the path
                m_animationPoints.Add(new Point(path.RowOrColumn, tile1.Row));

                // next location is a vertical move to row of tile2
                m_animationPoints.Add(new Point(path.RowOrColumn, tile2.Row));

                // final location is a horizontal move to the location tile2
                m_animationPoints.Add(new Point(tile2.Column, tile2.Row));
            }
            else
            {
                // start position is location of tile1
                m_animationPoints.Add(new Point(tile1.Column, tile1.Row));

                // next location is vertical move to the path
                m_animationPoints.Add(new Point(tile1.Column, path.RowOrColumn));

                // next location is a horizontal move to row of tile2
                m_animationPoints.Add(new Point(tile2.Column, path.RowOrColumn));

                // final location is a horizontal move to the location tile2
                m_animationPoints.Add(new Point(tile2.Column, tile2.Row));
            }
        }

        List<string> m_animationsIDs = new List<string>();
        private void GameLostAnimation()
        {
            txtGameOver1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            txtGameOver1.Text = "You lose!";
            m_animationsIDs.Clear();
            for (int row = 0; row < c_Rows; row++)
            {
                for (int column = 0; column < c_Columns; column++)
                {
                    Tile tile = m_grid[row, column];
                    if (!tile.Clear)
                    {
                        m_id++;
                        DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
                        DoubleAnimation myDoubleAnimation2 = new DoubleAnimation();
                        myDoubleAnimation1.Duration = new Duration(TimeSpan.FromSeconds(1));
                        myDoubleAnimation2.Duration = new Duration(TimeSpan.FromSeconds(1));
                        Storyboard sb = new Storyboard();
                        sb.Duration = new Duration(TimeSpan.FromSeconds(1));
                        sb.Children.Add(myDoubleAnimation1);
                        sb.Children.Add(myDoubleAnimation2);
                        Storyboard.SetTarget(myDoubleAnimation1, tile);
                        Storyboard.SetTarget(myDoubleAnimation2, tile);
                        Storyboard.SetTargetProperty(myDoubleAnimation1, "(Canvas.Left)");
                        Storyboard.SetTargetProperty(myDoubleAnimation2, "(Canvas.Top)");
                        myDoubleAnimation1.To = 300;
                        myDoubleAnimation2.To = 300;
                        gameBoard.Resources.Add(m_id.ToString(), sb);
                        m_animationsIDs.Add(m_id.ToString());
                        sb.Completed += new EventHandler<object>(ClearGameOverAnimations);
                        sb.Begin();
                    }
                }
            }
        }


        private void WinningGameAnimation()
        {
            txtGameOver1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            txtGameOver1.Text = "You win!";
            m_animationsIDs.Clear();
            m_id++;
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(6));
            gameBoard.Resources.Add(m_id.ToString(), sb);
            m_animationsIDs.Add(m_id.ToString());
            sb.Completed += new EventHandler<object>(ClearGameOverAnimations);

            for (int row = 0; row < c_Rows; row++)
            {
                for (int column = 0; column < c_Columns; column++)
                {
                    Tile tile = m_grid[row, column];
                    tile.Clear = false;
                    DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
                    myDoubleAnimation1.Duration = new Duration(TimeSpan.FromSeconds(4));
                    myDoubleAnimation1.BeginTime = TimeSpan.FromMilliseconds(2000 - (tile.Row + tile.Column) * 60);
                    sb.Children.Add(myDoubleAnimation1);
                    Storyboard.SetTarget(myDoubleAnimation1, tile);
                    Storyboard.SetTargetProperty(myDoubleAnimation1, "(Canvas.Top)");
                    myDoubleAnimation1.To = 1000;
                }
            }
            sb.Begin();
        }



        private void ClearGameOverAnimations<EventArgs>(object sender, EventArgs e)
        {
            m_grid.ClearAll();
            foreach (string id in m_animationsIDs)
            {
                gameBoard.Resources.Remove(id);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GameLostAnimation();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsPane.Show();
        }


    }
}

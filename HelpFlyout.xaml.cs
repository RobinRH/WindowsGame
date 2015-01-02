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
using Windows.UI.Xaml.Media.Animation;

namespace FourRivers
{
    public sealed partial class HelpFlyout : UserControl
    {
        public HelpFlyout()
        {
            this.InitializeComponent();
        }

        Image m_animationTile;
        int m_id = 1;
        private void btnNoTurns_Click(object sender, RoutedEventArgs e)
        {
            m_id++;
            tileAnimation.Source = tilePaw11055.Source;
            tileAnimation.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Canvas.SetTop(tileAnimation, 55);
            Canvas.SetLeft(tileAnimation, 110);
            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
            myDoubleAnimation1.Duration = new Duration(TimeSpan.FromSeconds(2));
            Storyboard sb = new Storyboard();
            sb.Duration = myDoubleAnimation1.Duration;
            sb.Children.Add(myDoubleAnimation1);
            Storyboard.SetTarget(myDoubleAnimation1, tileAnimation);
            Storyboard.SetTargetProperty(myDoubleAnimation1, "(Canvas.Top)");
            myDoubleAnimation1.To = 165;
            canHelp.Resources.Add(m_id.ToString(), sb);
            sb.Completed += new EventHandler<object>(NoTurnsEnded);
            tilePaw11055.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            sb.Begin();
        }

        private void NoTurnsEnded<EventArgs>(object sender, EventArgs e)
        {
            tileAnimation.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            tilePaw110165.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            tilePaw11055.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            btnNoTurns.IsEnabled = false;
        }

        private void btnOneTurn_Click(object sender, RoutedEventArgs e)
        {
            //Image tile1 = tileApple055;
            //Image tile2 = tileApple55165;

            m_id++;
            tileAnimation.Source = tileApple055.Source;
            tileAnimation.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Canvas.SetTop(tileAnimation, 55);
            Canvas.SetLeft(tileAnimation, 0);
            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
            myDoubleAnimation1.Duration = new Duration(TimeSpan.FromSeconds(2));
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(4));
            sb.Children.Add(myDoubleAnimation1);
            Storyboard.SetTarget(myDoubleAnimation1, tileAnimation);
            Storyboard.SetTargetProperty(myDoubleAnimation1, "(Canvas.Left)");
            myDoubleAnimation1.To = 55;

            DoubleAnimation secondLine = new DoubleAnimation();
            secondLine.Duration = new Duration(TimeSpan.FromSeconds(2));
            secondLine.BeginTime = TimeSpan.FromSeconds(2);
            sb.Children.Add(secondLine);
            Storyboard.SetTarget(secondLine, tileAnimation);
            Storyboard.SetTargetProperty(secondLine, "(Canvas.Top)");
            secondLine.To = 165;

            canHelp.Resources.Add(m_id.ToString(), sb);
            sb.Completed += new EventHandler<object>(OneTurnEnded);
            tileApple055.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            sb.Begin();
        }

        private void OneTurnEnded<EventArgs>(object sender, EventArgs e)
        {
            tileAnimation.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            tileApple055.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            tileApple55165.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            btnOneTurn.IsEnabled = false;
        }

        private void btnTwoTurns_Click(object sender, RoutedEventArgs e)
        {
            //Image tile1 = tileMountain00;
            //Image tile2 = tileMountain165110;

            m_id++;
            tileAnimation.Source = tileMountain00.Source;
            tileAnimation.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Canvas.SetTop(tileAnimation, 0);
            Canvas.SetLeft(tileAnimation, 0);
            DoubleAnimation line1 = new DoubleAnimation();
            line1.Duration = new Duration(TimeSpan.FromSeconds(2));
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(4));
            sb.Children.Add(line1);
            Storyboard.SetTarget(line1, tileAnimation);
            Storyboard.SetTargetProperty(line1, "(Canvas.Left)");
            line1.To = 55;

            DoubleAnimation line2 = new DoubleAnimation();
            line2.Duration = new Duration(TimeSpan.FromSeconds(2));
            line2.BeginTime = TimeSpan.FromSeconds(2);
            sb.Children.Add(line2);
            Storyboard.SetTarget(line2, tileAnimation);
            Storyboard.SetTargetProperty(line2, "(Canvas.Top)");
            line2.To = 110;

            canHelp.Resources.Add(m_id.ToString(), sb);
            sb.Completed += new EventHandler<object>(TwoTurnsEnded1);
            tileMountain00.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            sb.Begin();
        }

        private void TwoTurnsEnded1<EventArgs>(object sender, EventArgs e)
        {
            m_id++;
            tileAnimation.Source = tileMountain00.Source;
            tileAnimation.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Canvas.SetTop(tileAnimation, 110);
            Canvas.SetLeft(tileAnimation, 55);
            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
            myDoubleAnimation1.Duration = new Duration(TimeSpan.FromSeconds(2));
            Storyboard sb = new Storyboard();
            sb.Duration = myDoubleAnimation1.Duration;
            sb.Children.Add(myDoubleAnimation1);
            Storyboard.SetTarget(myDoubleAnimation1, tileAnimation);
            Storyboard.SetTargetProperty(myDoubleAnimation1, "(Canvas.Left)");
            myDoubleAnimation1.To = 165;
            canHelp.Resources.Add(m_id.ToString(), sb);
            sb.Completed += new EventHandler<object>(TwoTurnsEnded2);
            tileMountain00.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            sb.Begin();
        }

        private void TwoTurnsEnded2<EventArgs>(object sender, EventArgs e)
        {
            tileAnimation.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            tileMountain00.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            tileMountain165110.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            btnTwoTurns.IsEnabled = false;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            tileMountain165110.Visibility = Windows.UI.Xaml.Visibility.Visible;
            tileMountain00.Visibility = Windows.UI.Xaml.Visibility.Visible;
            tileApple055.Visibility = Windows.UI.Xaml.Visibility.Visible;
            tileApple55165.Visibility = Windows.UI.Xaml.Visibility.Visible;
            tilePaw110165.Visibility = Windows.UI.Xaml.Visibility.Visible;
            tilePaw11055.Visibility = Windows.UI.Xaml.Visibility.Visible;
            btnNoTurns.IsEnabled = true;
            btnOneTurn.IsEnabled = true;
            btnTwoTurns.IsEnabled = true;
        } 

    }
}

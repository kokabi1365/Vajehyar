using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Vajehyar.Utility
{
    class Utils
    {
        public static async void ShowToast(string message)
        {
            /*await Task.Factory.StartNew(() =>
            {
                MessageBox.Show(message);
                Task.Delay(2000);
            }).ContinueWith(
                t => 
            );*/
        }


    }

    public static class KeyboardFocus
    {
        public static readonly DependencyProperty OnProperty;

        public static void SetOn(UIElement element, FrameworkElement value)
        {
            element.SetValue(OnProperty, value);
        }

        public static FrameworkElement GetOn(UIElement element)
        {
            return (FrameworkElement)element.GetValue(OnProperty);
        }

        static KeyboardFocus()
        {
            OnProperty = DependencyProperty.RegisterAttached("On", typeof(FrameworkElement), typeof(KeyboardFocus), new PropertyMetadata(OnSetCallback));
        }

        private static void OnSetCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var frameworkElement = (FrameworkElement)dependencyObject;
            var target = GetOn(frameworkElement);

            if (target == null)
                return;

            frameworkElement.Loaded += (s, e) => Keyboard.Focus(target);
        }
    }
}

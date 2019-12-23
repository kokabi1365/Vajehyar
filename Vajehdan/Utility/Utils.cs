using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Vajehdan.Utility
{
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

    public static class Helper
    {
        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
                ? Application.Current.Windows.OfType<T>().Any()
                : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        
    }


}

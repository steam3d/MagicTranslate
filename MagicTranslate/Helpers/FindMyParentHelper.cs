using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace MagicTranslate.Helpers
{
    class FindMyParentHelper
    {
        public static class FindMyParent<T> where T : DependencyObject
        {
            public static T FindAncestor(DependencyObject dependencyObject)
            {
                var parent = VisualTreeHelper.GetParent(dependencyObject);

                if (parent == null) return null;

                var parentT = parent as T;
                return parentT ?? FindAncestor(parent);
            }
        }
    }
}

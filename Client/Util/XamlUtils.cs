using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Client.ViewModels;
using Client.Common;
using Entities;
using Client.Logging;

namespace Client.Util
{
    static public class ColorUtils
    {
        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
        }
    }

    static public class XamlUtils
    {
        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            
            try
            {
                 ;
                // Confirm parent and childName are valid. 
                if (parent == null) return null;

                T foundChild = null;

                int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    // If the child is not of the request child type child
                    T childType = child as T;
                    if (childType == null)
                    {
                        // recursively drill down the tree
                        foundChild = FindChild<T>(child, childName);

                        // If the child is found, break so we do not overwrite the found child. 
                        if (foundChild != null) break;
                    }
                    else if (!string.IsNullOrEmpty(childName))
                    {
                        var frameworkElement = child as FrameworkElement;
                        // If the child's name is set for search
                        if (frameworkElement != null && frameworkElement.Name == childName)
                        {
                            // if the child's name is of the request name
                            foundChild = (T)child;
                            break;
                        }
                    }
                    else
                    {
                        // child element found.
                        foundChild = (T)child;
                        break;
                    }
                     ;
                }

                return foundChild;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return null;
            }
        }

        //public class ControlledObservableCollection<T> : ObservableCollection<T>
        //{
        //    private List<ReactionParticipant> participants;
        //    private bool _notificationSupressed = false;
        //    private bool _supressNotification = false;

        //    public ControlledObservableCollection(List<ReactionParticipant> participants)
        //    {
        //        this.participants = participants;
        //    }

        //    public bool SupressNotification
        //    {
        //        get
        //        {
        //            return _supressNotification;
        //        }
        //        set
        //        {
        //            _supressNotification = value;
        //            if (_supressNotification == false && _notificationSupressed)
        //            {
        //                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //                _notificationSupressed = false;
        //            }
        //        }
        //    }

        //    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        //    {
        //        if (SupressNotification)
        //        {
        //            _notificationSupressed = true;
        //            return;
        //        }
        //        base.OnCollectionChanged(e);
        //    }
        //}
    }
}

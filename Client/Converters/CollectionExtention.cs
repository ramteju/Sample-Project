using Client.Common;
using Client.Logging;
using Client.ViewModels;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Converters
{
    static public class CollectionExtensions
    {
        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }


        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            
            try
            {
                IDictionary<string, object> expando = new ExpandoObject();
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
                {
                    var obj = propertyDescriptor.GetValue(anonymousObject);
                    expando.Add(propertyDescriptor.Name, obj);
                }
                return (ExpandoObject)expando;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
        public static ObservableCollection<ReactionVM> AddRange(this ObservableCollection<ReactionVM> list,List<ReactionVM> ListToAdd)
        {
            foreach (var reaction in ListToAdd)
                list.Add(reaction);
            return list;
        }
    }
}

using Entities;
using ProductTracking.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace ProductTracking.Util
{
    static public class CollectionExtensions
    {
        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            try
            {
                var i = 0;
                foreach (var e in ie) action(e, i++);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
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
                Log.Error(ex);
                throw;
            }
        }


        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            try
            {
                List<T> list = destination as List<T>;

                if (list != null)
                {
                    list.AddRange(source);
                }
                else
                {
                    foreach (T item in source)
                    {
                        destination.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public static string FullDetails(this Exception exception)
        {
            try
            {
                var properties = exception.GetType()
                                           .GetProperties();
                var fields = properties
                                 .Select(property => new
                                 {
                                     Name = property.Name,
                                     Value = property.GetValue(exception, null)
                                 })
                                 .Select(x => String.Format(
                                     "{0} = {1}",
                                     x.Name,
                                     x.Value != null ? x.Value.ToString() : String.Empty
                                 ));
                return String.Join("\n", fields);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

    }

    public class OrderedSet<T> : ICollection<T>
    {
        private readonly IDictionary<T, LinkedListNode<T>> m_Dictionary;
        private readonly LinkedList<T> m_LinkedList;

        public OrderedSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        public OrderedSet(IEqualityComparer<T> comparer)
        {
            m_Dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
            m_LinkedList = new LinkedList<T>();
        }

        public int Count
        {
            get { return m_Dictionary.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return m_Dictionary.IsReadOnly; }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            m_LinkedList.Clear();
            m_Dictionary.Clear();
        }

        public bool Remove(T item)
        {
            try
            {
                LinkedListNode<T> node;
                bool found = m_Dictionary.TryGetValue(item, out node);
                if (!found) return false;
                m_Dictionary.Remove(item);
                m_LinkedList.Remove(node);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_LinkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return m_Dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            try
            {
                m_LinkedList.CopyTo(array, arrayIndex);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public bool Add(T item)
        {
            try
            {
                if (m_Dictionary.ContainsKey(item)) return false;
                LinkedListNode<T> node = m_LinkedList.AddLast(item);
                m_Dictionary.Add(item, node);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }

}
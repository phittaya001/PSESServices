using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using AutoMapper;
using System.Collections;

namespace CSI.ModelHelper
{
    public class ObjectSnapshot
    {
        private Hashtable RootObjectMap = new Hashtable();
        private Hashtable SnapshotObjectMap = new Hashtable();
        private Dictionary<object, string[]> IgnoredProperties = new Dictionary<object, string[]>();
        private IMapper ObjectMapper;

        public ObjectSnapshot(IMapper mapper)
        {
            ObjectMapper = mapper;
        }

        ~ObjectSnapshot()
        {
            ResetSnapshot();
        }
        public virtual bool IsChanged(object obj)
        {
            return IsChanged(obj, null);
        }
        public virtual bool IsChanged(object obj, List<ChangedPropertyInfo> changedList)
        {
            if (false == SnapshotObjectMap.Contains(obj))
                return false;

            return false == AreObjectsEqual(obj, SnapshotObjectMap[obj], changedList);
        }

        public void Snap(object obj)
        {
            Type t = obj.GetType();

            object org = ObjectMapper.Map(obj, t, t);

            MapSubObjects(obj, org, SnapshotObjectMap);
            if (RootObjectMap.ContainsKey(obj))
            {
                object o = RootObjectMap[obj];
                t = o.GetType();
                if (typeof(IDisposable).IsAssignableFrom(t))
                    ((IDisposable)o).Dispose();
            }
            RootObjectMap[obj] = org;
        }
        public void ResetSnapshot()
        {
            SnapshotObjectMap.Clear();

            foreach (object o in RootObjectMap.Values)
            {
                Type t = o.GetType();
                if (typeof(IDisposable).IsAssignableFrom(t))
                    ((IDisposable)o).Dispose();
            }
            RootObjectMap.Clear();
        }
        public void SetIgnoredPropterties<T>(params string[] propertyName)
        {
            Type t = typeof(T);
            IgnoredProperties[t] = propertyName;
        }

        public List<T> GetChangedList<T>(IEnumerable<T> list)
        {
            List<T> changed = new List<T>();
            foreach (T o1 in list)
            {
                object o2 = GetOriginalObject<object>(o1);
                if (null != o2)
                {
                    if (false == AreObjectsEqual(o1, o2, null))
                        changed.Add(o1);
                }
                else
                    changed.Add(o1);
            }

            return changed;
        }

        public T GetOriginalObject<T>(T obj) where T : class
        {
            if (SnapshotObjectMap.ContainsKey(obj))
                return SnapshotObjectMap[obj] as T;

            return null;
        }

        public T RestoreOriginalObject<T>(T obj) where T : class
        {
            T org = GetOriginalObject(obj);
            if (null != org)
                ObjectMapper.Map<T, T>(org, obj);
            return obj;
        }

        private void MapSubObjects(object objectA, object objectB, Hashtable map, params Type[] ignoreList)
        {
            if (objectA != null && objectB != null)
            {
                Type objectTypeA = objectA.GetType();
                Type objectTypeB = objectB.GetType();

                if (CanDirectlyCompare(objectTypeA))
                    return;

                if (typeof(IEnumerable).IsAssignableFrom(objectTypeA))
                {
                    map[objectA] = objectB;

                    if (typeof(IDictionary).IsAssignableFrom(objectTypeA))
                    {
                        IDictionary dictA = ((IDictionary)objectA);
                        IDictionary dictB = ((IDictionary)objectB);
                        foreach (object k in dictA.Keys)
                            MapSubObjects(dictA[k], dictB[k], map, ignoreList);

                        MapSubObjects(dictA.Keys, dictB.Keys, map, ignoreList);
                    }
                    else
                    {
                        IEnumerable<object> enumA = ((IEnumerable)objectA).Cast<object>();
                        IEnumerable<object> enumB = ((IEnumerable)objectB).Cast<object>();
                        int enumCountA = enumA.Count();

                        for (int i = 0; i < enumCountA; i++)
                            MapSubObjects(enumA.ElementAt(i), enumB.ElementAt(i), map, ignoreList);
                    }
                }
                else if (objectTypeA.IsClass)
                {
                    map[objectA] = objectB;

                    foreach (PropertyInfo propertyInfoA in objectTypeA.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanRead && !ignoreList.Contains(p.PropertyType)))
                    {
                        if (CanDirectlyCompare(propertyInfoA.PropertyType))
                            continue;
                        try
                        {
                            PropertyInfo propertyInfoB = objectTypeB.GetProperty(propertyInfoA.Name, propertyInfoA.PropertyType);
                            if (null == propertyInfoB)
                                continue;

                            object valueA = propertyInfoA.GetValue(objectA, null);
                            object valueB = propertyInfoB.GetValue(objectB, null);
                            if (propertyInfoA.PropertyType.IsClass)
                                MapSubObjects(valueA, valueB, map, ignoreList);
                        }
                        catch (System.Reflection.TargetException)
                        {
                            continue;
                        }
                    }
                }
            }
        }

        public bool AreObjectsEqual(object objectA, object objectB, List<ChangedPropertyInfo> changedList)
        {
            bool result = false;

            if (objectA != null && objectB != null)
            {
                result = true; // assume by default they are equal
                Type objectTypeA = objectA.GetType();
                Type objectTypeB = objectB.GetType();

                if (CanDirectlyCompare(objectTypeA))
                    result = AreValuesEqual(objectA, objectB);
                else if (typeof(IEnumerable).IsAssignableFrom(objectTypeA))
                {
                    if (typeof(IDictionary).IsAssignableFrom(objectTypeA))
                    {
                        IDictionary dictA = ((IDictionary)objectA);
                        IDictionary dictB = ((IDictionary)objectB);

                        if (dictA.Keys.Count != dictB.Keys.Count)
                            result = false;
                        else if (null == changedList)
                        {
                            foreach (object k in dictA.Keys)
                                if (false == AreObjectsEqual(dictA[k], dictB[k], changedList))
                                {
                                    result = false;
                                    break;
                                }
                        }
                        else
                        {
                            foreach (object k in dictA.Keys)
                                result = result && AreObjectsEqual(dictA[k], dictB[k], changedList);
                        }
                    }
                    else
                    {
                        IEnumerable<object> enumA = ((IEnumerable)objectA).Cast<object>();
                        IEnumerable<object> enumB = ((IEnumerable)objectB).Cast<object>();
                        int enumCountA = enumA.Count();
                        int enumCountB = enumB.Count();

                        if (enumCountA != enumCountB)
                            result = false;
                        else if (null == changedList)
                        {
                            for (int i = 0; i < enumCountA; i++)
                                if (false == AreObjectsEqual(enumA.ElementAt(i), enumB.ElementAt(i), changedList))
                                {
                                    result = false;
                                    break;
                                }
                        }
                        else
                        {
                            for (int i = 0; i < enumCountA; i++)
                                result = result && AreObjectsEqual(enumA.ElementAt(i), enumB.ElementAt(i), changedList);
                        }
                    }
                }
                else if (objectTypeA.IsClass)
                {
                    string[] ignoredProperties = IgnoredProperties.ContainsKey(objectTypeA) ? IgnoredProperties[objectTypeA] : new string[0];

                    foreach (PropertyInfo propertyInfoA in objectTypeA.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                            .Where(p => false == ignoredProperties.Contains(p.Name)))
                    {
                        object valueA;
                        object valueB;
                        try
                        {
                            PropertyInfo propertyInfoB = objectTypeB.GetProperty(propertyInfoA.Name, propertyInfoA.PropertyType);
                            if (null == propertyInfoB)
                                continue;

                            valueA = propertyInfoA.GetValue(objectA, null);
                            valueB = propertyInfoB.GetValue(objectB, null);
                        }
                        catch (System.Reflection.TargetException)
                        {
                            continue;
                        }
                        // if it is a primative type, value type or implements
                        // IComparable, just directly try and compare the value
                        if (CanDirectlyCompare(propertyInfoA.PropertyType))
                            result = AreValuesEqual(valueA, valueB);
                        else if (typeof(IEnumerable).IsAssignableFrom(propertyInfoA.PropertyType))
                        {
                            // null check
                            if (valueA == null && valueB != null || valueA != null && valueB == null)
                                result = false;
                            else
                                result = AreObjectsEqual(valueA, valueB, changedList);
                        }
                        else if (propertyInfoA.PropertyType.IsClass)
                            result = AreObjectsEqual(valueA, valueB, changedList);
                        else
                            result = false;

                        if (false == result)
                        {
                            if (null != changedList)
                                changedList.Add(new ChangedPropertyInfo
                                {
                                    Object = objectA,
                                    Property = propertyInfoA,
                                    OldValue = valueB,
                                    NewValue = valueA,
                                });
                            else
                                break;
                        }
                    }
                }
            }
            else
                result = object.Equals(objectA, objectB);

            return result;
        }

        private bool CanDirectlyCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
        }

        private bool AreValuesEqual(object valueA, object valueB)
        {
            bool result;
            IComparable selfValueComparer;

            selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
                result = false; // one of the values is null
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                result = false; // the comparison using IComparable failed
            else if (!object.Equals(valueA, valueB))
                result = false; // the comparison using Equals failed
            else
                result = true; // match

            return result;
        }
    }

    public class ChangedPropertyInfo
    {
        public object Object { get; set; }
        public PropertyInfo Property { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}

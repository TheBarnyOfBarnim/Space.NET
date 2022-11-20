/*!
 * PylonSoftwareEngine - C# Library for creating Software/Games with DirectX (11)
 * https://github.com/PylonDev/PylonSoftwareEngine
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/PylonSoftwareEngine/blob/master/LICENSE.md
 */

/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space.NET.API.Utilities
{
    public class ObservableList<T> : List<T>
    {
        public delegate void Item_Added(T item);
        public event Item_Added On_ItemAdded;

        public delegate void Item_Removed(T item);
        public event Item_Removed On_ItemRemoved;

        public delegate void Item_Changed(T item);
        public event Item_Changed On_Item_Changed;

        public ObservableList()
        {
            On_ItemAdded += (x) => { };
            On_ItemRemoved += (x) => { };
            On_Item_Changed += (x) => { };
        }

        public ObservableList(List<T> list)
        {
            AddRange(list);

            On_ItemAdded += (x) => { };
            On_ItemRemoved += (x) => { };
            On_Item_Changed += (x) => { };
        }

        public T this[int index]
        {
            get
            {
               
                return base[index];
            }
            set
            {
                base[index] = value;
                ItemChanged(value);
            }
        }

        public new void Add(T item)
        {
            base.Add(item);
            On_ItemAdded(item);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                base.Add(item);
                On_ItemAdded(item);
            }
        }

        public new bool Remove(T item)
        {
            bool succeed = base.Remove(item);
            if(succeed)
                On_ItemRemoved(item);
            return succeed;
        }

        public new void UnsafeAdd(T item)
        {
            base.Add(item);
        }

        public new void UnsafeAddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                base.Add(item);
            }
        }

        public new bool UnsafeRemove(T item)
        {
            bool succeed = base.Remove(item);
            return succeed;
        }

        public new void Clear()
        {
            foreach (var item in this)
            {
                On_ItemRemoved(item);
            }

            base.Clear();
        }

        public void ItemChanged(T item)
        {
            On_Item_Changed(item);
        }
    }
}

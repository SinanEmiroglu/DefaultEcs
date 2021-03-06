﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DefaultEcs.Serialization;
using DefaultEcs.Technical.Helper;

namespace DefaultEcs.Technical.Serialization.TextSerializer
{
    internal sealed class ComponentTypeWriter : IComponentTypeReader
    {
        #region Fields

        private readonly StreamWriter _writer;
        private readonly Dictionary<Type, string> _types;
        private readonly int _worldMaxCapacity;
        private readonly Predicate<Type> _componentFilter;

        #endregion

        #region Initialisation

        public ComponentTypeWriter(StreamWriter writer, Dictionary<Type, string> types, int worldMaxCapacity, Predicate<Type> componentFilter)
        {
            _writer = writer;
            _types = types;
            _worldMaxCapacity = worldMaxCapacity;
            _componentFilter = componentFilter;
        }

        #endregion

        #region IComponentTypeReader

        void IComponentTypeReader.OnRead<T>(int maxCapacity)
        {
            if (_componentFilter(typeof(T)))
            {
                string shortName = typeof(T).Name;

                int repeatCount = 1;
                while (_types.ContainsValue(shortName))
                {
                    shortName = $"{typeof(T).Name}_{repeatCount++}";
                }

                _types.Add(typeof(T), shortName);

                _writer.WriteLine($"{nameof(EntryType.ComponentType)} {shortName} {TypeNames.Get(typeof(T))}");
                if (maxCapacity != _worldMaxCapacity && !typeof(T).GetTypeInfo().IsFlagType())
                {
                    _writer.WriteLine($"{nameof(EntryType.ComponentMaxCapacity)} {shortName} {maxCapacity}");
                }
            }
        }

        #endregion
    }
}

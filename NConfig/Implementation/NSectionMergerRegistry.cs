using System.Configuration;
using System;
using System.Collections.Generic;

namespace NConfig
{
    internal class NSectionMergerRegistry : INSectionMergerRegistry
    {
        private readonly Dictionary<Type, NSectionMerger> registry = new Dictionary<Type, NSectionMerger>();
        private readonly DefaultMerger defaultMerger = new DefaultMerger();


        public void AddMerger(Type sectionType, NSectionMerger merger)
        {
            registry[sectionType] = merger;
        }

        public NSectionMerger GetMerger(Type sectionType)
        {
            NSectionMerger result;
            registry.TryGetValue(sectionType, out result);
            return result ?? defaultMerger;
        }


    }

}

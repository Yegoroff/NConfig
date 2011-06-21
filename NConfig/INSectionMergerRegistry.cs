using System;

namespace NConfig
{
    internal interface INSectionMergerRegistry
    {
        void AddMerger(Type sectionType, NSectionMerger merger);
        
        NSectionMerger GetMerger(Type sectionType);
    }

}

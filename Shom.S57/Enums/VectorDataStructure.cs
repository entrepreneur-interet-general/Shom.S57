using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S57
{
    public enum VectorDataStructure : uint
    {
        CartographicSpaghetti = 1,
        ChainNode = 2,
        PlanarGraph = 3,
        FullTopology = 4,
        NotRelevant = 255
    }

}

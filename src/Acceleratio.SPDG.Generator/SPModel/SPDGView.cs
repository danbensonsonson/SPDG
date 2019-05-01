using System;

namespace Acceleratio.SPDG.Generator.SPModel
{
    public struct SPDGView
    {
        public string Title { get; private set; }
        public SPDGView(string title)
        {
            Title = title;
        }
    }
}

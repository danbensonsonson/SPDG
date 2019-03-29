using System.Collections.Generic;

namespace Acceleratio.SPDG.Generator.SPModel
{
    public interface ISPDGListItemInfo
    {
        object this[string name] { get; set; }
        IEnumerable<string> GetAvailableFields();
        ISPDGListItemAttachmentInfo Attachment { get; set; }
    }

    public interface ISPDGListItemAttachmentInfo
    {
        byte[] Content { get; set; }
        string Name { get; set; }
    }
    
}

using System.Collections.Generic;
using System.Linq;

namespace Acceleratio.SPDG.Generator.SPModel
{
    class SPDGListItemInfo : ISPDGListItemInfo
    {
        readonly Dictionary<string, object> _fieldValues=new Dictionary<string, object>();

        public object this[string fieldName]
        {
            get { return _fieldValues[fieldName]; }
            set { _fieldValues[fieldName] = value; }
        }

        public IEnumerable<string> GetAvailableFields()
        {
            return _fieldValues.Keys.ToList();
        }

        public ISPDGListItemAttachmentInfo Attachment { get; set; }
    }

    class SPDGListItemAttachmentInfo : ISPDGListItemAttachmentInfo
    {
        string _name;
        byte[] _content;

        public SPDGListItemAttachmentInfo()
        { }

        public SPDGListItemAttachmentInfo(string name, byte[] content)
        {
            _name = name;
            _content = content;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public byte[] Content
        {
            get { return _content; }
            set { _content = value; }
        }


    }
}

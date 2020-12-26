using System;

namespace BLL
{
    public class Change
    {
        public DateTime Time { get; private set; }
        public (object, object) ChangedProperty { get; private set; }
        public Change(object before, object after)
        {
            this.Time = DateTime.Now;
            this.ChangedProperty = (before, after);
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace EasyInsur.Models
{
    public class EditableTableClass : IEditableObject
    {
        private Dictionary<string, object>? _storedValues;
        protected Dictionary<string, object> BackUp()
        {
            var itemProperties = GetType().GetTypeInfo().DeclaredProperties;

            return itemProperties.Where(pDescriptor => pDescriptor.CanWrite)
                .ToDictionary(pDescriptor => pDescriptor.Name,
                    pDescriptor => pDescriptor.GetValue(this))!;
        }
        public void BeginEdit() => _storedValues = BackUp();

        public void CancelEdit()
        {
            if (_storedValues == null)
                return;

            foreach (var (key, value) in _storedValues)
            {
                var itemProperties = GetType().GetTypeInfo().DeclaredProperties;
                var pDesc = itemProperties.FirstOrDefault(p => p.Name == key);

                if (pDesc != null)
                    pDesc.SetValue(this, value);
            }
        }

        public void EndEdit()
        {
            if (_storedValues == null) return;
            _storedValues.Clear();
            _storedValues = null;
        }

        public List<string> EditedColumns = new();
    }
}

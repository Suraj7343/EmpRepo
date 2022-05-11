using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.BAL.Models
{
    public class KeyValueModel<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public bool IsChecked { get; internal set; }
    }
}

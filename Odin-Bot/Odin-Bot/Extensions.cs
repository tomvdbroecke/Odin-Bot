using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin_Bot.Extensions {
    public static class Extensions {
        public static bool IsNullOrEmpty(this Array array) {
            return (array == null || array.Length == 0);
        }
    }
}

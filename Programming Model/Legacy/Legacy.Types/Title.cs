using Legacy.Types;
using NakedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Types
{
    public class Title
    {
        private readonly StringBuilder stringBuilder;

        public Title() => stringBuilder = new StringBuilder();

        public Title(object text) : this() => stringBuilder.Append(text);

        public Title(TitledObject obj) : this() => stringBuilder.Append(TitleString(obj));

        public Title(TitledObject obj, string defaultValue) : this()
        {
            if (TitleString(obj).Length is 0)
            {
                stringBuilder.Append(defaultValue);
            }
            else
            {
                stringBuilder.Append(obj.Title());
            }
        }

        public override string ToString() => stringBuilder.ToString();

        public static string TitleString(TitledObject obj) => obj?.ToString() ?? "";
    }
}

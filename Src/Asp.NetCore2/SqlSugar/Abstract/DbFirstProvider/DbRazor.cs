using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class RazorFirst
    {
        internal List<KeyValuePair<string,string>> ClassStringList { get;  set; }
        internal Func<string, string> FormatFileNameFunc { get; set; }

        public static string DefaultRazorClassTemplate =
@"using System;
using System.Linq;
using System.Text;
using SqlSugar;
namespace @Model.Namespace 
{
    ///<summary>
    ///
    ///</summary>
    [SqlSugar.SugarTable(""@Model.DbTableName"")]
    public partial class @Model.ClassName
    {
        public @(Model.ClassName)(){ }

    @foreach (var item in @Model.Columns)
    {
        @:/// <summary>
        @:/// Desc:@item.ColumnDescription
        @:/// Default:@item.DefaultValue
        @:/// Nullable:@item.IsNullable
        @:/// </summary>     
        @:[SqlSugar.SugarColumn(ColumnName = ""@item.DbColumnName"", IsPrimaryKey = @(item.IsPrimarykey?""true"":""false""), IsIdentity = @(item.IsIdentity?""true"":""false""))]      
        @:public @item.DataType @item.PropertyName { get; set; }
    }

    }
}";

        public void CreateClassFile(string directoryPath)
        {
            var seChar = Path.DirectorySeparatorChar.ToString();
            if (ClassStringList.HasValue())
            {
                foreach (var item in ClassStringList)
                {
                    var fileName = item.Key;
                    if (this.FormatFileNameFunc != null) 
                    {
                        fileName = this.FormatFileNameFunc(fileName);
                    }
                    var filePath = directoryPath.TrimEnd('\\').TrimEnd('/') + string.Format(seChar + "{0}.cs",fileName);
                    FileHelper.CreateFile(filePath, item.Value, Encoding.UTF8);
                }
            }
        }
        public List<KeyValuePair<string, string>> GetClassStringList()
        {
            return ClassStringList;
        }
    }
}

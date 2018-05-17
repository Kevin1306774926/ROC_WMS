using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROC.Models
{
    public partial class MyDbContext
    {
        partial void InitializePartial()
        {
            this.Configuration.LazyLoadingEnabled = false;
            DisplaySQL();
        }

        public void DisplaySQL()
        {
#if DEBUG
            this.Database.Log = new Action<string>(t => Debug.WriteLine(t));
            //this.Database.Log = Console.Write;
            //this.Database.Log = log => File.AppendAllText("ef.log", string.Format("{0} {1} {2}", DateTime.Now, Environment.NewLine, log));
#endif
        }
    }
}

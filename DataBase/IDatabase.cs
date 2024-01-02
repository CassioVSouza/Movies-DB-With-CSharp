using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    internal interface IDatabase
    {
        void InitializeDataBase();
        void CloseDatabase();
    }
}

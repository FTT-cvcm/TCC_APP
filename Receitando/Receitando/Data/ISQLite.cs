using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Receitando.Data
{
    public interface ISQLite
    {
        SQLiteConnection PegarConnection();
    }
}

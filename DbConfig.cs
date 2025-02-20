using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBlocker
{
    public static class DbConfig
    {
        public static string? SqlUid = Environment.GetEnvironmentVariable("DB_APP_BLOCKER_UID");
        public static string? SqlPassword = Environment.GetEnvironmentVariable("DB_APP_BLOCKER_PASSWORD");
    }
}
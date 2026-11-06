using System;
using System.ComponentModel;

namespace ioschools.Data.User
{
    /// <summary>
    /// Bit-flag enumeration of user roles within the ioSchools system.
    /// Using [Flags] allows a single user to belong to multiple groups simultaneously,
    /// which is required by the group-based action filter.
    /// Migrated from legacy ASP.NET MVC UserGroup enum — no value changes required.
    /// WARNING: These values are persisted in the database as a 32-bit integer.
    /// Do NOT change existing flag values without a corresponding database migration.
    /// IF YOU WANT TO REPLACE A ROLE, CHECK IN DB THAT IT IS NOT CURRENTLY IN USE.
    /// </summary>
    [Flags]
    public enum UserGroup : int
    {
        NONE        = 0x0000,
        DIRECTOR    = 0x0001,
        HEAD        = 0x0002,
        ADMIN       = 0x0004,
        TEACHER     = 0x0008,
        FINANCE     = 0x0020,
        CLERK       = 0x0040,
        STUDENT     = 0x0080,
        GUARDIAN    = 0x0100,
        SUPPORT     = 0x0200,
        SUPERUSER   = 0x0400
    }
}

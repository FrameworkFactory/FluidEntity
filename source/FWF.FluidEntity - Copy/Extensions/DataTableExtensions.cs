using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace FWF.FluidEntity.Extensions
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// Dynamically create data columns based on the entity's type info, then add rows to the table from those entities.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="entities"></param>
         public static void Load<T>(this DataTable dt, IEnumerable<T> entities)
            where T : class
        {
             dt.Load<T, object>(entities, null);
        }

        /// <summary>
        /// Dynamically create data columns based on the entity's type info, then add rows to the table from those entities.
        /// Also able to specify which property member should be the primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPk"></typeparam>
        /// <param name="dt"></param>
        /// <param name="entities"></param>
        /// <param name="primaryKeyMember"></param>
        public static void Load<T, TPk>(this DataTable dt, IEnumerable<T> entities, Expression<Func<T, TPk>> primaryKeyMember)
            where T : class
        {
            dt.Load<T, TPk, object>(entities, primaryKeyMember, new List<Expression<Func<T, object>>>());
        }

        /// <summary>
        /// Dynamically create data columns based on the entity's type info, then add rows to the table from those entities.
        /// Also able to specify which property member should be the primary key and which member should be ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPk"></typeparam>
        /// <typeparam name="TIm"></typeparam>
        /// <param name="dt"></param>
        /// <param name="entities"></param>
        /// <param name="primaryKeyMember"></param>
        /// <param name="ignoreMember"></param>
        public static void Load<T, TPk, TIm>(this DataTable dt, IEnumerable<T> entities, 
            Expression<Func<T, TPk>> primaryKeyMember, Expression<Func<T, TIm>> ignoreMember)
            where T : class
        {
            var props = typeof(T).GetProperties();

            var pkExp = (primaryKeyMember != null ? primaryKeyMember.Body as MemberExpression : null);
            var pkMemberName = (pkExp != null ? pkExp.Member.Name : null);

            var ignoreExp = (ignoreMember != null ? ignoreMember.Body as MemberExpression : null);
            var ignoreMemberName = (ignoreExp != null ? ignoreExp.Member.Name : null);

            // create the table columns
            foreach (var prop in props.Where(x => x.Name != ignoreMemberName))
            {
               var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

               var column = dt.Columns.Add(prop.Name, propType);

                if (!dt.PrimaryKey.Any() && prop.Name == pkMemberName) // is this the primary key?
                {
                    column.Unique = true;
                    column.AllowDBNull = false;
                    dt.PrimaryKey = new[] {column};
                }
            }

            if (entities != null)
            {
                // add new rows from the entities
                foreach (var entity in entities)
                {
                    var row = dt.NewRow();

                    foreach (var prop in props.Where(x => x.Name != ignoreMemberName && x.GetMethod != null))
                    {
                        var propValue = prop.GetValue(entity);
                        row[prop.Name] = propValue ?? DBNull.Value;
                    }

                    dt.Rows.Add(row);
                }
            }
        }

        /// <summary>
        /// Dynamically create data columns based on the entity's type info, then add rows to the table from those entities.
        /// Also able to specify which property member should be the primary key and which member should be ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPk"></typeparam>
        /// <typeparam name="TIm"></typeparam>
        /// <param name="dt"></param>
        /// <param name="entities"></param>
        /// <param name="primaryKeyMember"></param>
        /// <param name="ignoreMembers"></param>
        public static void Load
            <T, TPk, TIm>(this DataTable dt, IEnumerable<T> entities,
            Expression<Func<T, TPk>> primaryKeyMember, List<Expression<Func<T, TIm>>> ignoreMembers)
            where T : class
        {
            var props = typeof(T).GetProperties();

            var pkExp = (primaryKeyMember != null ? primaryKeyMember.Body as MemberExpression : null);
            var pkMemberName = (pkExp != null ? pkExp.Member.Name : null);

            List<string> ignoreMemberNames = new List<string>();

            foreach (var ignoreMember in ignoreMembers)
            {
                ignoreMemberNames.Add(GetMemberName(ignoreMember));
            }

            // create the table columns
            foreach (var prop in props.Where(x => !ignoreMemberNames.Contains(x.Name)))
            {
                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                var column = dt.Columns.Add(prop.Name, propType);

                if (!dt.PrimaryKey.Any() && prop.Name == pkMemberName) // is this the primary key?
                {
                    column.Unique = true;
                    column.AllowDBNull = false;
                    dt.PrimaryKey = new[] { column };
                }
            }

            if (entities != null)
            {
                // add new rows from the entities
                foreach (var entity in entities)
                {
                    var row = dt.NewRow();

                    foreach (var prop in props.Where(x => !ignoreMemberNames.Contains(x.Name) && x.GetMethod != null))
                    {
                        var propValue = prop.GetValue(entity);
                        row[prop.Name] = propValue ?? DBNull.Value;
                    }

                    dt.Rows.Add(row);
                }
            }
        }
        /// <summary>
        /// Gets MemberName from expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIm"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        private static string GetMemberName<T,TIm>(Expression<Func<T, TIm>> member)
        {
            var memberExp = (member != null ? member.Body as MemberExpression : null);
            var memberName = (memberExp != null ? memberExp.Member.Name : null);

            return memberName;
        }
    }
}

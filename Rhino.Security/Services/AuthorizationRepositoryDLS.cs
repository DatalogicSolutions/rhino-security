using System;
using System.Linq;
using NHibernate.Criterion;
using Rhino.Security.Impl;
using Rhino.Security.Model;
using Rhino.Security.Impl.Util;

namespace Rhino.Security.Services
{
    public partial class AuthorizationRepository
    {
        /// <summary>
        /// Gets all users groups
        /// </summary>
        public virtual UsersGroup[] GetUsersGroups()
        {
            return session.CreateCriteria<UsersGroup>()
                .AddOrder(Order.Asc("Name"))
                .SetCacheable(true)
                .List<UsersGroup>().ToArray();
        }

        /// <summary>
        /// Gets all operations
        /// </summary>
        public Operation[] GetOperations()
        {
            return session.CreateCriteria<Operation>()
            .SetCacheable(true)
            .List<Operation>().ToArray();
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        public Permission[] GetPermissions()
        {
            return session.CreateCriteria<Permission>()
             .SetCacheable(true)
             .List<Permission>().ToArray();
        }

        /// <summary>
        /// Gets the users group by its id
        /// </summary>
        /// <param name="userGroupId">Id of the group.</param>
        public UsersGroup GetUsersGroupById(Guid userGroupId)
        {
            return session.CreateCriteria<UsersGroup>()
             .Add(Restrictions.Eq("Id", userGroupId))
             .SetCacheable(true)
             .UniqueResult<UsersGroup>();
        }

        /// <summary>
        /// Gets the opertaion by its id
        /// </summary>
        /// <param name="operationId">Id of the group.</param>
        public Operation GetOperationById(Guid operationId)
        {
            return session.CreateCriteria<Operation>()
             .Add(Restrictions.Eq("Id", operationId))
             .SetCacheable(true)
             .UniqueResult<Operation>();
        }

        /// <summary>
        /// Gets the permission by its id
        /// </summary>
        /// <param name="permissionId">Id of the group.</param>
        public Permission GetPermissionById(Guid permissionId)
        {
            return session.CreateCriteria<Permission>()
                .Add(Restrictions.Eq("Id", permissionId))
                .SetCacheable(true)
                .UniqueResult<Permission>();
        }

        /// <summary>
        /// Gets all operations by users group
        /// </summary>
        /// <param name="usersGroupName">Name of the users group.</param>
        /// <returns></returns>
        public Operation[] GetOperationsByUsersGroup(string usersGroupName)
        {
            //SubQueries 
            var userGroupIds = DetachedCriteria.For<UsersGroup>()
                .SetProjection(Projections.Distinct(Projections.Property("Id")))
                .Add(Restrictions.Eq("Name", usersGroupName));

            var permissionIds = DetachedCriteria.For<Permission>()
                .SetProjection(Projections.Distinct(Projections.Property("Operation.Id")))
                .Add(Subqueries.PropertyIn("UsersGroup.Id", userGroupIds));

            //Joins
            /*var usersGroupIdsFromPermissions = DetachedCriteria.For<Permission>()
                .SetProjection(Projections.Distinct(Projections.Property("Operation.Id")))
                .CreateCriteria("UsersGroups", JoinType.InnerJoin)
                .Add(Restrictions.Eq("Name", usersGroupName));
*/
            var operations = DetachedCriteria.For<Operation>()
                .Add(Subqueries.PropertyIn("Id", permissionIds));

            return operations.GetExecutableCriteria(session).List<Operation>().ToArray();
        }


        /// <summary>
        /// Creates the operation with the given name and comment
        /// </summary>
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="comment">Description of the operation.</param>
        /// <returns></returns>
        /* Code duplication [ CreateOperation(string operationName) can call this method with empty comment
         * but was not modified to keep the trunck version clean ] */
        public virtual Operation CreateOperation(string operationName, string comment)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(operationName), "operationName must have a value");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(comment), "comment must have a value");
            Guard.Against<ArgumentException>(operationName[0] != '/', "Operation names must start with '/'");

            var op = new Operation { Name = operationName, Comment = comment};

            var parentOperationName = Strings.GetParentOperationName(operationName);
            if (parentOperationName != string.Empty) //we haven't got to the root
            {
                var parentOperation = GetOperationByName(parentOperationName) ??
                                      CreateOperation(parentOperationName);

                parentOperation.Comment = string.IsNullOrEmpty(parentOperation.Comment)
                                              ? parentOperationName.Substring(1) + " operations"
                                              : comment;
                

                op.Parent = parentOperation;
                parentOperation.Children.Add(op);
            }

            session.Save(op);
            return op;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="operationName"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public virtual Operation UpdateOperation(Guid operationId, string operationName, string comment)
        {
            var operation = GetOperationById(operationId);
            if (operation != null)
            {
                session.Save(operation);
            }
            return operation;
        }
    }
}

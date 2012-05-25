using System;
using Rhino.Security.Model;

namespace Rhino.Security.Interfaces
{
	/// <summary>
	/// Allows to edit the security information of the 
	/// system
	/// </summary>
	public partial interface IAuthorizationRepository
	{
        /// <summary>
        /// Gets all users groups
        /// </summary>
        UsersGroup[] GetUsersGroups();

        /// <summary>
        /// Gets all operations
        /// </summary>
        Operation[] GetOperations();

        /// <summary>
        /// Gets the users group by its id
        /// </summary>
        /// <param name="userGroupId">Id of the group.</param>
	    UsersGroup GetUsersGroupById(Guid userGroupId);

        /// <summary>
        /// Gets all operations by users group
        /// </summary>
        /// <param name="usersGroupName">Name of the users group.</param>
        /// <returns></returns>
        Operation[] GetOperationsByUsersGroup(string usersGroupName);

	    /// <summary>
	    /// Gets the opertaion by its id
	    /// </summary>
	    /// <param name="operationId">Id of the group.</param>
	    Operation GetOperationById(Guid operationId);

	    /// <summary>
	    /// Gets all permissions
	    /// </summary>
	    Permission[] GetPermissions();

	    /// <summary>
	    /// Gets the permission by its id
	    /// </summary>
	    /// <param name="permissionId">Id of the group.</param>
	    Permission GetPermissionById(Guid permissionId);

	    /// <summary>
	    /// Creates the operation with the given name and comment
	    /// </summary>
	    /// <param name="operationName">Name of the operation.</param>
	    /// <param name="comment">Description of the operation.</param>
	    /// <returns></returns>
	    Operation CreateOperation(string operationName, string comment);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="operationName"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
	    Operation UpdateOperation(Guid operationId, string operationName, string comment);
	}
}

using System;
using System.ComponentModel;

namespace CoreCommon.Data.Domain.Attributes
{
    /// <summary>
    /// UserRolePageAction.
    /// </summary>
    public enum UserRolePageAction : int
    {
        /// <summary>
        /// None.
        /// </summary>
        [Description("")]
        None = 0,

        /// <summary>
        /// View.
        /// </summary>
        [Description("")]
        View = 1,

        /// <summary>
        /// Edit.
        /// </summary>
        [Description("")]
        Edit = 2,

        /// <summary>
        /// Delete.
        /// </summary>
        [Description("")]
        Delete = 3,

        /// <summary>
        /// Create.
        /// </summary>
        [Description("")]
        Create = 4,
    }

    /// <summary>
    /// UserAuthorizeAttribute.
    /// </summary>
    public class UserAuthorizeAttribute : Attribute
    {
    }

    /// <summary>
    /// UserAllowAnonymousAttribute.
    /// </summary>
    public class UserAllowAnonymousAttribute : Attribute
    {
    }

    /// <summary>
    /// RoleActionAttribute.
    /// </summary>
    public class RoleActionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleActionAttribute"/> class.
        /// </summary>
        /// <param name="moduleKey">ModuleKey.</param>
        /// <param name="pageKey">PageKey.</param>
        /// <param name="actionKey">ActionKey.</param>
        public RoleActionAttribute(string moduleKey, string pageKey, string actionKey)
        {
            ModuleKey = moduleKey;
            PageKey = pageKey;
            ActionKey = actionKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleActionAttribute"/> class.
        /// </summary>
        /// <param name="moduleKey">ModuleKey.</param>
        /// <param name="pageKey">PageKey.</param>
        public RoleActionAttribute(string moduleKey, string pageKey)
        {
            ModuleKey = moduleKey;
            PageKey = pageKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleActionAttribute"/> class.
        /// </summary>
        /// <param name="actionKey">ActionKey.</param>
        public RoleActionAttribute(string actionKey)
        {
            ActionKey = actionKey;
        }

        /// <summary>
        /// Gets or sets ModuleKey.
        /// </summary>
        public string ModuleKey { get; set; }

        /// <summary>
        /// Gets or sets PageKey.
        /// </summary>
        public string PageKey { get; set; }

        /// <summary>
        /// Gets or sets ActionKey.
        /// </summary>
        public string ActionKey { get; set; }
    }

    /// <summary>
    /// UserRolePageAttribute.
    /// </summary>
    public class UserRolePageAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRolePageAttribute"/> class.
        /// </summary>
        /// <param name="pageRoleAction">Action.</param>
        /// <param name="pageName">Page Name.</param>
        public UserRolePageAttribute(UserRolePageAction pageRoleAction, string pageName)
        {
            Action = (int)pageRoleAction;
            PageName = pageName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRolePageAttribute"/> class.
        /// </summary>
        /// <param name="pageRoleAction">Action.</param>
        public UserRolePageAttribute(UserRolePageAction pageRoleAction)
        {
            Action = (int)pageRoleAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRolePageAttribute"/> class.
        /// </summary>
        /// <param name="pageRoleAction">Action.</param>
        public UserRolePageAttribute(int pageRoleAction)
        {
            Action = pageRoleAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRolePageAttribute"/> class.
        /// </summary>
        /// <param name="pageRoleAction">Action.</param>
        /// <param name="pageName">Page Name.</param>
        public UserRolePageAttribute(int pageRoleAction, string pageName)
        {
            Action = pageRoleAction;
            PageName = pageName;
        }

        /// <summary>
        /// Gets or sets action.
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets PageName.
        /// </summary>
        public string PageName { get; set; }
    }
}

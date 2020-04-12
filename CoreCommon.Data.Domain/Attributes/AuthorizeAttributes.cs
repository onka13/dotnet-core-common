using System;
using System.ComponentModel;

namespace CoreCommon.Data.Domain.Attributes
{
    public class UserAuthorizeAttribute : Attribute
    {
    }

    public class UserAllowAnonymousAttribute : Attribute
    {
    }

    public class RoleActionAttribute : Attribute
    {
        public string ModuleKey { get; set; }
        public string PageKey { get; set; }
        public string ActionKey { get; set; }

        public RoleActionAttribute(string moduleKey, string pageKey, string actionKey)
        {
            ModuleKey = moduleKey;
            PageKey = pageKey;
            ActionKey = actionKey;
        }

        public RoleActionAttribute(string moduleKey, string pageKey)
        {
            ModuleKey = moduleKey;
            PageKey = pageKey;
        }

        public RoleActionAttribute(string actionKey)
        {
            ActionKey = actionKey;
        }
    }

    public class UserRolePageAttribute : Attribute
    {
        public int Action { get; set; }
        public string PageName { get; set; }

        public UserRolePageAttribute(UserRolePageAction pageRoleAction)
        {
            Action = (int)pageRoleAction;
        }
        public UserRolePageAttribute(UserRolePageAction pageRoleAction, string pageName)
        {
            Action = (int)pageRoleAction;
            PageName = pageName;
        }
        public UserRolePageAttribute(int pageRoleAction)
        {
            Action = pageRoleAction;
        }
        public UserRolePageAttribute(int pageRoleAction, string pageName)
        {
            Action = pageRoleAction;
            PageName = pageName;
        }
    }

    public enum UserRolePageAction : int
    {

        [Description("")]
        None = 0,
        [Description("")]
        View = 1,
        [Description("")]
        Edit = 2,
        [Description("")]
        Delete = 3,
        [Description("")]
        Create = 4,
    }
}

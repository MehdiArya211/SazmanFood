using FajrLog.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FajrLog.Enum
{
    /// <summary>
    /// انواع عملیات های لاگ فجر
    /// </summary>
    public enum FajrActionType
    {
        #region userActivity - فعالیت های انجام شده توسط کاربر
        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "createCourse", Desc = "افزودن دوره")]
        createCourse = 1125,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "logIn", Desc = "لاگین")]
        logIn = 1101,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "logOut", Desc = "لاگ اوت")]
        logOut = 1102,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "logInSecurePage", Desc = "لاگین در صفحاتی که نیاز به لاگین مجدد دارند")]
        logInSecurePage = 1103,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "logOutSecurePage", Desc = "لاگ اوت در صفحاتی که نیاز به لاگین مجدد دارند")]
        logOutSecurePage = 1104,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "changePassword", Desc = "تغییر کمله عبور")]
        changePassword = 1105,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "changeGroup", Desc = "")]
        changeGroup = 1106,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "addGroup", Desc = "")]
        addGroup = 1107,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "editGroup", Desc = "")]
        editGroup = 1108,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "deleteGroup", Desc = "")]
        deleteGroup = 1109,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "changeGroupAccess", Desc = "")]
        changeGroupAccess = 1110,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "creatRole", Desc = "افزودن نقش")]
        creatRole = 1111,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "editRole", Desc = "ویرایش نقش")]
        editRole = 1112,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "deleteRole", Desc = "حذف نقش")]
        deleteRole = 1113,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "addRolePermission", Desc = "افزودن دسترسی به نقش")]
        addRolePermission = 1114,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "deleteRolePermission", Desc = "حذف دسترسی از نقش")]
        deleteRolePermission = 1115,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "addToken", Desc = "")]
        addToken = 1116,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "deleteToken", Desc = "")]
        deleteToken = 111,

        /// <summary>
        /// تغییر پروفایل توسط خود کاربر
        /// </summary>
        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "changeUserProfile", Desc = "ویرایش پروفایل")]
        editProfile = 1118,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "defineSecurePage", Desc = "تعریف صفحه ای که نیاز به لاگین مجدد دارد")]
        defineSecurePage = 1119,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "removeSecurePage", Desc = "حذف صفحه ای که نیاز به لاگین مجدد دارد")]
        removeSecurePage = 1120,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "pageSeen", Desc = "")]
        pageSeen = 1121,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "createSetting", Desc = "افزودن تنظیمات")]
        createSetting = 1122,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "editSetting", Desc = "ویرایش تنظیمات")]
        editSetting = 1123,

        [FajrActionTypeInfo(ActionId = 1101, ActionType = "userActivity", ActionSubType = "deleteSetting", Desc = "حذف تنظیمات")]
        deleteSetting = 1124,
        #endregion


        #region userManagement - مدیریت اطلاعات کاربران توسط مدیر
        [FajrActionTypeInfo(ActionId = 1201, ActionType = "userManagement", ActionSubType = "changeUserPassword", Desc = "تغییر کلمه عبور کاربر")]
        changeUserPassword = 1201,

        [FajrActionTypeInfo(ActionId = 1202, ActionType = "userManagement", ActionSubType = "unBlockUser", Desc = "رفع بلاک کاربر")]
        unBlockUser = 1202,

        /// <summary>
        /// تغییر پروفایل کاربر توسط ادمین
        /// </summary>
        [FajrActionTypeInfo(ActionId = 1203, ActionType = "userManagement", ActionSubType = "changeUserProfile", Desc = "تغییر پروفایل کاربر توسط ادمین")]
        changeUserProfile = 1203,

        [FajrActionTypeInfo(ActionId = 1204, ActionType = "userManagement", ActionSubType = "changeUserAccess", Desc = "تغییر دسترسی های کاربر")]
        changeUserAccess = 1204,

        [FajrActionTypeInfo(ActionId = 1205, ActionType = "userManagement", ActionSubType = "addUserToGroup", Desc = "")]
        addUserToGroup = 1205,

        [FajrActionTypeInfo(ActionId = 1206, ActionType = "userManagement", ActionSubType = "deleteUserFromGroup", Desc = "")]
        deleteUserFromGroup = 1206,

        [FajrActionTypeInfo(ActionId = 1207, ActionType = "userManagement", ActionSubType = "deleteUserRole", Desc = "حذف نقش از کاربر")]
        deleteUserRole = 1207,

        [FajrActionTypeInfo(ActionId = 1208, ActionType = "userManagement", ActionSubType = "addUserRole", Desc = "افزودن نقش به کاربر")]
        addUserRole = 1208,

        [FajrActionTypeInfo(ActionId = 1209, ActionType = "userManagement", ActionSubType = "tokenAssignment", Desc = "")]
        tokenAssignment = 1209,

        [FajrActionTypeInfo(ActionId = 1210, ActionType = "userManagement", ActionSubType = "AddTokenToUser", Desc = "")]
        addTokenToUser = 1210,

        [FajrActionTypeInfo(ActionId = 1211, ActionType = "userManagement", ActionSubType = "RemoveTokenFromUser", Desc = "")]
        removeTokenFromUser = 1211,

        [FajrActionTypeInfo(ActionId = 1212, ActionType = "userManagement", ActionSubType = "CreatUser", Desc = "ایجاد کاربر")]
        creatUser = 1212,

        [FajrActionTypeInfo(ActionId = 1213, ActionType = "userManagement", ActionSubType = "DeleteUser", Desc = "حذف کاربر")]
        deleteUser = 1213,

        [FajrActionTypeInfo(ActionId = 1214, ActionType = "userManagement", ActionSubType = "acceptUser", Desc = "تایید کاربر")]
        acceptUser = 1214,
        #endregion


        #region fileManagement - مدیریت فایل ها
        [FajrActionTypeInfo(ActionId = 1301, ActionType = "fileManagement", ActionSubType = "upload", Desc = "")]
        upload = 1301,

        [FajrActionTypeInfo(ActionId = 1302, ActionType = "fileManagement", ActionSubType = "download", Desc = "")]
        download = 1302,

        [FajrActionTypeInfo(ActionId = 1303, ActionType = "fileManagement", ActionSubType = "send", Desc = "")]
        send = 1303,

        [FajrActionTypeInfo(ActionId = 1304, ActionType = "fileManagement", ActionSubType = "recive", Desc = "")]
        recive = 1304,

        [FajrActionTypeInfo(ActionId = 1305, ActionType = "fileManagement", ActionSubType = "write", Desc = "")]
        write = 1305,

        [FajrActionTypeInfo(ActionId = 1306, ActionType = "fileManagement", ActionSubType = "read", Desc = "")]
        read = 1306,

        [FajrActionTypeInfo(ActionId = 1307, ActionType = "fileManagement", ActionSubType = "remove", Desc = "")]
        remove = 1307,

        [FajrActionTypeInfo(ActionId = 1308, ActionType = "fileManagement", ActionSubType = "movement", Desc = "")]
        movement = 1308,

        [FajrActionTypeInfo(ActionId = 1309, ActionType = "fileManagement", ActionSubType = "creat", Desc = "")]
        creat = 1309,

        [FajrActionTypeInfo(ActionId = 1310, ActionType = "fileManagement", ActionSubType = "delete", Desc = "")]
        delete = 1310,

        [FajrActionTypeInfo(ActionId = 1311, ActionType = "fileManagement", ActionSubType = "edit", Desc = "")]
        edit = 1311,

        [FajrActionTypeInfo(ActionId = 1312, ActionType = "fileManagement", ActionSubType = "print", Desc = "")]
        print = 1312,

        [FajrActionTypeInfo(ActionId = 1313, ActionType = "fileManagement", ActionSubType = "export", Desc = "")]
        export = 1313,

        [FajrActionTypeInfo(ActionId = 1314, ActionType = "fileManagement", ActionSubType = "view", Desc = "")]
        view = 1314,

        [FajrActionTypeInfo(ActionId = 1315, ActionType = "fileManagement", ActionSubType = "rename", Desc = "")]
        rename = 1315,
        #endregion


        #region errorManagement - مدیریت ارور ها
        [FajrActionTypeInfo(ActionId = 2101, ActionType = "errorManagement", ActionSubType = "DBConnectionError", Desc = "")]
        DBConnectionError = 2101,

        [FajrActionTypeInfo(ActionId = 2102, ActionType = "errorManagement", ActionSubType = "InputVariableError", Desc = "")]
        InputVariableError = 2102,

        [FajrActionTypeInfo(ActionId = 2103, ActionType = "errorManagement", ActionSubType = "DBMatchError", Desc = "")]
        DBMatchError = 2103,

        [FajrActionTypeInfo(ActionId = 2104, ActionType = "errorManagement", ActionSubType = "TransactionCommitError", Desc = "")]
        TransactionCommitError = 2104,

        [FajrActionTypeInfo(ActionId = 2105, ActionType = "errorManagement", ActionSubType = "AccessDeniedError", Desc = "")]
        AccessDeniedError = 2105,

        [FajrActionTypeInfo(ActionId = 2106, ActionType = "errorManagement", ActionSubType = "SessionError", Desc = "")]
        SessionError = 2106,

        [FajrActionTypeInfo(ActionId = 2107, ActionType = "errorManagement", ActionSubType = "SystemError", Desc = "")]
        SystemError = 2107,

        [FajrActionTypeInfo(ActionId = 2108, ActionType = "errorManagement", ActionSubType = "EncryptionModuleError", Desc = "")]
        EncryptionModuleError = 2108,

        [FajrActionTypeInfo(ActionId = 2109, ActionType = "errorManagement", ActionSubType = "SSLError", Desc = "")]
        SSLError = 2109,

        [FajrActionTypeInfo(ActionId = 2110, ActionType = "errorManagement", ActionSubType = "TSLError", Desc = "")]
        TSLError = 2110,

        [FajrActionTypeInfo(ActionId = 2111, ActionType = "errorManagement", ActionSubType = "RedirectionError", Desc = "")]
        RedirectionError = 2111,

        [FajrActionTypeInfo(ActionId = 2112, ActionType = "errorManagement", ActionSubType = "GenericError", Desc = "")]
        GenericError = 2112,

        [FajrActionTypeInfo(ActionId = 2113, ActionType = "errorManagement", ActionSubType = "sessionHijacking", Desc = "")]
        sessionHijacking = 2113,

        [FajrActionTypeInfo(ActionId = 2114, ActionType = "errorManagement", ActionSubType = "XSSAttack", Desc = "")]
        XSSAttack = 2114,

        [FajrActionTypeInfo(ActionId = 2115, ActionType = "errorManagement", ActionSubType = "CSRFAttacPrevention", Desc = "")]
        CSRFAttacPrevention = 2115,

        [FajrActionTypeInfo(ActionId = 2116, ActionType = "errorManagement", ActionSubType = "ConflictIP", Desc = "")]
        ConflictIP = 2116,

        [FajrActionTypeInfo(ActionId = 2117, ActionType = "errorManagement", ActionSubType = "controlledError", Desc = "")]
        controlledError = 2117,

        [FajrActionTypeInfo(ActionId = 2118, ActionType = "errorManagement", ActionSubType = "DuplicateUser", Desc = "")]
        DuplicateUser = 2118,

        [FajrActionTypeInfo(ActionId = 2119, ActionType = "errorManagement", ActionSubType = "InvalidToken", Desc = "")]
        InvalidToken = 2119,

        [FajrActionTypeInfo(ActionId = 2120, ActionType = "errorManagement", ActionSubType = "twoFactorAuthentication", Desc = "")]
        twoFactorAuthentication = 2120,

        [FajrActionTypeInfo(ActionId = 2121, ActionType = "errorManagement", ActionSubType = "NotValidRequest", Desc = "")]
        NotValidRequest = 2121,

        [FajrActionTypeInfo(ActionId = 2122, ActionType = "errorManagement", ActionSubType = "ViewPageResourceNotFound", Desc = "")]
        ViewPageResourceNotFound = 2122,

        [FajrActionTypeInfo(ActionId = 2123, ActionType = "errorManagement", ActionSubType = "BlockUser", Desc = "")]
        BlockUser = 2123,

        [FajrActionTypeInfo(ActionId = 2124, ActionType = "errorManagement", ActionSubType = "LisenceExpired", Desc = "")]
        LisenceExpired = 2124,

        #endregion


    }
}

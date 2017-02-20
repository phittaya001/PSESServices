using System.Collections.Generic;
using System.Linq;
using CSI.ModelHelper.Paging;
using System.Transactions;
using AutoMapper;
using CSI.ModelHelper.Cache;
using CSI.Security;
using CSI.CastleWindsorHelper;
using PesWeb.Service.Common;

namespace PesWeb.Service.Security
{
    public class UserMaintenanceSvc : IPagingable<tbs_User, tbs_User>
    {
        public virtual int Count(tbs_User condition)
        {
            using (SecurityEntities sc = new SecurityEntities())
            {
                var query = from u in sc.tbs_User select u;
                if (false == string.IsNullOrEmpty(condition.GroupCodeParam))
                    query = from u in sc.tbs_User
                            join g in sc.tbs_UserGroup
                            on u.UserCode equals g.UserCode
                            where g.GroupCode == condition.GroupCodeParam
                            select u;

                if (false == string.IsNullOrEmpty(condition.UserCode))
                    query = query.Where(u => u.UserCode == condition.UserCode);
                if (false == string.IsNullOrEmpty(condition.LoginName))
                    query = query.Where(u => 0 == string.Compare(u.LoginName, condition.LoginName, true));
                if (condition.IsActiveParam.HasValue)
                    query = query.Where(u => u.IsActive == condition.IsActiveParam.Value);
                if (false == string.IsNullOrEmpty(condition.Description))
                    query = query.Where(u => u.Description.Contains(condition.Description));

                return query.Count();
            }
        }

        public virtual List<tbs_User> GetPage(tbs_User condition)
        {
            using (SecurityEntities sc = new SecurityEntities())
            {
                var query = from u in sc.tbs_User select u;
                if (false == string.IsNullOrEmpty(condition.GroupCodeParam))
                    query = from u in sc.tbs_User
                            join g in sc.tbs_UserGroup
                            on u.UserCode equals g.UserCode
                            where g.GroupCode == condition.GroupCodeParam
                            select u;

                if (false == string.IsNullOrEmpty(condition.UserCode))
                    query = query.Where(u => u.UserCode == condition.UserCode);
                if (false == string.IsNullOrEmpty(condition.LoginName))
                    query = query.Where(u => 0 == string.Compare(u.LoginName, condition.LoginName, true));
                if (condition.IsActiveParam.HasValue)
                    query = query.Where(u => u.IsActive == condition.IsActiveParam.Value);
                if (false == string.IsNullOrEmpty(condition.Description))
                    query = query.Where(u => u.Description.Contains(condition.Description));

                return query.Page(condition).ToList();
            }
        }
        public virtual List<tbs_User> GetUsers(tbs_User condition)
        {
            return GetPage(condition);
        }

        public virtual List<tbs_Group> GetGroups(string userCode)
        {
            using (SecurityEntities sc = new SecurityEntities())
            {
                var groups = (from u in sc.tbs_User
                              join ug in sc.tbs_UserGroup on u.UserCode equals ug.UserCode
                              join g in sc.tbs_Group on ug.GroupCode equals g.GroupCode
                              where u.UserCode == userCode
                              select g).Distinct().ToList();
                return groups;
            }
        }

        public virtual bool Exist(string userCode, string LoginName)
        {
            using (SecurityEntities sc = new SecurityEntities())
            {
                return sc.tbs_User
                    .Where(u => (0 == string.Compare(u.LoginName, LoginName, true) || LoginName == null)
                             && (0 == string.Compare(u.UserCode, userCode, true) || userCode == null))
                    .Count() > 0;
            }
        }

        public virtual void CreateUser(tbs_User user)
        {
            using (SecurityEntities sc = new SecurityEntities())
            using (TransactionScope ts = new TransactionScope())
            {
                var mapper = ServiceContainer.GetService<IMapper>();
                tbs_User newUser = mapper.Map<tbs_User>(user);
                newUser.Password = SecurityModelCrypto.HashEncrypt(user.Password);
                sc.tbs_User.Add(newUser);
                sc.SaveChanges();
                ts.Complete();
            }
        }

        [FlushCache(CacheTags.User)]
        public virtual void UpdateUser(tbs_User user)
        {
            using (SecurityEntities sc = new SecurityEntities())
            using (TransactionScope ts = new TransactionScope())
            {
                var mapper = ServiceContainer.GetService<IMapper>();

                var passList = sc.tbs_User.Where(u => u.UserCode == user.UserCode).Select(u => u.Password).ToList();
                string pass = passList.Count > 0 ? passList[0] : string.Empty;
                tbs_User edited = mapper.Map<tbs_User>(user);
                if (false == edited.Password.Equals(pass) || string.IsNullOrEmpty(pass))
                    edited.Password = SecurityModelCrypto.HashEncrypt(edited.Password);

                sc.tbs_User.Attach(edited);
                sc.Entry(edited).State = System.Data.Entity.EntityState.Modified;
                sc.SaveChanges();
                ts.Complete();
            }
        }
        [FlushCache(CacheTags.UserGroup, CacheTags.User)]
        public virtual void DeleteUser(List<string> userCodes)
        {
            if (userCodes.Count > 0)
            {
                var targets = userCodes.ToArray();
                using (SecurityEntities sc = new SecurityEntities())
                using (TransactionScope ts = new TransactionScope())
                {
                    sc.Database.ExecuteSqlCommand(tbs_UserGroup.GetSqlDeleteByUser(targets));
                    sc.Database.ExecuteSqlCommand(tbs_User.GetSqlDelete(targets));
                    ts.Complete();
                }
            }
        }
        public virtual bool ChangePassword(string userCode, string oldPassword, string newPassword)
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                string encryptedOld = SecurityModelCrypto.HashEncrypt(oldPassword);
                var users = db.tbs_User
                    .Where(a => (a.UserCode == userCode)
                            && a.Password == encryptedOld)
                    .ToList();
                if (users.Count != 1)
                    return false;

                string encryptedNew = SecurityModelCrypto.HashEncrypt(newPassword);
                users[0].Password = encryptedNew;
                db.Entry(users[0]).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }
    }
}

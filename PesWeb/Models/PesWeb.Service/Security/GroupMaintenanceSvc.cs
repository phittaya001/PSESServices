using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSI.ModelHelper.Paging;
using System.Transactions;
using AutoMapper;
using CSI.ModelHelper.Cache;
using CSI.CastleWindsorHelper;
using PesWeb.Service.Common;

namespace PesWeb.Service.Security
{
    public class GroupMaintenanceSvc : IPagingable<tbs_Group, tbs_Group>
    {
        public virtual int Count(tbs_Group condition)
        {
            using (SecurityEntities sc = new SecurityEntities())
            {
                var query = from g in sc.tbs_Group select g;

                if (false == string.IsNullOrEmpty(condition.GroupCode))
                    query = query.Where(g => g.GroupCode == condition.GroupCode);
                if (false == string.IsNullOrEmpty(condition.GroupName))
                    query = query.Where(g => 0 == string.Compare(g.GroupName, condition.GroupName, true));
                if (condition.IsActiveParam.HasValue)
                    query = query.Where(g => g.IsActive == condition.IsActiveParam.Value);
                if (false == string.IsNullOrEmpty(condition.Description))
                    query = query.Where(g => g.Description.Contains(condition.Description));

                return query.Count();
            }
        }

        public virtual List<tbs_Group> GetPage(tbs_Group condition)
        {
            using (SecurityEntities sc = new SecurityEntities())
            {
                var query = from g in sc.tbs_Group select g;

                if (false == string.IsNullOrEmpty(condition.GroupCode))
                    query = query.Where(g => g.GroupCode == condition.GroupCode);
                if (false == string.IsNullOrEmpty(condition.GroupName))
                    query = query.Where(g => 0 == string.Compare(g.GroupName, condition.GroupName, true));
                if (condition.IsActiveParam.HasValue)
                    query = query.Where(g => g.IsActive == condition.IsActiveParam.Value);
                if (false == string.IsNullOrEmpty(condition.Description))
                    query = query.Where(g => g.Description.Contains(condition.Description));

                return query.Page(condition).ToList();
            }
        }
        public virtual List<tbs_Group> GetGroups(tbs_Group condition)
        {
            return GetPage(condition);
        }
        public virtual List<tbs_User> GetMembers(string groupCode)
        {
            using (SecurityEntities sc = new SecurityEntities())
            {
                var query = from u in sc.tbs_User
                            join ug in sc.tbs_UserGroup on u.UserCode equals ug.UserCode
                            where ug.GroupCode == groupCode
                            select u;

                return query.ToList();
            }
        }

        public virtual bool Exist(string groupCode, string groupName)
        {
            using (SecurityEntities sc = new SecurityEntities())
            {
                return sc.tbs_Group
                    .Where(g => (0 == string.Compare(g.GroupName, groupName, true) || groupName == null)
                             && (0 == string.Compare(g.GroupCode, groupCode, true) || groupCode == null))
                    .Count() > 0;
            }
        }

        public virtual void CreateGroup(tbs_Group group)
        {
            using (SecurityEntities sc = new SecurityEntities())
            using (TransactionScope ts = new TransactionScope())
            {
                var mapper = ServiceContainer.GetService<IMapper>();
                tbs_Group newGroup = mapper.Map<tbs_Group>(group);
                sc.tbs_Group.Add(newGroup);
                sc.SaveChanges();
                ts.Complete();
            }
        }

        public virtual void UpdateGroup(tbs_Group group)
        {
            using (SecurityEntities sc = new SecurityEntities())
            using (TransactionScope ts = new TransactionScope())
            {
                var mapper = ServiceContainer.GetService<IMapper>();
                tbs_Group edited = mapper.Map<tbs_Group>(group);
                sc.tbs_Group.Attach(edited);
                sc.Entry(edited).State = System.Data.Entity.EntityState.Modified;
                sc.SaveChanges();
                ts.Complete();
            }
        }

        [FlushCache(CacheTags.UserGroup)]
        public virtual void UpdateGroupMember(UpdateGroupMemberParam updates)
        {
            using (SecurityEntities sc = new SecurityEntities())
            using (TransactionScope ts = new TransactionScope())
            {
                StringBuilder sb = new StringBuilder();
                foreach (var m in updates.out_Members)
                {
                    sb.Append(tbs_UserGroup.GetSqlDelete(m.UserCode, m.GroupCode));
                    sb.AppendLine(";");
                }
                foreach (var m in updates.in_Members)
                {
                    sb.Append(tbs_UserGroup.GetSqlDelete(m.UserCode, m.GroupCode));
                    sb.AppendLine(";");
                }
                string sql = sb.ToString();
                if (false == string.IsNullOrEmpty(sql))
                    sc.Database.ExecuteSqlCommand(sql);

                foreach (var m in updates.in_Members)
                {
                    sc.tbs_UserGroup.Add(
                    new tbs_UserGroup
                    {
                        GroupCode = m.GroupCode,
                        UserCode = m.UserCode,
                    });
                    sc.SaveChanges();
                }
                ts.Complete();
            }
        }

        [FlushCache(CacheTags.UserGroup)]
        public virtual void DeleteGroup(List<string> groupCodes)
        {
            if (groupCodes.Count > 0)
            {
                var targets = groupCodes.ToArray();
                using (SecurityEntities sc = new SecurityEntities())
                using (TransactionScope ts = new TransactionScope())
                {
                    sc.Database.ExecuteSqlCommand(tbs_UserGroup.GetSqlDeleteByGroup(targets));
                    sc.Database.ExecuteSqlCommand(tbs_Group.GetSqlDelete(targets));
                    ts.Complete();
                }
            }
        }
    }

    public partial class UpdateGroupMemberParam
    {
        public List<tbs_UserGroup> in_Members { get; set; }
        public List<tbs_UserGroup> out_Members { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;
using BCCStudents.Application.Services.Sync.UpStream;
using BCCStudents.Application.Services.Sync;

using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services
{
    public class SubGroupService : ISubGroupService
    {
        private readonly ISubGroupRepository _subGroupRepository;
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;

        public SubGroupService(ISubGroupRepository subGroupRepository, IDatabaseConnectionProvider connectionProvider, IUpStreamChangeTracker upStreamChangeTracker)
        {
            _subGroupRepository = subGroupRepository;
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _upStreamChangeTracker = upStreamChangeTracker ?? throw new ArgumentNullException(nameof(upStreamChangeTracker));
        }
        

        public void AddSubGroups(SubGroup subGroup, int subGroupCount)
        {
            for (int i = 1; i <= subGroupCount; i++)
            {
                subGroup.Name = $"კლასი {i}";
                int newSubGroupId = _subGroupRepository.AddSubGroup(subGroup);
                SyncSubGroupSnapshot(newSubGroupId, SyncOperationType.Insert);
            }
        }
        public int AddSubGroup(SubGroup subGroup)
        {
            int newSubGroupId = _subGroupRepository.AddSubGroup(subGroup);
            SyncSubGroupSnapshot(newSubGroupId, SyncOperationType.Insert);
            return newSubGroupId;
        }
        public void DeleteStudentFromSubGroup(int studentId, int groupId)
        {
            var snapshots = GetStudentSubGroupSnapshots(studentId, groupId);
            _subGroupRepository.DeleteStudentFromSubGroup(studentId, groupId);
            foreach (var snapshot in snapshots)
            {
                _upStreamChangeTracker.TrackStudentSubGroupChange(snapshot.Id, SyncOperationType.Delete, snapshot);
            }
        }
        public List<SubGroup> GetStudentSubGroupsByStudentId(int studentId)
        { 
            return _subGroupRepository.GetStudentSubGroupsByStudentId(studentId); 
        }
        public List<SubGroup> GetSubGroupsByGroupId(int groupId)
        {
            return _subGroupRepository.GetSubGroupsByGroupId(groupId);
        }
        public int GetCurrentStudentSubGroupId(int studentId, int groupId, bool status)
        { return _subGroupRepository.GetCurrentStudentSubGroupId(studentId, groupId, status); }
        public int GetStudentCountInSubGroup(int subGroupId)
        { return _subGroupRepository.GetStudentCountInSubGroup(subGroupId); }
        public DataTable GetAllSubGroupsFor()
        {
            return _subGroupRepository.GetAllSubGroupsFor();
        }
        public List<SubGroup> GetAllSubGroups()
        {
            return _subGroupRepository.GetAllSubGroups();
        }
        public SubGroup GetFirstSubGroupByGroupId(int groupId)
        { return _subGroupRepository.GetFirstSubGroupByGroupId(groupId); }
        public void UpdateStudentSubGroupPaymentDate(int studentId, int groupId, int subGroupId, DateTime paymentDate)
        { _subGroupRepository.UpdateStudentSubGroupPaymentDate(studentId, groupId, subGroupId, paymentDate); }
        public void UpdateSubGroupStudentCount(int subGroupId, int count)
        { _subGroupRepository.UpdateSubGroupStudentCount(subGroupId, count); }
        public void UpdateStudentSubGroup(int studentId, int groupId, int subGroupId, int oldSubGroupId)
        {
            // განვაახლოთ SubGroupId StudentSubGroups ცხრილში
            _subGroupRepository.UpdateStudentSubGroup(studentId, groupId, subGroupId, oldSubGroupId);
            
            // განვაახლოთ StudentCount ორივე ქვეჯგუფისთვის (ძველი და ახალი)
            if (oldSubGroupId > 0 && oldSubGroupId != subGroupId)
            {
                // ძველი ქვეჯგუფის StudentCount-ის შემცირება (-1)
                _subGroupRepository.DecreaseStudentCount(oldSubGroupId);
                SyncSubGroupSnapshot(oldSubGroupId, SyncOperationType.Update);
                
                // ახალი ქვეჯგუფის StudentCount-ის გაზრდა (+1)
                _subGroupRepository.IncrementSubGroupCount(subGroupId, null, null);
                SyncSubGroupSnapshot(subGroupId, SyncOperationType.Update);
            }
            
            // StudentSubGroups-ის სინქრონიზაცია
            SyncStudentSubGroupSnapshot(studentId, groupId, subGroupId, SyncOperationType.Update);
        }
        public bool UpdateStudentSubGroupPaymentStatus(int studentId, int groupId, int subGroupId, string status)
        {
            return _subGroupRepository.UpdateStudentSubGroupPaymentStatus(studentId, groupId, subGroupId, status);
        }
        public void IncrementSubGroupCount(int subGroupId, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null)
        { _subGroupRepository.IncrementSubGroupCount(subGroupId,externalConnection,externalTransaction); }
        public void DecreaseStudentCount(int SubGroupId)
        { _subGroupRepository.DecreaseStudentCount(SubGroupId); }
        public SubGroup GetSubGroupById(int Id)
        {
            return _subGroupRepository.GetSubGroupById(Id);
        }
        public SubGroup GetSubGroupByNumber(int groupId, int Id)
        {
            return _subGroupRepository.GetSubGroupByNumber(groupId, Id);
        }
        /*public void DeleteSubGroup(int Id)
        {
            _subGroupRepository.DeleteSubGroup(Id);
        }*/
        /// <summary>
        /// Removes all StudentSubGroups links for a student in a given group.
        /// </summary>
        public void RemoveStudentFromAllSubGroups(int studentId, int groupId)
        {
            // Get SubGroupIds before deletion for recount
            var subGroupIds = new List<int>();
            try
            {
                using (var conn = _connectionProvider.GetLocalConnection())
                {
                    conn.Open();
                    // ვიღებთ SubGroupIds-ს soft delete-ის წინ
                    using (var cmd = new MySqlCommand("SELECT DISTINCT SubGroupId FROM StudentSubGroups WHERE StudentId=@sid AND GroupId=@gid AND (IsDeleted=0 OR IsDeleted IS NULL) AND Status=1", conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", studentId);
                        cmd.Parameters.AddWithValue("@gid", groupId);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                subGroupIds.Add(Convert.ToInt32(r["SubGroupId"]));
                            }
                        }
                    }
                }
            }
            catch { }

            // Soft delete: Status=0, IsDeleted=1
            DeleteStudentFromSubGroup(studentId, groupId);
            
            // Recalculate SubGroup counts after deletion (გამოვაკლოთ 1 თითოეულ ქვეჯგუფს)
            // DecreaseStudentCount ამოწმებს IsDeleted=0-ს, ამიტომ soft delete-ის შემდეგ ის ავტომატურად გამოაკლებს 1-ს
            foreach (var subGroupId in subGroupIds)
            {
                try
                {
                    _subGroupRepository.DecreaseStudentCount(subGroupId);
                    SyncSubGroupSnapshot(subGroupId, SyncOperationType.Update);
                }
                catch { }
            }
        }

        /// <summary>
        /// Update an existing subgroup
        /// </summary>
        public bool UpdateSubGroup(SubGroup subGroup)
        {
            try
            {
                _subGroupRepository.UpdateSubGroup(subGroup);
                SyncSubGroupSnapshot(subGroup.Id, SyncOperationType.Update);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete a subgroup by ID
        /// </summary>
        public bool DeleteSubGroup(int subGroupId)
        {
            try
            {
                var snapshot = _subGroupRepository.GetSubGroupById(subGroupId);
                _subGroupRepository.DeleteSubGroup(subGroupId);
                if (snapshot != null)
                {
                    _upStreamChangeTracker.TrackSubGroupChange(subGroupId, SyncOperationType.Delete, snapshot);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update status of all subgroups for a given group
        /// </summary>
        public bool UpdateSubGroupsStatusByGroupId(int groupId, bool status)
        {
            try
            {
                // 1) Update locally (all subgroups under the group)
                _subGroupRepository.UpdateSubGroupsStatusByGroupId(groupId, status);
                var updatedSubGroups = _subGroupRepository.GetSubGroupsByGroupId(groupId);
                foreach (var subGroup in updatedSubGroups)
                {
                    _upStreamChangeTracker.TrackSubGroupChange(subGroup.Id, SyncOperationType.Update, subGroup);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update status of all subgroups for a given group
        /// </summary>
        /// 
        public bool UpdateStudentSubGroupStatus(int groupId, int studentId, int subGroupId, bool status)
        {
            try
            {
                
                return _subGroupRepository.UpdateStudentSubGroupStatus(groupId, studentId, subGroupId, status);
            }
            catch
            {
                return false;
            }
        }
        #region Sync Helpers

        /// <summary>
        /// Loads single subgroup and triggers UpStream change tracking.
        /// </summary>
        private void SyncSubGroupSnapshot(int subGroupId, SyncOperationType operation)
        {
            try
            {
                var subGroup = _subGroupRepository.GetSubGroupById(subGroupId);
                if (subGroup != null)
                {
                    _upStreamChangeTracker.TrackSubGroupChange(subGroupId, operation, subGroup);
                }
            }
            catch { }
        }

        /// <summary>
        /// Loads single StudentSubGroups record and triggers UpStream change tracking.
        /// </summary>
        private void SyncStudentSubGroupSnapshot(int studentId, int groupId, int subGroupId, SyncOperationType operation)
        {
            try
            {
                using (var connection = _connectionProvider.GetMySqlConnection()) 
                {
                    connection.Open();
                    const string sql = @"SELECT Id, StudentId, GroupId, SubGroupId, Status, PaymentStatus, DateOfPayment, Price, Discount, UpdatedAt
                                         FROM StudentSubGroups
                                         WHERE StudentId = @sid AND GroupId = @gid AND SubGroupId = @subId
                                         ORDER BY Id DESC
                                         LIMIT 1";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@sid", studentId);
                        command.Parameters.AddWithValue("@gid", groupId);
                        command.Parameters.AddWithValue("@subId", subGroupId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var snapshot = new StudentSubGroups
                                {
                                    Id = reader.GetInt32("Id"),
                                    StudentId = reader.GetInt32("StudentId"),
                                    GroupId = reader.GetInt32("GroupId"),
                                    SubGroupId = reader.GetInt32("SubGroupId"),
                                    Status = reader["Status"] != DBNull.Value && reader.GetBoolean("Status"),
                                    PaymentStatus = reader["PaymentStatus"] == DBNull.Value ? null : reader.GetString("PaymentStatus"),
                                    DateOfPayment = reader["DateOfPayment"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime("DateOfPayment"),
                                    Price = reader["Price"] == DBNull.Value ? 0 : reader.GetDecimal("Price"),
                                    Discount = reader["Discount"] == DBNull.Value ? 0 : reader.GetDouble("Discount"),
                                    UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.MinValue : reader.GetDateTime("UpdatedAt")
                                };
                                _upStreamChangeTracker.TrackStudentSubGroupChange(snapshot.Id, operation, snapshot);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Loads all StudentSubGroups records for the student/group combination (before deletion).
        /// </summary>
        private List<StudentSubGroups> GetStudentSubGroupSnapshots(int studentId, int groupId)
        {
            var snapshots = new List<StudentSubGroups>();
            try
            {
                using (var connection = _connectionProvider.GetMySqlConnection())
                {
                    connection.Open();
                    const string sql = @"SELECT Id, StudentId, GroupId, SubGroupId, Status, PaymentStatus, DateOfPayment, Price, Discount, UpdatedAt
                                         FROM StudentSubGroups
                                         WHERE StudentId = @sid AND GroupId = @gid";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@sid", studentId);
                        command.Parameters.AddWithValue("@gid", groupId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                snapshots.Add(new StudentSubGroups
                                {
                                    Id = reader.GetInt32("Id"),
                                    StudentId = reader.GetInt32("StudentId"),
                                    GroupId = reader.GetInt32("GroupId"),
                                    SubGroupId = reader.GetInt32("SubGroupId"),
                                    Status = reader["Status"] != DBNull.Value && reader.GetBoolean("Status"),
                                    PaymentStatus = reader["PaymentStatus"] == DBNull.Value ? null : reader.GetString("PaymentStatus"),
                                    DateOfPayment = reader["DateOfPayment"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime("DateOfPayment"),
                                    Price = reader["Price"] == DBNull.Value ? 0 : reader.GetDecimal("Price"),
                                    Discount = reader["Discount"] == DBNull.Value ? 0 : reader.GetDouble("Discount"),
                                    UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.MinValue : reader.GetDateTime("UpdatedAt")
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return snapshots;
        }

        #endregion
    }
}





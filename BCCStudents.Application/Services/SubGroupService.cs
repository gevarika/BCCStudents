using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;

namespace BCCStudents.Application.Services
{
    public class SubGroupService : ISubGroupService
    {
        private readonly ISubGroupRepository _subGroupRepository;
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public SubGroupService(ISubGroupRepository subGroupRepository, IDatabaseConnectionProvider connectionProvider)
        {
            _subGroupRepository = subGroupRepository;
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }


        public void AddSubGroups(SubGroup subGroup, int subGroupCount)
        {
            for (int i = 1; i <= subGroupCount; i++)
            {
                subGroup.Name = $"კლასი {i}";
                _subGroupRepository.AddSubGroup(subGroup);
            }
        }
        public int AddSubGroup(SubGroup subGroup)
        {
            return _subGroupRepository.AddSubGroup(subGroup);
        }
        public void DeleteStudentFromSubGroup(int studentId, int groupId)
        {
            _subGroupRepository.DeleteStudentFromSubGroup(studentId, groupId);
        }
        public List<SubGroup> GetStudentSubGroupsByStudentId(int studentId)
        {
            return _subGroupRepository.GetStudentSubGroupsByStudentId(studentId);
        }
        public List<SubGroup> GetSubGroupsByGroupId(int groupId)
        {
            return _subGroupRepository.GetSubGroupsByGroupId(groupId);
        }

        public List<SubGroup> GetAllSubGroupsByGroupId(int groupId)
        {
            return _subGroupRepository.GetAllSubGroupsByGroupId(groupId);
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
        {
            _subGroupRepository.UpdateStudentSubGroupPaymentDate(studentId, groupId, subGroupId, paymentDate);
        }

        public void UpdateStudentSubGroup(int studentId, int groupId, int subGroupId, int oldSubGroupId)
        {
            // განვაახლოთ SubGroupId StudentSubGroups ცხრილში
            _subGroupRepository.UpdateStudentSubGroup(studentId, groupId, subGroupId, oldSubGroupId);

            // განვაახლოთ StudentCount ორივე ქვეჯგუფისთვის (ძველი და ახალი)
            if (oldSubGroupId > 0 && oldSubGroupId != subGroupId)
            {
                // ძველი ქვეჯგუფის StudentCount-ის შემცირება (-1)
                _subGroupRepository.DecreaseStudentCount(oldSubGroupId);

                // ახალი ქვეჯგუფის StudentCount-ის გაზრდა (+1)
                _subGroupRepository.IncrementSubGroupCount(subGroupId, null, null);
            }
        }
        public bool UpdateStudentSubGroupPaymentStatus(int studentId, int groupId, int subGroupId, string status)
        {
            return _subGroupRepository.UpdateStudentSubGroupPaymentStatus(studentId, groupId, subGroupId, status);
        }
        /*public void UpdateSubGroupStudentCount(int subGroupId, int count)
        { _subGroupRepository.UpdateSubGroupStudentCount(subGroupId, count); }
        /*public void IncrementSubGroupCount(int subGroupId, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null)
        { _subGroupRepository.IncrementSubGroupCount(subGroupId,externalConnection,externalTransaction); }*/
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
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ქვეჯგუფის სრული წაშლა (Hard Delete).
        /// აქტიური მოსწავლეების არსებობისას იკრძალება.
        /// </summary>
        public bool DeleteSubGroup(int subGroupId)
        {
            try
            {
                var snapshot = _subGroupRepository.GetSubGroupById(subGroupId);
                if (snapshot == null)
                    return false;

                var activeStudents = _subGroupRepository.GetStudentCountInSubGroup(subGroupId);
                if (activeStudents > 0)
                {
                    throw new InvalidOperationException(
                        $"ქვეჯგუფში არის {activeStudents} აქტიური მოსწავლე. ჯერ გადაიტანეთ ან ამოიღეთ მოსწავლეები, შემდეგ წაშალეთ ქვეჯგუფი.");
                }

                return _subGroupRepository.DeleteSubGroup(subGroupId);
            }
            catch (InvalidOperationException)
            {
                throw;
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
                _subGroupRepository.UpdateSubGroupsStatusByGroupId(groupId, status);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update tuition fee of all subgroups for a given group
        /// </summary>
        public bool UpdateSubGroupsTuitionFeeByGroupId(int groupId, decimal newFee)
        {
            try
            {
                _subGroupRepository.UpdateSubGroupsTuitionFeeByGroupId(groupId, newFee);
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
    }
}

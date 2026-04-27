using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    public class PendingStudentGroupRepository : IPendingStudentGroupRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;
        public PendingStudentGroupRepository(IDatabaseConnectionProvider connectionProvider)
        { _connectionProvider = connectionProvider; }
        public List<int> GetGroupsForPendingStudent(int pendingStudentId)
        {
            var groupIds = new List<int>();

            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = "SELECT GroupId FROM PendingStudentGroups WHERE StudentId = @StudentId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", pendingStudentId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            groupIds.Add(Convert.ToInt32(reader["GroupId"]));
                        }
                    }
                }
            }

            return groupIds;
        }
        public List<PendingStudentSubGroup> GetPendingSubGroupsByStudentId(int studentId)
        {
            var list = new List<PendingStudentSubGroup>();
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                var query = "SELECT GroupId, SubGroupId FROM PendingStudentSubGroups WHERE StudentId = @StudentId";
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new PendingStudentSubGroup
                            {
                                GroupId = reader.GetInt32("GroupId"),
                                SubGroupId = reader.GetInt32("SubGroupId")
                            });
                        }
                    }
                }
            }

            return list;
        }

        /*public void AddGroupForPendingStudent(int pendingStudentId, int groupId)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = "INSERT INTO PendingStudentGroups (PendingStudentId, GroupId) VALUES (@PendingStudentId, @GroupId)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PendingStudentId", pendingStudentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.ExecuteNonQuery();
                }
            }
        }*/

        public void DeleteByPendingStudentId(int pendingStudentId)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = "DELETE FROM PendingStudentGroups WHERE PendingStudentId = @PendingStudentId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PendingStudentId", pendingStudentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}



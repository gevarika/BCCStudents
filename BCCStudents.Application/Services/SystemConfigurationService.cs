using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services
{
    public class SystemConfigurationService : ISystemConfigurationService
    {
        private readonly ISystemConfigurationRepository _repository;

        public SystemConfigurationService(ISystemConfigurationRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public DateTime? GetStudyStartDate()
        {
            return _repository.GetStudyStartDate();
        }

        public void SetStudyStartDate(DateTime date)
        {
            _repository.SetStudyStartDate(date);
        }

        public DateTime? GetDefaultPaymentDate()
        {
            return _repository.GetDefaultPaymentDate();
        }

        public void SetDefaultPaymentDate(DateTime date)
        {
            _repository.SetDefaultPaymentDate(date);
        }

        public List<(DateTime StartDate, DateTime EndDate)> GetVacationPeriods()
        {
            return _repository.GetVacationPeriods();
        }

        public void AddVacationPeriod(DateTime startDate, DateTime endDate, string description = null)
        {
            if (startDate >= endDate)
            {
                throw new ArgumentException("დასვენების დაწყების თარიღი უნდა იყოს დასრულების თარიღზე ადრე");
            }

            _repository.AddVacationPeriod(startDate, endDate, description);
        }

        public void DeleteVacationPeriod(DateTime startDate, DateTime endDate)
        {
            _repository.DeleteVacationPeriod(startDate, endDate);
        }

        public bool IsVacationDate(DateTime date)
        {
            return IsDateInVacationPeriod(date);
        }

        public bool IsDateInVacationPeriod(DateTime date)
        {
            var vacationPeriods = GetVacationPeriods();
            return vacationPeriods.Any(period => date.Date >= period.StartDate.Date && date.Date <= period.EndDate.Date);
        }
    }
}
